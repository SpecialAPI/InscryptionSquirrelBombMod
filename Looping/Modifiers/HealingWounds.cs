using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class HealingWounds : BoonBehaviour
    {
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return !playerUpkeep && LifeManager.Instance.OpponentDamage > 0;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            yield return PlayBoonAnimation();
            LifeManager.Instance.OpponentDamage -= 1;
            if(LifeManager.Instance.Scales3D != null)
            {
                LifeManager.Instance.Scales3D.opponentWeight -= 1;
                if(LifeManager.Instance.Scales3D.opponentWeights.Count > 0)
                {
                    LifeManager.Instance.Scales3D.DestroyWeight(LifeManager.Instance.Scales3D.opponentWeights[0]);
                    LifeManager.Instance.Scales3D.opponentWeights.RemoveAt(0);
                }
                LifeManager.Instance.Scales3D.SetBalanceBasedOnWeights();
            }
            yield break;
        }
    }
}
