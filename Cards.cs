using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod
{
    public partial class Plugin
    {
        public static void AddCards()
        {
            /*CardManager
                .New(PREFIX, "MoleEnigman", "Mole Enigman", 0, 8, "Everyone who climbs the tower will stare at themselves through a mirror, darkly.")
                .AddAbilities(Ability.Reach, Ability.WhackAMole, FindRegisteredAbility<TimeWaster>())
                .SetPortrait(LoadTexture("portrait_enigman"));

            CardManager
                .New(PREFIX, "EnigmaStaff", "Enigmatic Staff", 2, 2)
                .AddAbilities(Ability.Sharp)
                .SetPortrait(LoadTexture("portrait_enigmaticstaff"));

            CardManager
                .New(PREFIX, "LostInTime", "Lost in Time", 2, 1)
                .AddAbilities(Ability.GuardDog, Ability.Brittle, Ability.DoubleStrike)
                .SetPortrait(LoadTexture("portrait_lostintime"));

            CardManager
                .New(PREFIX, "TimeSnail", "Time Snail", 1, 3)
                .AddAbilities(FindRegisteredAbility<FastForward>())
                .SetPortrait(LoadTexture("portrait_timesnail"));

            CardManager
                .New(PREFIX, "!CLOCK_POWER", "Power", 4, 1)
                .AddAbilities(Ability.Brittle, Ability.Flying)
                .SetPortrait(LoadTexture("portrait_power"));

            CardManager
                .New(PREFIX, "!CLOCK_HEALTH", "Longevity", 1, 4)
                .AddAbilities(Ability.WhackAMole, Ability.Reach)
                .SetPortrait(LoadTexture("portrait_longevity"));

            CardManager
                .New(PREFIX, "!CLOCK_SUFFERING", "Suffering", 0, 1)
                .AddAbilities(Ability.MoveBeside, Ability.Submerge)
                .AddTraits(Trait.Terrain)
                .AddAppearances(CardAppearanceBehaviour.Appearance.TerrainBackground)
                .SetPortrait(LoadTexture("portrait_suffering"))
                .hideAttackAndHealth = true;

            CardManager
                .New(PREFIX, "!CLOCK_ALLY", "Ally", 3, 3)
                .AddAbilities(Ability.GuardDog)
                .SetPortrait(LoadTexture("portrait_ally"));

            var names = new string[]
            {
                "I - Power",
                "II - Longevity",
                "III - Punishment",
                "IV - Suffering",
                "V - Choice",
                "VI - Restoration",
                "VII - Ally",
                "IIX - Destruction",
                "IX - Utility",
                "X - Overwhelm",
                "XI - Preparation",
                "XII - Full Reset"
            };

            for(int i = 0; i < 12; i++)
            {
                var val = i + 1;
                CardManager.New(PREFIX, $"!CLOCK_{val}", names[i], 0, 0, null)
                    .AddAbilities(fakeClockAbilities[i])
                    .SetPortrait(LoadSprite($"portrait_clock{val}"))
                    .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
                    .hideAttackAndHealth = true;
            }*/
        }
    }
}
