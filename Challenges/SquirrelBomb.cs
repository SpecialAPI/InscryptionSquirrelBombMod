using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class SquirrelBomb : ChallengeBehaviour
    {
        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            return card.IsFromSideDeck() && card.IsPlayerCard();
        }
        
        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            ShowActivation();
            yield return Explode(deathSlot, bombOpposing: false);
            yield break;
        }
    }
}
