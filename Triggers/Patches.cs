using InscryptionAPI.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Triggers
{
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(Part1Opponent), nameof(Part1Opponent.ModifyQueuedCard))]
        [HarmonyPatch(typeof(Part1Opponent), nameof(Part1Opponent.ModifySpawnedCard))]
        [HarmonyPostfix]
        public static void TriggerOnOpponentCard(PlayableCard card)
        {
            if(card == null)
                return;

            CustomTriggerFinder.CallAll<IOnModifyOpponentCard>(false, x => x.RespondsToModifyOpponentCard(card), x => x.OnModifyOpponentCard(card));
        }
    }
}
