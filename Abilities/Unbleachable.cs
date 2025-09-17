using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities
{
    [HarmonyPatch]
    public static class Unbleachable
    {
        [HarmonyPatch(typeof(BleachPotItem), nameof(BleachPotItem.CardHasNoAbilities))]
        [HarmonyPostfix]
        public static void MakeCardUnbleachable_Postfix(ref bool __result, PlayableCard card)
        {
            if (card.HasAbility(FindRegisteredAbility("Unbleachable")))
                __result = true;
        }
    }
}
