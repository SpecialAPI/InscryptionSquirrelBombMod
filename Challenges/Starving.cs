using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Starving : ChallengeBehaviour
    {
        public override bool RespondsToPreBattleSetup()
        {
            return true;
        }

        public override IEnumerator OnPreBattleSetup()
        {
            CardDrawPiles.Instance.turnsSinceExhausted = Mathf.Min(RunState.Run.regionTier, 1);
            yield break;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return playerUpkeep && ((TurnManager.Instance?.TurnNumber).GetValueOrDefault() > 1 || RunState.Run.regionTier >= 2);
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            ShowActivation();
            if (CardDrawPiles.Instance != null)
            {
                List<PlayableCard> previousCards = new List<PlayableCard>(BoardManager.Instance.OpponentSlotsCopy.FindAll((x) => x != null && x.Card != null).ConvertAll((x) => x.Card));
                yield return CardDrawPiles.Instance.ExhaustedSequence();
                List<PlayableCard> cardsNow = new List<PlayableCard>(BoardManager.Instance.OpponentSlotsCopy.FindAll((x) => x != null && x.Card != null).ConvertAll((x) => x.Card));
                cardsNow.RemoveAll((x) => previousCards.Contains(x));
                cardsNow.RemoveAll((x) => x.Info.name != "Starvation");
                cardsNow.RemoveAll((x) => x.HasAbility(Ability.Brittle));
                if (cardsNow.Count > 0)
                {
                    cardsNow[0].Anim.PlayTransformAnimation();
                    yield return new WaitForSeconds(0.5f);
                    cardsNow[0].AddTemporaryMod(new CardModificationInfo { abilities = new List<Ability> { Ability.Brittle }, fromTotem = true });
                }
            }
            yield break;
        }
    }
}
