using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod
{
    public partial class Plugin
    {
        //public static Ability[] fakeClockAbilities = new Ability[12];

        public static void AddAbilities()
        {
            //actual abilities
            /*NewAbility("Time Waster", "When [creature] perishes, the opponent will be forced to skip the next turn.", typeof(TimeWaster), "ability_timewaster").SetPart1Rulebook();
            NewAbility("Fast Forward", "When [creature] perishes, progress the time cycle.", typeof(FastForward), "ability_fastforward").SetPart1Rulebook();

            //fake clock abilities
            var namesAndDescriptions = new (string, string)[]
            {
                ("I",   "When a card bearing this sigil is cast, all creatures on the same side get +1 attack."),
                ("II",  "When a card bearing this sigil is cast, all creatures on the same side get +2 health."),
                ("III", "When a card bearing this sigil is cast, the opponent will be dealt 3 damage"),
                ("IV",  "When a card bearing this sigil is cast, all of the cards on the opposite side will be dealt 1 damage"),
                ("V",   "When a card bearing this sigil is cast, drawing side deck on the next turn will deal 5 damage to the opponent"),
                ("VI",  "When a card bearing this sigil is cast, heal the user for 6 damage."),
                ("VII", "When a card bearing this sigil is cast, a random card with the cost tier of 7 will be played."),
                ("IIX", "When a card bearing this sigil is cast, deal 8 damage to a random card on the opposite side. Overkill damage will be dealt to other cards."),
                ("IX",  "When a card bearing this sigil is cast, use an item with the power of 4. If this is not the first phase, use an item with the power of 5 instead."),
                ("X",   "When a card bearing this sigil is cast, the opponent will skip the next draw phase."),
                ("XI",  "When a card bearing this sigil is cast, all of the cards queued on this turn will get +1 attack, +1 health and a random sigil"),
                ("XII", "When a card bearing this sigil is cast, reset the board to its initial state.")
            };

            for (int i = 0; i < 12; i++)
            {
                var s = namesAndDescriptions[i];
                var ability = NewAbility(s.Item1, s.Item2, typeof(AbilityBehaviour), "ability_clock" + (i + 1)).SetPart1Rulebook();
                ability.passive = true;
                fakeClockAbilities[i] = ability.ability;
            }*/

            //curses
            /* //when i get around making the art for these
            NewAbility("Painful", "[creature] will deal 1 damage to its owner when played.", typeof(Painful), "ability_painful")
                .SetCurse(2);

            NewAbility("Burst", "When [creature] perishes, it will deal 10 damage to adjacent creatures.", typeof(Burst), "ability_burst")
                .SetCurse(2);

            NewAbility("Soulbound", "When [creature] takes damage, it will 1 damage to its owner.", typeof(Soulbound), "ability_soulbound")
                .SetCurse(3);

            NewAbility("Void", "When [creature] perishes, it will take away 3 bones", typeof(VoidBones), "ability_void")
                .SetCurse(2);

            NewAbility("Broken", "[creature] will take 1 more damage.", typeof(Broken), "ability_broken")
                .SetCurse(2);

            NewAbility("Inaccurate", "[creature] can miss its target, attacking adjacent slots instead.", typeof(Inaccurate), "ability_inaccurate")
                .SetCurse(2);

            NewAbility("Cursed!", "When [creature] is drawn, this sigil will be replaced by a random curse.", typeof(Cursed), "ability_cursed")
                .SetCurse(2)
                .SetOpponentUnusable();

            NewAbility("Cleanse", "When [creature] is played, this sigil as well as another random one will be removed.", typeof(Cleanse), "ability_cleanse")
                .SetCurse(3);

            NewAbility("Radioactive", "At the start of its owner's turn, [creature] will deal 1 damage to adjacent cards", typeof(Radioactive), "ability_radioactive")
                .SetCurse(2);
            */
        }
    }
}
