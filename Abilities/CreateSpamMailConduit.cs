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
            return AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode) ? "morebosses_BearMail" : "morebosses_SpamMail";
        }
    }
}
