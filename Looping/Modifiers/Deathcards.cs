using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class Deathcards : BoonBehaviour
    {
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return !triggered && GetEmptyQueueSlots().Count > 0;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            (string, DeathCardInfo) a;
            int seed = GetRandomSeed() * 20;
            if (SeededRandom.Range(0, 10, seed++) == 0)
            {
                a = specialDeathcards.RandomElement(seed++);
                seed += 4;
            }
            else
            {
                a = (names.RandomElement(seed++), new((CompositeFigurine.FigurineType)SeededRandom.Range(0, (int)CompositeFigurine.FigurineType.NUM_FIGURINES, seed++), SeededRandom.Range(0, 6, seed++), SeededRandom.Range(0, 6, seed++))
                {
                    lostEye = SeededRandom.Range(0, 5, seed++) == 0
                });
            }
            CardModificationInfo cmi = CreateRandomizedAbilitiesStatsModSeeded(seed++, AbilitiesUtil.AllData.FindAll(x => x.metaCategories.Contains(AbilityMetaCategory.Part1Modular)), 10, 1, 2);
            cmi.nameReplacement = a.Item1;
            cmi.deathCardInfo = a.Item2;
            cmi.bloodCostAdjustment = 2;
            CardInfo dcb = TryGetCard("!DEATHCARD_BASE");
            dcb.Mods.Add(cmi);
            yield return TurnManager.Instance.Opponent.QueueCard(dcb, GetEmptyQueueSlots().RandomElement(seed++));
            triggered = true;
            yield break;
        }

        public bool triggered;

        public static readonly string[] names = new string[]
        {
            "Bob",
            "Gregory",
            "Rose",
            "Mary",
            "Johnson",
            "Daniel",
            "Amelia",
            "Barry"
        };

        public static readonly (string, DeathCardInfo)[] specialDeathcards = new (string, DeathCardInfo)[]
        {
            ("API", new(CompositeFigurine.FigurineType.Robot, 0, 0)
            {
                lostEye = false
            }),
            ("AMY", new(CompositeFigurine.FigurineType.Enchantress, 4, 0)
            {
                lostEye = true
            }),
            ("SLIMEMAN", new(CompositeFigurine.FigurineType.SettlerMan, 5, 2)
            {
                lostEye = false
            }),
            ("JURY", new(CompositeFigurine.FigurineType.SettlerMan, 5, 4)
            {
                lostEye = true
            }),
            ("LILY", new(CompositeFigurine.FigurineType.Prospector, 2, 4)
            {
                lostEye = false
            }),
            ("EASY", new(CompositeFigurine.FigurineType.Enchantress, 1, 0)
            {
                lostEye = false
            }),
            ("KEX", new(CompositeFigurine.FigurineType.Chief, 1, 2)
            {
                lostEye = false
            }),
            ("POTATO", new(CompositeFigurine.FigurineType.Wildling, 4, 2)
            {
                lostEye = false
            }),
            ("CACTUS", new(CompositeFigurine.FigurineType.Wildling, 3, 5)
            {
                lostEye = false
            }),
            ("CACTUS", new(CompositeFigurine.FigurineType.Prospector, 5, 3)
            {
                lostEye = false
            }),
            ("THAT GUY WHO GETS SHOT IN THE HEAD AT THE END", new(CompositeFigurine.FigurineType.SettlerMan, 2, 0)
            {
                lostEye = true
            }),
            ("FROGGY", new(CompositeFigurine.FigurineType.Chief, 3, 1)
            {
                lostEye = false
            })
        };
    }
}
