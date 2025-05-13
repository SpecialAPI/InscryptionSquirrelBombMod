using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities
{
    [HarmonyPatch]
    public class TimeWaster : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return true;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            if (Card.OpponentCard)
            {
                SkipNextPlayerTurn();
            }
            else
            {
                TurnManager.Instance.Opponent.SkipNextTurn = true;
            }
            yield break;
        }

        [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.PlayerTurn))]
        [HarmonyPostfix]
        public static IEnumerator MaybeSkipTurn(IEnumerator rat, TurnManager __instance)
        {
            if (DoSkipPlayerTurn())
            {
                if (Singleton<PlayerHand>.Instance != null)
                {
                    Singleton<PlayerHand>.Instance.PlayingLocked = true;
                }
                __instance.IsPlayerTurn = true;
                __instance.PlayerPhase = TurnManager.PlayerTurnPhase.End;
                Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
                __instance.playerInitiatedCombat = false;
                yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("PlayerSkipTurn", TextDisplayer.MessageAdvanceMode.Auto, TextDisplayer.EventIntersectMode.Wait, null, null);
                UnskipNextPlayerTurn();
            }
            else
            {
                yield return rat;
            }
            yield break;
        }
    }
}
