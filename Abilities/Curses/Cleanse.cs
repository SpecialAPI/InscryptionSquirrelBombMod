using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class Cleanse : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public override bool RespondsToResolveOnBoard()
        {
            return Card.Info.Abilities.Exists(x => AbilitiesUtil.GetInfo(x) != null && AbilitiesUtil.GetInfo(x).powerLevel >= 0);
        }

        public override IEnumerator OnResolveOnBoard()
        {
            Card.Status.hiddenAbilities.Add(Ability);
            Card.AddTemporaryMod(new()
            {
                negateAbilities = new()
                {
                    Card.Info.Abilities.FindAll(x => AbilitiesUtil.GetInfo(x) != null && AbilitiesUtil.GetInfo(x).powerLevel >= 0).RandomElement(GetRandomSeed())
                }
            });
            yield break;
        }
    }
}
