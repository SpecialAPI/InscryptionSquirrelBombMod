using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class AnnoyingCards : BoonBehaviour
    {
        public override IEnumerator OnOtherCardDrawn(PlayableCard card)
        {
            card?.AddTemporaryMod(new(Ability.BuffEnemy));
            yield break;
        }

        public override bool RespondsToOtherCardDrawn(PlayableCard card)
        {
            return true;
        }
    }
}
