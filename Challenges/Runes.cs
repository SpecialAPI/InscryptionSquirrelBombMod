using SquirrelBombMod.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Runes : ChallengeBehaviour, IOnModifyOpponentCard
    {
        public void OnModifyOpponentCard(PlayableCard card)
        {
            if (GlobalTriggerHandler.Instance)
                GlobalTriggerHandler.Instance.NumTriggersThisBattle++; // inscryption api Call() doesnt increment triggers

            var learnedAbs = AbilitiesUtil.GetLearnedAbilities(true, 1, 3);
            learnedAbs.RemoveAll(x => card.HasAbility(x) && AbilitiesUtil.GetInfo(x) is AbilityInfo abInfo && !abInfo.canStack);

            if (learnedAbs.Count <= 0)
                return;

            card.AddTemporaryMod(new(learnedAbs.RandomElement(GetRandomSeed() * 10))
            {
                fromCardMerge = true
            });
            ShowActivation();
        }

        public bool RespondsToModifyOpponentCard(PlayableCard card)
        {
            if (TurnManager.Instance == null)
                return false;

            var opponent = TurnManager.Instance.opponent;

            if(opponent == null || opponent.Blueprint == null || opponent.Blueprint.dominantTribes is not List<Tribe> dominantTribes)
                return false;

            if(dominantTribes.Count <= 0 || !card.IsOfTribe(dominantTribes[0]))
                return false;

            return true;
        }
    }
}
