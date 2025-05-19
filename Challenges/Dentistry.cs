using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    [HarmonyPatch]
    public static class Dentistry
    {
        public static MethodInfo sa_tb_s = AccessTools.Method(typeof(Dentistry), nameof(ShowActivation_TrinketBearer_Show));

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

        [HarmonyPatch(typeof(RandomConsumable), nameof(RandomConsumable.OnResolveOnBoard), MethodType.Enumerator)]
        [HarmonyILManipulator]
        public static void ShowActivation_TrinketBearer_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);
            
            if(!crs.JumpToNext(x => x.MatchBge(out _)))
                return;
            
            crs.Emit(OpCodes.Call, sa_tb_s);
        }

        public static void ShowActivation_TrinketBearer_Show()
        {
            if (!AscensionSaveData.Data.ChallengeIsActive(Plugin.DentistryChallenge))
                return;

            ChallengeActivationUI.TryShowActivation(Plugin.DentistryChallenge);
        }
    }
}
