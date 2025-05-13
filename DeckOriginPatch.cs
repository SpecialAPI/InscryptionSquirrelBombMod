using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SquirrelBombMod
{
    [HarmonyPatch]
    public static class DeckOriginPatch
    {
        public static MethodInfo mcfsd_aa = AccessTools.Method(typeof(DeckOriginPatch), nameof(MarkCardFromSideDeck_AddAction));
        public static MethodInfo mcfnd_aa = AccessTools.Method(typeof(DeckOriginPatch), nameof(MarkCardFromMainDeck_AddAction));

        [HarmonyPatch(typeof(CardDrawPiles3D), nameof(CardDrawPiles3D.DrawFromSidePile), MethodType.Enumerator)]
        [HarmonyILManipulator]
        public static void MarkCardFromSideDeck_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CardSpawner>(nameof(CardSpawner.SpawnCardToHand))))
                return;

            crs.Emit(OpCodes.Call, mcfsd_aa);
        }

        public static Action<PlayableCard> MarkCardFromSideDeck_AddAction(Action<PlayableCard> curr)
        {
            return curr + MarkCardFromSideDeck_Mark;
        }

        public static void MarkCardFromSideDeck_Mark(PlayableCard card)
        {
            card.AddComponent<CardFromSideDeck>();
        }

        [HarmonyPatch(typeof(CardDrawPiles), nameof(CardDrawPiles.DrawCardFromDeck), MethodType.Enumerator)]
        [HarmonyILManipulator]
        public static void MarkCardFromMainDeck_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<CardSpawner>(nameof(CardSpawner.SpawnCardToHand))))
                return;

            crs.Emit(OpCodes.Call, mcfnd_aa);
        }

        public static Action<PlayableCard> MarkCardFromMainDeck_AddAction(Action<PlayableCard> curr)
        {
            return curr + MarkCardFromMainDeck_Mark;
        }

        public static void MarkCardFromMainDeck_Mark(PlayableCard card)
        {
            card.AddComponent<CardFromMainDeck>();
        }
    }

    public class CardFromSideDeck : MonoBehaviour
    {
    }

    public class CardFromMainDeck : MonoBehaviour
    {
    }
}
