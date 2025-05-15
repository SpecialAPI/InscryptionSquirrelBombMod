using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    [HarmonyPatch]
    public static class Dentistry //TODO: Make Trinket Bearer show challenge activation
    {
        public const string PliersId = "Pliers";

        [HarmonyPatch(typeof(ItemsUtil), nameof(ItemsUtil.GetUnlockedConsumables))]
        [HarmonyPrefix]
        public static bool OverrideConsumables_Unlocked_Prefix(ref List<ConsumableItemData> __result)
        {
            if (!AscensionSaveData.Data.ChallengeIsActive(Plugin.DentistryChallenge))
                return true;

            __result = [ItemsUtil.GetConsumableByName(PliersId)];
            return false;
        }

        [HarmonyPatch(typeof(GainConsumablesSequencer), nameof(GainConsumablesSequencer.GenerateItemChoices))]
        [HarmonyPrefix]
        public static bool OverrideConsumables_ItemNode_Prefix(ref List<ConsumableItemData> __result)
        {
            if (!AscensionSaveData.Data.ChallengeIsActive(Plugin.DentistryChallenge))
                return true;

            ChallengeActivationUI.TryShowActivation(Plugin.DentistryChallenge);

            var pliers = ItemsUtil.GetConsumableByName(PliersId);
            __result = [pliers, pliers, pliers];
            return false;
        }
    }
}
