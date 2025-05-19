using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    [HarmonyPatch]
    public static class Archived
    {
        public static MethodInfo acc_ccs_pac = AccessTools.Method(typeof(Archived), nameof(ArchiveCardChoices_CardChoicesSequencer_PickArchivedCards));
        public static MethodInfo acc_ccs_macc = AccessTools.Method(typeof(Archived), nameof(ArchiveCardChoices_CardChoicesSequencer_MaybeArchiveCurrentCard));

        public static Texture2D archivedCardBack = LoadTexture("archived");

        [HarmonyPatch(typeof(CardSingleChoicesSequencer), nameof(CardSingleChoicesSequencer.CardSelectionSequence), MethodType.Enumerator)]
        [HarmonyILManipulator]
        public static void ArchiveCardChoices_CardChoicesSequencer_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<CardChoicesSequencer>(nameof(CardChoicesSequencer.SpawnCards))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, acc_ccs_pac);

            if(!crs.JumpToNext(x => x.MatchStloc(6)))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, acc_ccs_macc);
        }

        public static List<SelectableCard> ArchiveCardChoices_CardChoicesSequencer_PickArchivedCards(List<SelectableCard> cards, IEnumerator enumerator)
        {
            if(cards.Count <= 0 || !AscensionSaveData.Data.ChallengeIsActive(Plugin.ArchivedChallenge))
                return cards;

            var cardsToArchive = AscensionSaveData.Data.GetNumChallengesOfTypeActive(Plugin.ArchivedChallenge);
            var cardsCopy = cards.ToList();
            var seed = enumerator.EnumeratorGetField<int>("randomSeed");

            for (var i = 0; i < cardsToArchive && cardsCopy.Count > 0; i++)
            {
                var idx = SeededRandom.Range(0, cardsCopy.Count, seed++);
                var card = cardsCopy[idx];
                cardsCopy.RemoveAt(idx);

                card.AddComponent<ArchiveSelectedCard>();
            }

            enumerator.EnumeratorSetField("randomSeed", seed);

            return cards;
        }

        public static void ArchiveCardChoices_CardChoicesSequencer_MaybeArchiveCurrentCard(IEnumerator enumerator)
        {
            var card = enumerator.EnumeratorGetField<object>("1").EnumeratorGetField<SelectableCard>("card");

            if (!card.GetComponent<ArchiveSelectedCard>())
                return;

            card.Flipped = true;
            card.SetFaceDown(true, true);
            card.flippedBackTexture = archivedCardBack;
        }
    }

    public class ArchiveSelectedCard : MonoBehaviour
    {
    }
}
