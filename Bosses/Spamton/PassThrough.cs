using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Spamton
{
    public class PassThrough : AbilityBehaviour
    {
        public override Ability Ability => ab;

        public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
        {
            return slot == Card.Slot && attacker != null && attacker.Attack > Card.Health;
        }

        public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
        {
            yield return Card.Die(false, null, true);
            yield break;
        }

        public static Ability ab;
    }
}
