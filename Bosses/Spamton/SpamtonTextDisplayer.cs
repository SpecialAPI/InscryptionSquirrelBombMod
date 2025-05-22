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
        public static MethodInfo sstc_mrb = AccessTools.Method(typeof(SpamtonTextDisplayer), nameof(SwitchSpamtonTextColors_MaybeReplaceBrackets));
        public static MethodInfo isb_di = AccessTools.Method(typeof(SpamtonTextDisplayer), nameof(InsertSpamtonBrackets_DoInsert));

        public const string SpamtonDialogueCode = "_spamton_";

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

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt(typeof(Localization), nameof(Localization.ToUpper))))
                return;

            crs.Emit(OpCodes.Ldarg, 4);
            crs.Emit(OpCodes.Call, sstc_mrb);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<TextDisplayer>(nameof(TextDisplayer.PlayVoiceSound))))
				return;

            crs.Emit(OpCodes.Ldarg, 4);
			crs.Emit(OpCodes.Call, sstc_s);
		}

        public static string SwitchSpamtonTextColors_MaybeReplaceBrackets(string curr, Speaker speaker)
        {
            // Very sloppy workaround but it works
            if (speaker != SpamtonSetup.spamtonSpeaker)
                return curr;

            return curr.Replace('{', '[').Replace('}', ']');
        }

		public static void SwitchSpamtonTextColors_Switch(Speaker speaker)
        {
			if (speaker != SpamtonSetup.spamtonSpeaker)
				return;

			if (SneoStyle is not TextDisplayer.SpeakerTextStyle sneoStyle)
				return;

            sneoStyle.color = (sneoTextShouldBePink = !sneoTextShouldBePink) ? glassPink : glassYellow;
        }

        [HarmonyPatch(typeof(DialogueParser), nameof(DialogueParser.ParseDialogueCodes))]
        [HarmonyILManipulator]
        public static void InsertSpamtonBrackets_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchStloc(4)))
                return;

            crs.Emit(OpCodes.Ldloc_3);
            crs.Emit(OpCodes.Call, isb_di);
        }

        public static string InsertSpamtonBrackets_DoInsert(string curr, string dialogueCode)
        {
            if (!dialogueCode.StartsWith($"[{SpamtonDialogueCode}"))
                return curr;

            dialogueCode = dialogueCode.Replace("[", "").Replace("]", "").Substring(SpamtonDialogueCode.Length);
            var commaIdx = dialogueCode.IndexOf(',');

            if (commaIdx < 0)
                return curr;

            var msg = dialogueCode.Substring(commaIdx + 1);
            var extraBracketsStr = dialogueCode.Substring(0, commaIdx);

            if (!int.TryParse(extraBracketsStr, out var extraBrackets))
                return curr;

            if (AscensionSaveData.Data.ChallengeIsActive(Plugin.ArchivedChallenge))
                msg = "CONFIDENTIAL";

            for (var i = 0; i < extraBrackets + 1; i++)
                msg = $"{{{msg}}}";

            return msg;
        }
    }
}
