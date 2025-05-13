using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Abilities
{
    public class Test : AbilityBehaviour
    {
        public override Ability Ability => FindRegisteredAbility("test");

        public override bool RespondsToResolveOnBoard()
        {
            return BoardManager.Instance.AllSlotsCopy.Exists(x => x.Card != null && x.Card != this);
        }

        public override IEnumerator OnResolveOnBoard()
        {
            CardSlot cs = null;
            yield return BoardManager.Instance.ChooseTarget(BoardManager.Instance.AllSlotsCopy, BoardManager.Instance.AllSlotsCopy.FindAll(x => x.Card != null && x.Card != this),
                x => cs = x, null, null, null, CursorType.Target);
            if(cs?.Card != null)
            {
				cs.Card.Anim.StrongNegationEffect();
				cs.Card.AddTemporaryMod(new()
				{
					abilities = new()
					{
						AbilitiesUtil.GetRandomLearnedAbility(GetRandomSeed(), cs.Card.OpponentCard),
						AbilitiesUtil.GetRandomLearnedAbility(GetRandomSeed() + 1, cs.Card.OpponentCard)
					}
				});
            }
            yield break;
        }

		public static CardInfo CreateCloneCard(CardInfo cardToCopy, int randomSeed)
		{
			CardInfo cardByName = TryGetCard(cardToCopy.name);
			foreach (CardModificationInfo cardModificationInfo in cardToCopy.Mods.FindAll((CardModificationInfo x) => !x.nonCopyable))
			{
				if (cardModificationInfo.singletonId != "paint_decal")
				{
					CardModificationInfo item = (CardModificationInfo)cardModificationInfo.Clone();
					cardByName.Mods.Add(item);
				}
			}
			CardModificationInfo cardModificationInfo2 = new();
			cardModificationInfo2.singletonId = "paint_decal";
			CopyCardSequencer.paintDecalIndex++;
			if (CopyCardSequencer.paintDecalIndex > 2)
			{
				CopyCardSequencer.paintDecalIndex = 0;
			}
			cardModificationInfo2.DecalIds.Add("decal_paint_" + (CopyCardSequencer.paintDecalIndex + 1));
			cardByName.Mods.Add(cardModificationInfo2);
			CardModificationInfo cardModificationInfo3 = new();
			int currentRandomSeed = randomSeed;
			float num = SeededRandom.Value(currentRandomSeed++);
            if (num < 0.33f)
            {
                if (cardByName.Mods.Exists((CardModificationInfo x) => x.abilities.Count > 0))
                {
                    List<CardModificationInfo> list = cardByName.Mods.FindAll((CardModificationInfo x) => x.abilities.Count > 0);
                    list[SeededRandom.Range(0, list.Count, currentRandomSeed++)].abilities[0] = AbilitiesUtil.GetRandomLearnedAbility(currentRandomSeed++, false, 0, 5, AbilityMetaCategory.Part1Modular);
                }
            }
            else
			{
				if (num < 0.66f && cardByName.Attack > 0)
				{
					cardModificationInfo3.attackAdjustment = (SeededRandom.Bool(currentRandomSeed++) ? 1 : -1);
				}
				else if (cardByName.Health > 1)
				{
					int num2 = Mathf.Min(2, cardByName.Health - 1);
					cardModificationInfo3.healthAdjustment = (SeededRandom.Bool(currentRandomSeed++) ? 2 : (-num2));
				}
			}
			cardByName.Mods.Add(cardModificationInfo3);
			return cardByName;
		}
    }
}
