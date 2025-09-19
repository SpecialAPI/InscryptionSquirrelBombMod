using InscryptionAPI.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities
{
    public class ReverseAnnoying : AbilityBehaviour, IOnCardPassiveAttackBuffs
    {
        public override Ability Ability => FindFromContext();

        public int CollectCardPassiveAttackBuffs(PlayableCard card, int currentValue)
        {
            if (card.Slot == null || card.Slot.opposingSlot == null)
                return currentValue;

            var opposingCard = card.Slot.opposingSlot.Card;

            if(opposingCard == null || opposingCard.Dead)
                return currentValue;

            return currentValue + 1;
        }

        public bool RespondsToCardPassiveAttackBuffs(PlayableCard card, int currentValue)
        {
            return Card == card;
        }
    }
}
