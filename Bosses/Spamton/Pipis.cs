using DiskCardGame;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Spamton
{
    public class Pipis : AbilityBehaviour
    {
        private readonly GameObject bombPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb");

        public override Ability Ability => ab;

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return !wasSacrifice && Card.OnBoard && Card.Slot.opposingSlot != null && BoardManager.Instance != null;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            List<CardSlot> slotsToAttack = new() { Card.Slot.opposingSlot };
            slotsToAttack.AddRange(BoardManager.Instance.GetAdjacentSlots(Card.Slot.opposingSlot));
            slotsToAttack.RemoveAll(x => x == null || x.Card == null);
            slotsToAttack.Sort((x, x2) => x.Index - x2.Index);
            foreach(CardSlot slot in slotsToAttack)
            {
                if(slot.Card != null)
                {
                    GameObject bomb = Instantiate(bombPrefab);
                    bomb.transform.position = Card.transform.position + Vector3.up * 0.1f;
                    Tween.Position(bomb.transform, slot.Card.transform.position + Vector3.up * 0.1f, 0.5f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
                    yield return new WaitForSeconds(0.5f);
                    slot.Card.Anim.PlayHitAnimation();
                    Destroy(bomb);
                    yield return slot.Card.TakeDamage(3, Card);
                }
            }
            yield break;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            yield return Card.Die(false, null, true);
            yield break;
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return playerTurnEnd == !Card.OpponentCard;
        }

        public static Ability ab;
    }
}
