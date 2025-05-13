using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Triggers
{
    public interface IOnModifyOpponentCard
    {
        public bool RespondsToModifyOpponentCard(PlayableCard card);
        public void OnModifyOpponentCard(PlayableCard card);
    }
}
