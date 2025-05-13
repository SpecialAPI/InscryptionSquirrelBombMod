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

            card.AddTemporaryMod(new(AbilitiesUtil.GetRandomLearnedAbility(GetRandomSeed() * 10, true, 1, 3))
            {
                fromCardMerge = true
            });
            ShowActivation();
        }

        public bool RespondsToModifyOpponentCard(PlayableCard card)
        {
            return true;
        }
    }
}
