using SquirrelBombMod.Abilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod
{
    public partial class Plugin
    {
        public static void AddAbilities()
        {
            NewAbility("Pipis", "Pipis.", typeof(Pipis), "ability_pipis.png")
                .SetPart1Rulebook()
                .SetCanStack(true);

            NewAbility("Help", "At the end of the owner's turn [creature] will sacrifice itself to heal the adjacent cards by 1.", typeof(DieHeal), "ability_dieheal")
                .SetPart1Rulebook();

            NewAbility("Pass-through", "When [creature] is about to get attacked by a card with an attack higher than this card's health, this card perishes.", typeof(PassThrough), "ability_passthrough")
                .SetPart1Rulebook();

            NewAbility("Spam Call", "Empty spaces within a circuit completed by [creature] spawn Spam Mail at the end of the owner's turn.", typeof(CreateSpamMailConduit), "ability_spamcall")
                .SetConduit(true)
                .SetPart1Rulebook();

            NewAbility("NEO Strike", "[creature] will strike each opposing space that is occupied by a creature and additionally every side of the board occupied by a creature. It will strike directly if no creatures oppose it.", "NeoStrike", "ability_neostrike.png")
                .SetPassive(true)
                .SetFlipYIfOpponent(true)
                .SetPart1Rulebook();

            NewAbility("A Call for Help", "At the end of the owner's turn, [creature] will call for help.", "HelpCall", "ability_helpcall")
                .SetPassive(true)
                .SetPart1Rulebook();

            NewAbility("Run Forth", "Before [creature] attacks, its power will be set to at least 1.", typeof(RunForth), "ability_runforth")
                .SetPart1Rulebook();
        }
    }
}
