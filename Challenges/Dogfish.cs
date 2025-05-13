using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Dogfish : ChallengeBehaviour
    {
        public override bool RespondsToPostBattleSetup()
        {
            return BoardManager.Instance.OpponentSlotsCopy.FindAll((x) => x.Card == null).Count > 0;
        }

        public override IEnumerator OnPostBattleSetup()
        {
            List<CardSlot> emptySlots = BoardManager.Instance.OpponentSlotsCopy.FindAll((x) => x.Card == null);
            CardSlot randomSlot = emptySlots[SeededRandom.Range(0, emptySlots.Count, GetRandomSeed())];
            ShowActivation();
            var card = CardLoader.GetCardByName("BaitBucket");
            card.Mods.Add(new(Ability.GuardDog));
            yield return BoardManager.Instance.CreateCardInSlot(card, randomSlot, 0.5f, true);
            baitCard = randomSlot?.Card;
            yield break;
        }
        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            return card != null && baitCard != null && card == baitCard && (deathSlot.Card == null || deathSlot.Card.Dead);
        }

        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            yield return new WaitForSeconds(0.2f);
            CardInfo shark = CardLoader.GetCardByName("Shark");
            ShowActivation();
            yield return Singleton<BoardManager>.Instance.CreateCardInSlot(shark, deathSlot, 0.1f, true);
            yield return new WaitForSeconds(0.25f);
            yield return new WaitForSeconds(0.1f);
            yield break;
        }

        public PlayableCard baitCard;
    }
}
