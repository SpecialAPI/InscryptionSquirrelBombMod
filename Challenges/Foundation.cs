using DiskCardGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Foundation : ChallengeBehaviour
    {
        public const string ModInfoId = "foundation_challenge";

        public override bool RespondsToPreBattleSetup()
        {
            return true;
        }

        public override IEnumerator OnPreBattleSetup()
        {
            ShowActivation();
            yield break;
        }

        public CardModificationInfo MakeCardMod(PlayableCard card)
        {
            var mod = new CardModificationInfo() { singletonId = ModInfoId };

            if (card.Attack > 0)
                mod.attackAdjustment = -1;
            if (card.IsFromSideDeck())
                mod.abilities.Add(Ability.BuffNeighbours);

            return mod;
        }

        public bool CardIsAlreadyAffected(PlayableCard card)
        {
            return card.TemporaryMods.Exists(x => x.singletonId == ModInfoId);
        }

        public override bool RespondsToOtherCardDrawn(PlayableCard card)
        {
            return !CardIsAlreadyAffected(card);
        }

        public override IEnumerator OnOtherCardDrawn(PlayableCard card)
        {
            card.AddTemporaryMod(MakeCardMod(card));

            yield break;
        }

        public override bool RespondsToOtherCardResolve(PlayableCard card)
        {
            return !card.OpponentCard && !CardIsAlreadyAffected(card);
        }

        public override IEnumerator OnOtherCardResolve(PlayableCard card)
        {
            card.AddTemporaryMod(MakeCardMod(card));

            yield break;
        }
    }
}
