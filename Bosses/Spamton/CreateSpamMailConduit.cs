using DiskCardGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Spamton
{
    public class CreateSpamMailConduit : ConduitSpawn
    {
        public override Ability Ability => ab;

        public override string GetSpawnCardId()
        {
            return AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode) ? "morebosses_BearMail" : "morebosses_SpamMail";
        }

        public static Ability ab;
    }
}
