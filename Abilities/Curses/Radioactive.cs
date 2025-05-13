using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class Radioactive : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return playerUpkeep != Card.OpponentCard;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            yield return Explode(Card.slot, 1, false);
            yield break;
        }
    }
}
