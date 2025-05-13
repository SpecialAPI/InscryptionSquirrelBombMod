using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Bosses.TimeGuardian
{
    public class TimeGuardianBossOpponent : Part1BossOpponent
    {
		public override string DefeatedPlayerDialogue => "";

        public override IEnumerator IntroSequence(EncounterData encounter)
        {
			RunState.CurrentMapRegion.FadeOutAmbientAudio();
			yield return ReducePlayerLivesSequence();
			yield return new WaitForSeconds(0.25f);
			Singleton<ViewManager>.Instance.SwitchToView(View.BossSkull, false, true);
			yield return new WaitForSeconds(0.5f);
			GameObject gameObject = Instantiate(ResourceBank.Get<GameObject>("Prefabs/CardBattle/" + BossSkullPrefabId), transform);
			bossSkull = gameObject.GetComponent<BossSkull>();
			yield return new WaitForSeconds(0.166f);
			SetSceneEffectsShown(true);
			yield return new WaitForSeconds(0.5f);
			GlitchModelDeactivate(bossSkull.transform);
			yield return new WaitForSeconds(0.75f);
			if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.BossTotems))
			{
				yield return new WaitForSeconds(0.25f);
				ChallengeActivationUI.TryShowActivation(AscensionChallenge.BossTotems);
				int difficulty = Mathf.Min(encounter.Difficulty, 15);
				TotemItemData totemData = EncounterBuilder.BuildOpponentTotem(encounter.Blueprint.dominantTribes[0], difficulty, encounter.Blueprint.redundantAbilities);
				yield return AssembleTotem(totemData, new Vector3(1f, 0f, -1f), new Vector3(0f, 20f, 0f), InteractablesGlowColor, false);
				yield return new WaitForSeconds(0.5f);
				GlitchModelDeactivate(totem.transform);
				yield return new WaitForSeconds(0.75f);
				Singleton<OpponentAnimationController>.Instance.ClearLookTarget();
			}
			Singleton<ViewManager>.Instance.SwitchToView(View.Candles, false, true);
			yield return new WaitForSeconds(1f);
			GlitchModelDeactivate(CandleHolder.Instance.transform);
			yield return new WaitForSeconds(0.75f);
			Singleton<ViewManager>.Instance.SwitchToView(View.BossCloseup, false, true);
			yield return new WaitForSeconds(1f);
			GlitchModelDeactivate(LeshyAnimationController.Instance.transform);
			yield return new WaitForSeconds(0.75f);
			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, true);
			yield return new WaitForSeconds(1f);
			AudioController.Instance.PlaySound2D("giant_head_rising", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
			yield return new WaitForSeconds(2f);
			AudioController.Instance.SetLoopAndPlay("20 - Final Encounter", 0, true, true);
			AudioController.Instance.SetLoopVolumeImmediate(0.75f, 0);
			yield break;
        }
    }
}
