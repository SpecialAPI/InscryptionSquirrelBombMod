using DiskCardGame;
using InscryptionAPI.Triggers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Looping.Modifiers
{
    public class CrowdedHand : BoonBehaviour, IOnOtherCardAddedToHand
    {
        public bool RespondsToOtherCardAddedToHand(PlayableCard card)
        {
            return card != null && PlayerHand.Instance.CardsInHand.Count > 5;
        }

        public IEnumerator OnOtherCardAddedToHand(PlayableCard card)
        {
            yield return new WaitForSeconds(0.5f);
            yield return PlayBoonAnimation();
            card?.Anim?.PlayDeathAnimation();
            PlayerHand.Instance?.CardsInHand?.Remove(card);
            yield return new WaitForSeconds(0.5f);
            card?.StartCoroutine(card?.DestroyWhenStackIsClear());
            yield break;
        }
    }
}
