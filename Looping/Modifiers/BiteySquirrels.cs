using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class BiteySquirrels : BoonBehaviour
    {
        public override bool RespondsToOtherCardDrawn(PlayableCard card)
        {
            return card.IsFromSideDeck() && !firstSideDeckChosen;
        }

        public override IEnumerator OnOtherCardDrawn(PlayableCard card)
        {
            firstSideDeckCard = card;
            firstSideDeckChosen = true;
            yield break;
        }

        public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
        {
            return otherCard == firstSideDeckCard && !otherCard.OpponentCard;
        }

        public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
        {
            yield return PlayBoonAnimation();
            yield return DealDamageNoKill(2, true);
            yield break;
        }

        public bool firstSideDeckChosen;
        public PlayableCard firstSideDeckCard;
    }
}
