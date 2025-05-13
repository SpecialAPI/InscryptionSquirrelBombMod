using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Spamton
{
    public class DieHeal : AbilityBehaviour
    {
        public override Ability Ability => ab;

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return playerTurnEnd != Card.OpponentCard && Card.OnBoard;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            foreach(CardSlot slot in BoardManager.Instance.GetAdjacentSlots(Card.Slot))
            {
                if(slot != null && slot.Card != null)
                {
                    if(slot.Card.Health < slot.Card.MaxHealth)
                    {
                        slot.Card.HealDamage(1);
                    }
                }
            }
            yield return Card.Die(false);
            yield break;
        }

        public static Ability ab;
    }
}
