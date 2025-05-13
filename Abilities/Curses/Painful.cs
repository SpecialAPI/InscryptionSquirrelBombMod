using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class Painful : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public override bool RespondsToPlayFromHand()
        {
            return true;
        }

        public override IEnumerator OnPlayFromHand()
        {
            yield return DealDamageNoKill(1, !Card.OpponentCard);
            yield break;
        }
    }
}
