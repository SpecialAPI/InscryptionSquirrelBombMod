using DiskCardGame;
using SquirrelBombMod.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Sus : ChallengeBehaviour
    {
        public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
        {
            return otherCard != null && otherCard.HasTrait(Trait.Giant) && otherCard.OpponentCard;
        }

        public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
        {
            ShowActivation();
            otherCard.AddTemporaryMod(new()
            {
                abilities = [FindRegisteredAbility<Unbounded>()],
                fromTotem = true
            });
            yield break;
        }
    }
}
