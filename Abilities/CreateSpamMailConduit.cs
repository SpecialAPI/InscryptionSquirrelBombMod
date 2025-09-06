using DiskCardGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities
{
    public class CreateSpamMailConduit : ConduitSpawn
    {
        public override Ability Ability => FindFromContext();

        public override string GetSpawnCardId()
        {
            return AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode) ? $"{PREFIX}_BearMail" : $"{PREFIX}_SpamMail";
        }
    }
}
