using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Abilities
{
    public class Unbounded : AbilityBehaviour
    {
        public override Ability Ability => FindFromContext();

        public readonly float repeatChance = 0.95f;
        public readonly int cycles = 1;
        public const string ModSingletonId = "runforth";

        public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
        {
            return attacker == Card;
        }

        public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
        {
            var mod = Card.TemporaryMods.Find(x => x.singletonId == ModSingletonId);
            if(mod == null)
            {
                Card.AddTemporaryMod(mod = new()
                {
                    singletonId = ModSingletonId,
                });
            }

            var target = UnboundedRandomNumber(repeatChance, cycles, GetRandomSeed() * cycles);
            var cardAttackMinusMod = Card.Attack - mod.attackAdjustment;
            mod.attackAdjustment = target - cardAttackMinusMod;
            Card.OnStatsChanged();
            yield break;
        }
    }
}
