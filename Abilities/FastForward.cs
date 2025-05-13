using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities
{
    public class FastForward : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return !wasSacrifice && Singleton<TimeGuardianBattleSequencer>.Instance != null;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            yield return Singleton<TimeGuardianBattleSequencer>.Instance?.NextTimeSpell();
            yield break;
        }
    }
}
