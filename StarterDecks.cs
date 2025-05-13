using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod
{
    public partial class Plugin
    {
        public static void AddStarterDecks()
        {
            StarterDeckManager.New(GUID, "Caged Potential", LoadTexture("starterdeck_icon_cage"), new string[]
            {
                "RingWorm",
                "CagedWolf",
                "Tadpole"
            }, 0);
            StarterDeckManager.New(GUID, "Ultimate Challenge", LoadTexture("starterdeck_icon_bell"), new string[]
            {
                "DausBell",
                "DausBell",
                "DausBell"
            }, 0);
            StarterDeckManager.New(GUID, "Sus", LoadTexture("starterdeck_icon_sus"), new string[]
            {
                "Ijiraq",
                "Mole",
                "Skink"
            }, 0);
        }
    }
}
