using DiskCardGame;
using InscryptionAPI.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class Inaccurate : AbilityBehaviour, ISetupAttackSequence
    {
        public override Ability Ability => FindFromContext();

        public List<CardSlot> CollectModifyAttackSlots(PlayableCard card, OpposingSlotTriggerPriority modType, List<CardSlot> originalSlots, List<CardSlot> currentSlots, ref int attackCount, ref bool didRemoveDefaultSlot)
        {
            int randomseed = GetRandomSeed() * 10;
            for(int i = 0; i < currentSlots.Count; i++)
            {
                var before = currentSlots[i];
                List<CardSlot> possibilities = new() { before };
                possibilities.AddRange(BoardManager.Instance.GetAdjacentSlots(before));
                currentSlots[i] = possibilities.RandomElement(randomseed++);
                didRemoveDefaultSlot |= currentSlots[i] != before;
            }
            return currentSlots;
        }

        public int GetTriggerPriority(PlayableCard card, OpposingSlotTriggerPriority modType, List<CardSlot> originalSlots, List<CardSlot> currentSlots, int attackCount, bool didRemoveDefaultSlot)
        {
            return 999999;
        }

        public bool RespondsToModifyAttackSlots(PlayableCard card, OpposingSlotTriggerPriority modType, List<CardSlot> originalSlots, List<CardSlot> currentSlots, int attackCount, bool didRemoveDefaultSlot)
        {
            return modType == OpposingSlotTriggerPriority.PostAdditionModification;
        }
    }
}
