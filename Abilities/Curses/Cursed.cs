using DiskCardGame;
using InscryptionAPI.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class Cursed : AbilityBehaviour, IOnAddedToHand
    {
        public override Ability Ability => FindFromContext();

        public IEnumerator OnAddedToHand()
		{
			Card.Status.hiddenAbilities.Add(Ability);
			CardModificationInfo cardModificationInfo = 
				new(AbilitiesUtil.AllData.FindAll(x => x.metaCategories.Contains(CurseMetacategory) && x.opponentUsable == Card.OpponentCard && x.ability != Ability).RandomElement(GetRandomSeed()).ability);
			CardModificationInfo cardModificationInfo2 = Card.TemporaryMods.Find(x => x.HasAbility(Ability));
			if (cardModificationInfo2 == null)
			{
				cardModificationInfo2 = Card.Info.Mods.Find(x => x.HasAbility(Ability));
			}
			if (cardModificationInfo2 != null)
			{
				cardModificationInfo.fromTotem = cardModificationInfo2.fromTotem;
				cardModificationInfo.fromCardMerge = cardModificationInfo2.fromCardMerge;
			}
			Card.AddTemporaryMod(cardModificationInfo);
			yield break;
        }

        public bool RespondsToAddedToHand()
        {
            return true;
        }
    }
}
