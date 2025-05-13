using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Challenges
{
    public class DanceParty : ChallengeBehaviour
    {
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return playerUpkeep && TurnManager.Instance.TurnNumber > 1;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            ViewManager.Instance.SwitchToView(View.Board, false, false);
            yield return new WaitForSeconds(0.25f);
            List<CardSlot> slots = BoardManager.Instance.PlayerSlotsCopy;
            ShowActivation();
            if (!moveLeft)
            {
                slots.Reverse();
            }
            foreach (CardSlot slot in slots)
            {
                if(slot?.Card != null)
                {
                    CardSlot target = BoardManager.Instance.GetAdjacent(slot, moveLeft);
                    if(target != null && (target.Card == null || target.Card.Dead))
                    {
                        yield return BoardManager.Instance.AssignCardToSlot(slot.Card, target, 0.1f, null, true);
                    }
                    else
                    {
                        yield return slot.Card.Die(false, null, true);
                        yield return TextDisplayer.Instance.PlayDialogueEvent("DancePartyDie", TextDisplayer.MessageAdvanceMode.Input);
                    }
                }
            }
            moveLeft = !moveLeft;
            yield break;
        }

        public bool moveLeft;
    }
}
