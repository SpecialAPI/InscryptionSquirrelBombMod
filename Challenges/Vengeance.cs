using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Vengeance : ChallengeBehaviour
    {
        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            return card != null && card.OpponentCard && killer != null && killer.OnBoard;
        }

        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            ShowActivation();
            yield return killer.TakeDamage(card.Attack, null);
        }
    }
}
