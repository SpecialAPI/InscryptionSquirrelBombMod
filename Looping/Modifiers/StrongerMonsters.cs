using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class StrongerMonsters : BoonBehaviour
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
            otherCard.AddTemporaryMod(new(1, 2));
            yield break;
        }

        public bool playedAnim;
    }
}
