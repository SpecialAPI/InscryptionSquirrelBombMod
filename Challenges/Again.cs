using InscryptionAPI.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class Again : ChallengeBehaviour, ISetupAttackSequence, IOnPreSlotAttackSequence
    {
        public List<CardSlot> CollectModifyAttackSlots(PlayableCard card, OpposingSlotTriggerPriority modType, List<CardSlot> originalSlots, List<CardSlot> currentSlots, ref int attackCount, ref bool didRemoveDefaultSlot)
        {
            attackCount *= 2;
            return [..currentSlots, ..currentSlots];
        }

        public int GetTriggerPriority(PlayableCard card, OpposingSlotTriggerPriority modType, List<CardSlot> originalSlots, List<CardSlot> currentSlots, int attackCount, bool didRemoveDefaultSlot)
        {
            return 88888;
        }

        public bool RespondsToModifyAttackSlots(PlayableCard card, OpposingSlotTriggerPriority modType, List<CardSlot> originalSlots, List<CardSlot> currentSlots, int attackCount, bool didRemoveDefaultSlot)
        {
            return modType == OpposingSlotTriggerPriority.PostAdditionModification && card.OpponentCard;
        }

        public bool RespondsToPreSlotAttackSequence(CardSlot attackingSlot)
        {
            return attackingSlot?.Card != null && attackingSlot.Card.OpponentCard;
        }

        public IEnumerator OnPreSlotAttackSequence(CardSlot attackingSlot)
        {
            ShowActivation();
            yield break;
        }
    }
}
