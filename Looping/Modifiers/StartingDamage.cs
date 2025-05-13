using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class StartingDamage : BoonBehaviour
    {
        public override bool RespondsToPostBoonActivation()
        {
            return true;
        }

        public override IEnumerator OnPostBoonActivation()
        {
            yield return PlayBoonAnimation();
            yield return DealDamageNoKill(1, true);
            yield break;
        }
    }
}
