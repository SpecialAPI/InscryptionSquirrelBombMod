using DiskCardGame;
using InscryptionAPI.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities.Curses
{
    public class Broken : AbilityBehaviour, IModifyDamageTaken
    {
        public override Ability Ability => FindFromContext();

        public bool RespondsToCardTakenDamageModifier(PlayableCard card, int currentValue)
        {
            return card == Card;
        }

        public int CollectCardTakenDamageModifier(PlayableCard card, int currentValue)
        {
            return currentValue + 1;
        }

        public bool RespondsToModifyDamageTaken(PlayableCard target, int damage, PlayableCard attacker, int originalDamage)
        {
            return target == Card;
        }

        public int OnModifyDamageTaken(PlayableCard target, int damage, PlayableCard attacker, int originalDamage)
        {
            return damage + 1;
        }

        public int TriggerPriority(PlayableCard target, int damage, PlayableCard attacker)
        {
            return 157;
        }
    }
}
