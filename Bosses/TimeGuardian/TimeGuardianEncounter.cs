using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Bosses.TimeGuardian
{
    public class TimeGuardianEncounter
    {
        public static EncounterBlueprintData loopfinalBlueprint = new()
        {

            dominantTribes = new() { Tribe.Squirrel },
            turns = new()
            {
                new()
                {
                    BuildCardBlueprint("EnigmaStaff")
                },
                new()
                {
                    BuildCardBlueprint("TimeSnail"),
                    BuildCardBlueprint("TimeSnail")
                },
                new()
                {
                    BuildCardBlueprint("LostInTime")
                },
                new()
                {
                    BuildCardBlueprint("EnigmaStaff"),
                    BuildCardBlueprint("EnigmaStaff")
                },
                new()
                {
                    BuildCardBlueprint("LostInTime"),
                    BuildCardBlueprint("TimeSnail")
                },
                new()
                {
                    BuildCardBlueprint("EnigmaStaff"),
                    BuildCardBlueprint("LostInTime")
                }
            }
        };

        public static EncounterBlueprintData loopfinalBlueprintP2 = new()
        {
            dominantTribes = new() { Tribe.Squirrel },
            turns = new()
            {
                new()
                {
                    BuildCardBlueprint("Squirrel")
                },
                new()
                {
                    BuildCardBlueprint("Shieldbot"),
                    BuildCardBlueprint("Bonehound")
                },
                new()
                {
                    BuildCardBlueprint("RoboSkeleton"),
                    BuildCardBlueprint("Skeleton"),
                    BuildCardBlueprint("SentryBot")
                },
                new()
                {
                    BuildCardBlueprint("BurrowingTrap"),
                    BuildCardBlueprint("Porcupine"),
                    BuildCardBlueprint("Mantis")
                },
                new()
                {
                    BuildCardBlueprint("SquirrelBall"),
                    BuildCardBlueprint("Mummy_Telegrapher"),
                    BuildCardBlueprint("Mole_Telegrapher"),
                },
                new()
                {
                    BuildCardBlueprint("Snelk"),
                    BuildCardBlueprint("Bee"),
                    BuildCardBlueprint("Sniper"),
                },
                new()
                {
                    BuildCardBlueprint("SentinelBlue"),
                    BuildCardBlueprint("MageKnight"),
                    BuildCardBlueprint("Pupil")
                },
                new()
                {
                    BuildCardBlueprint("MasterGoranj"),
                    BuildCardBlueprint("EmptyVessel_GreenGem"),
                    BuildCardBlueprint("MasterGoranj")
                },
                new()
                {
                    BuildCardBlueprint("Angler_Fish_Good"),
                    BuildCardBlueprint("Zombie"),
                    BuildCardBlueprint("Geck")
                }
            }
        };
    }
}
