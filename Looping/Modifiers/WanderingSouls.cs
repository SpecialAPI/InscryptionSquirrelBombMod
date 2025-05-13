using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class WanderingSouls : BoonBehaviour
    {
        public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
        {
            return otherCard.OpponentCard;
        }

        public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
        {
            if (!playedAnim)
            {
                yield return PlayBoonAnimation();
                playedAnim = true;
            }
            otherCard.AddTemporaryMod(new(AbilitiesUtil.GetRandomLearnedAbility(GetRandomSeed(), true, 1, 3)));
            yield break;
        }

        public bool playedAnim;
    }
}
