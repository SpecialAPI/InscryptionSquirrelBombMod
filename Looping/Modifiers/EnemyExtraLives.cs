using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class EnemyExtraLives : BoonBehaviour
    {
        public override bool RespondsToPostBoonActivation()
        {
            return TurnManager.Instance?.Opponent != null && TurnManager.Instance.Opponent is not Part1BossOpponent && TurnManager.Instance.Opponent.StartingLives <= 1;
        }

        public override IEnumerator OnPostBoonActivation()
        {
            TurnManager.Instance.Opponent.NumLives++;
            yield break;
        }
    }
}
