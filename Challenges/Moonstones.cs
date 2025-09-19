using SquirrelBombMod.Abilities;
using SquirrelBombMod.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Moonstones : ChallengeBehaviour, IOnModifyOpponentCard
    {
        public void OnModifyOpponentCard(PlayableCard card)
        {
            ShowActivation();
            card.AddTemporaryMod(new()
            {
                abilities = [FindRegisteredAbility<Unbounded>(), FindRegisteredAbility<ReverseAnnoying>()]
            });
        }

        public bool RespondsToModifyOpponentCard(PlayableCard card)
        {
            return card != null && card.HasTrait(Trait.Terrain) && card.Attack <= 0;
        }
    }
}
