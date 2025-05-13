using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class Soulbound : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public override bool RespondsToTakeDamage(PlayableCard source)
        {
            return true;
        }

        public override IEnumerator OnTakeDamage(PlayableCard source)
        {
            yield return DealDamageNoKill(1, !Card.OpponentCard);
            yield break;
        }
    }
}
