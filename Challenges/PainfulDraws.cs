using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class PainfulDraws : ChallengeBehaviour
    {
        public override bool RespondsToOtherCardDrawn(PlayableCard card)
        {
            return card.IsFromMainDeck() && TurnManager.Instance != null && TurnManager.Instance.TurnNumber > 0;
        }

        public override IEnumerator OnOtherCardDrawn(PlayableCard card)
        {
            ShowActivation();
            yield return DealDamageNoKill(1, true);
            yield break;
        }
    }
}
