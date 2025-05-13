using DiskCardGame;
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace SquirrelBombMod.Spamton
{
    [HarmonyPatch]
    public class SpamtonTextDisplayer
    {
		private static TextDisplayer.SpeakerTextStyle _sneostyle;
		public static bool sneoTextShouldBePink;
		public static Color glassYellow = new Color32(255, 242, 0, 255);
		public static Color glassPink = new Color32(255, 174, 201, 255);
		public static TextDisplayer.SpeakerTextStyle SneoStyle
        {
            get
            {
				if(_sneostyle != null)
                {
					if (TextDisplayer.Instance != null && TextDisplayer.Instance.alternateSpeakerStyles != null && !TextDisplayer.Instance.alternateSpeakerStyles.Contains(_sneostyle))
					{
						TextDisplayer.Instance.alternateSpeakerStyles.Add(_sneostyle);
					}
					return _sneostyle;
                }
				sneoTextShouldBePink = false;
				if(TextDisplayer.Instance != null && TextDisplayer.Instance.alternateSpeakerStyles != null)
                {
					_sneostyle = TextDisplayer.Instance.alternateSpeakerStyles.Find(x => x.speaker == SpamtonSetup.spamtonSpeaker);
					var piratestyle = TextDisplayer.Instance.alternateSpeakerStyles.Find(x => x.speaker == DialogueEvent.Speaker.PirateSkull);
					if (_sneostyle == null && piratestyle != null)
                    {
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
					}
					return _sneostyle;
				}
				return null;
            }
        }

		[HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowMessage))]
        [HarmonyPrefix]
        public static bool HandleSpamtonTalking(ref string __result, TextDisplayer __instance, string message, Emotion emotion = Emotion.Neutral, 
			TextDisplayer.LetterAnimation letterAnimation = TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker speaker = DialogueEvent.Speaker.Single, string[] variableStrings = null)
        {
            if (speaker == SpamtonSetup.spamtonSpeaker)
            {
				var sneostyle = SneoStyle;
				string text = Localization.Translate(message);
				text = Localization.ToUpper(text);
				TextDisplayer.SpeakerTextStyle speakerTextStyle = __instance.alternateSpeakerStyles.Find((TextDisplayer.SpeakerTextStyle x) => x.speaker == speaker);
				if (speakerTextStyle != null)
				{
					__instance.SetTextStyle(speakerTextStyle);
				}
				else
				{
					__instance.SetTextStyle(__instance.defaultStyle);
				}
				__instance.Show();
				if ((speaker == DialogueEvent.Speaker.Leshy || speaker == DialogueEvent.Speaker.Single) && LeshyAnimationController.Instance != null)
				{
					LeshyAnimationController.Instance.SetEyesAnimated(true);
				}
				__instance.textMesh.text = text;
				__instance.textAnimation.StopAllAnimations(true);
				__instance.textAnimation.PlayAnimation((int)letterAnimation);
				__instance.PlayVoiceSound(emotion);
				sneoTextShouldBePink = !sneoTextShouldBePink;
				sneostyle.color = sneoTextShouldBePink ? glassPink : glassYellow;
				__result = text;
				return false;
			}
			return true;
        }

		[HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowUntilInput))]
		[HarmonyPostfix]
		public static IEnumerator SpamtonizeUntilInput(IEnumerator result, TextDisplayer __instance, string message, float effectFOVOffset = -2.5f, float effectEyelidIntensity = 0.5f, Emotion emotion = Emotion.Neutral, 
			TextDisplayer.LetterAnimation letterAnimation = TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker speaker = DialogueEvent.Speaker.Single, string[] variableStrings = null, bool obeyTimescale = true)
		{
			if(speaker == SpamtonSetup.spamtonSpeaker)
            {
				string transformedMessage = Localization.Translate(message);
				if (!string.IsNullOrEmpty(transformedMessage))
				{
					__instance.CurrentAdvanceMode = TextDisplayer.MessageAdvanceMode.Input;
					Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
					Singleton<ViewManager>.Instance.OffsetFOV(effectFOVOffset, 0.15f, obeyTimescale);
					Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(effectEyelidIntensity, 0.15f);
					if (obeyTimescale)
					{
						yield return new WaitForSeconds(0.15f);
					}
					else
					{
						yield return new WaitForSecondsRealtime(0.15f);
					}
					__instance.triangleAnim.updateMode = (obeyTimescale ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.UnscaledTime);
					__instance.triangleAnim.ResetTrigger("clear");
					__instance.triangleAnim.Play("idle", 0, 0f);
					__instance.ShowMessage(transformedMessage, emotion, letterAnimation, speaker, variableStrings);
					if (obeyTimescale)
					{
						yield return new WaitForSeconds(0.2f);
					}
					else
					{
						yield return new WaitForSecondsRealtime(0.2f);
					}
					__instance.continuePressed = false;
					while (!__instance.continuePressed)
					{
						yield return new WaitForFixedUpdate();
					}
					__instance.Clear();
					__instance.triangleAnim.SetTrigger("clear");
					if (obeyTimescale)
					{
						yield return new WaitForSeconds(0.05f);
					}
					else
					{
						yield return new WaitForSecondsRealtime(0.05f);
					}
					Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
					Singleton<ViewManager>.Instance.OffsetFOV(0f, 0.15f, true);
					Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0f, 0.15f);
				}
			}
            else
            {
				yield return result;
            }
			yield break;
        }
    }
}
