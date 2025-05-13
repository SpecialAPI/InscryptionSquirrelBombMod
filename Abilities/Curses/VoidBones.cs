using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class VoidBones : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return true;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            yield return Card.OpponentCard ? ResourcesManager.Instance?.AddBones(3, Card.Slot) : ResourcesManager.Instance?.SpendBones(3);
            yield break;
        }
    }
}
