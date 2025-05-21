using DiskCardGame;
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.TextCore;
using System.Reflection;

namespace SquirrelBombMod.Spamton
{
    [HarmonyPatch]
    public static class SpamtonTextDisplayer
    {
		private static TextDisplayer.SpeakerTextStyle _sneostyle;
		public static bool sneoTextShouldBePink;
		public static Color glassYellow = new Color32(255, 242, 0, 255);
		public static Color glassPink = new Color32(255, 174, 201, 255);

        public static MethodInfo sstc_s = AccessTools.Method(typeof(SpamtonTextDisplayer), nameof(SwitchSpamtonTextColors_Switch));
        public static MethodInfo rscp_stm = AccessTools.Method(typeof(SpamtonTextDisplayer), nameof(RemoveSpamtonCodeParsing_SaveTranslatedMessage));
        public static MethodInfo rscp_rpm = AccessTools.Method(typeof(SpamtonTextDisplayer), nameof(RemoveSpamtonCodeParsing_ReplaceParsedMessage));

        public static TextDisplayer.SpeakerTextStyle SneoStyle
        {
            get
            {
                if (_sneostyle != null)
                {
                    if (TextDisplayer.Instance != null && TextDisplayer.Instance.alternateSpeakerStyles != null && !TextDisplayer.Instance.alternateSpeakerStyles.Contains(_sneostyle))
                        TextDisplayer.Instance.alternateSpeakerStyles.Add(_sneostyle);

                    return _sneostyle;
                }

                if (TextDisplayer.Instance == null || TextDisplayer.Instance.alternateSpeakerStyles == null)
                    return null;

                _sneostyle = TextDisplayer.Instance.alternateSpeakerStyles.Find(x => x.speaker == SpamtonSetup.spamtonSpeaker);
                if (_sneostyle != null)
                    return _sneostyle;

                var piratestyle = TextDisplayer.Instance.alternateSpeakerStyles.Find(x => x.speaker == Speaker.PirateSkull);
                if (piratestyle == null)
                    return null;

                sneoTextShouldBePink = false;
                _sneostyle = new()
                {
                    color = glassYellow,
                    font = piratestyle.font,
                    fontSizeChange = piratestyle.fontSizeChange,
                    speaker = SpamtonSetup.spamtonSpeaker,
                    triangleOffset = piratestyle.triangleOffset,
                    triangleSprite = piratestyle.triangleSprite,
                    voiceSoundVolume = piratestyle.voiceSoundVolume,
                    voiceSoundIdPrefix = "sneo"
                };
                TextDisplayer.Instance.alternateSpeakerStyles.Add(_sneostyle);
                return _sneostyle;
            }
        }

        [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowMessage))]
        [HarmonyPrefix]
        public static void AddSpamtonTextStyle_Prefix()
		{
			_ = SneoStyle;
		}

		[HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowMessage))]
		[HarmonyILManipulator]
		public static void SwitchSpamtonTextColors_Transpiler(ILContext ctx)
		{
			var crs = new ILCursor(ctx);

			if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<TextDisplayer>(nameof(TextDisplayer.PlayVoiceSound))))
				return;

            crs.Emit(OpCodes.Ldarg, 4);
			crs.Emit(OpCodes.Call, sstc_s);
		}

		public static void SwitchSpamtonTextColors_Switch(Speaker speaker)
        {
			if (speaker != SpamtonSetup.spamtonSpeaker)
				return;

			if (SneoStyle is not TextDisplayer.SpeakerTextStyle sneoStyle)
				return;

            sneoStyle.color = (sneoTextShouldBePink = !sneoTextShouldBePink) ? glassPink : glassYellow;
        }

        [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowUntilInput), MethodType.Enumerator)]
		[HarmonyILManipulator]
		public static void RemoveSpamtonCodeParsing_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

			if(!crs.JumpToNext(x => x.MatchCallOrCallvirt(typeof(Localization), nameof(Localization.Translate))))
				return;

			var translatedMsgLoc = crs.DeclareLocal<string>();
			crs.Emit(OpCodes.Ldloca, translatedMsgLoc);
			crs.Emit(OpCodes.Call, rscp_stm);

			if (!crs.JumpToNext(x => x.MatchCallOrCallvirt(typeof(DialogueParser), nameof(DialogueParser.ParseDialogueCodes))))
				return;
            
			crs.Emit(OpCodes.Ldarg_0);
			crs.Emit(OpCodes.Ldloc, translatedMsgLoc);
			crs.Emit(OpCodes.Call, rscp_rpm);
            crs.EmitDelegate((string msg) =>
            {
                Debug.Log($"got this: {msg}");
                return msg;
            });

            if (!crs.JumpToNext(x => x.MatchStfld(out _)))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitDelegate((IEnumerator enumerator) =>
            {
                Debug.Log($"saved fld: {enumerator.EnumeratorGetField<string>("transformedMessage")}");
            });

            Debug.Log("made it to the end yippee");
        }

		public static string RemoveSpamtonCodeParsing_SaveTranslatedMessage(string curr, out string translatedMessage)
		{
			translatedMessage = curr;
            Debug.Log($"translated message save: {translatedMessage}");

            return curr;
		}

		public static string RemoveSpamtonCodeParsing_ReplaceParsedMessage(string curr, IEnumerator enumerator, string translatedMessage)
		{
			var speaker = enumerator.EnumeratorGetField<Speaker>("speaker");

            Debug.Log($"speaker: {speaker}. spamton speaker: {SpamtonSetup.spamtonSpeaker}.");
            Debug.Log($"current: {curr}");
            Debug.Log($"translated save: {translatedMessage}");

			if (speaker != SpamtonSetup.spamtonSpeaker)
				return curr;

            Debug.Log("REPLACING");

			return translatedMessage;
		}
    }
}
