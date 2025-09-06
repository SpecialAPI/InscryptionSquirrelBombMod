using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Remorse : ChallengeBehaviour
    {
        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            return card != null && killer != null && killer.OnBoard;
        }

        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            ShowActivation();
            killer.AddTemporaryMod(new()
            {
                attackAdjustment = -card.Attack
            });
            killer.Anim.StrongNegationEffect();

            yield break;
        }
    }
}
