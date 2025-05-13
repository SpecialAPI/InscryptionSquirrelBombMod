using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Challenges
{
    public class AnnoyingTotems : ChallengeBehaviour
    {
        public override bool RespondsToPreBattleSetup()
        {
            return DeckHasAnyBasegameTribes();
        }

        public override IEnumerator OnPreBattleSetup()
        {
            Tribe dominantTribe = GetRandomMostDominantBasegameTribe();
            if (dominantTribe == Tribe.None || dominantTribe == Tribe.NUM_TRIBES)
            {
                yield break;
            }
            ShowActivation();
            GameObject totemObj = Instantiate(ResourceBank.Get<GameObject>("Prefabs/Items/CompositeTotem"), TurnManager.Instance.opponent.transform);
            totem = totemObj.GetComponent<Totem>();
            totem.transform.position += new Vector3(-1.75f, 0f, 4f);
            totem.transform.localScale *= 1.45f;
            totem.transform.eulerAngles += new Vector3(0f, -20f, 0f);
            yield return new WaitForSeconds(0.15f);
            yield return new WaitForSeconds(0.3f);
            totem.SetData(new TotemItemData() { top = new TotemTopData(dominantTribe), bottom = new TotemBottomData(Ability.BuffEnemy) });
            totem.Anim.Play("slow_assemble", 0, 0f);
            yield return new WaitForSeconds(0.166f);
            Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.1f);
            yield return new WaitForSeconds(0.166f);
            Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.1f);
            yield return new WaitForSeconds(1.418f);
            Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.2f);
            AudioController.Instance.PlaySound2D("metal_object_up#2", MixerGroup.TableObjectsSFX, 1f, 0.25f, null, null, null, null, false);
            if (!DialogueEventsData.EventIsPlayed("LeshyAnnoyingTotem"))
            {
                yield return new WaitForSeconds(0.25f);
                yield return TextDisplayer.Instance.PlayDialogueEvent("LeshyAnnoyingTotem", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            }
            yield break;
        }

        public override bool RespondsToPreBattleCleanup()
        {
            return totem != null;
        }

        public override IEnumerator OnPreBattleCleanup()
        {
            ShowActivation();
            totem.Anim.Play("slow_disassemble", 0, 0f);
            yield return new WaitForSeconds(0.333f);
            Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.1f);
            totem.ShowHighlighted(false, true);
            Destroy(totem.gameObject, 0.7f);
            AudioController.Instance.PlaySound2D("metal_object_up#2", MixerGroup.TableObjectsSFX, 1f, 0.25f, null, null, null, null, false);
            yield break;
        }

        public static Tribe GetRandomMostDominantBasegameTribe(int? randomSeed = null)
        {
            List<Tribe> dominantTribes = GetMostDominantBasegameTribes();
            if (dominantTribes.Count <= 0)
            {
                return Tribe.None;
            }
            else if (dominantTribes.Count == 1)
            {
                return dominantTribes[0];
            }
            else
            {
                return dominantTribes[SeededRandom.Range(0, dominantTribes.Count, randomSeed ?? SaveManager.SaveFile.GetCurrentRandomSeed())];
            }
        }

        public static List<Tribe> GetMostDominantBasegameTribes()
        {
            if (!DeckHasAnyBasegameTribes())
            {
                return new List<Tribe>();
            }
            List<Tribe> dominantTribes = new();
            int mostUses = GetMostTribeUses();
            dominantTribes.AddRange(GetTribesAndUsesForCards(RunState.DeckList).ToList().FindAll((x) => x.Key != Tribe.None && x.Key != Tribe.NUM_TRIBES && x.Value >= mostUses).ConvertAll((x) => x.Key));
            return dominantTribes;
        }

        public static bool DeckHasAnyBasegameTribes()
        {
            return GetMostTribeUses() > 0;
        }

        public static int GetMostTribeUses()
        {
            return Mathf.Max(GetTribesAndUsesForCards(RunState.DeckList).ToList().FindAll((x) => x.Key != Tribe.None && x.Key != Tribe.NUM_TRIBES).ConvertAll((x) => x.Value).ToArray());
        }

        public static Dictionary<Tribe, int> GetTribesAndUsesForCards(List<CardInfo> cards)
        {
            Dictionary<Tribe, int> tribesAndUses = new();
            foreach (Tribe tribe in Enum.GetValues(typeof(Tribe)))
            {
                if (tribe == Tribe.NUM_TRIBES)
                {
                    continue;
                }
                int uses = 0;
                if (tribe == Tribe.None)
                {
                    foreach (CardInfo card in cards)
                    {
                        if (card != null && card.tribes.Count <= 0)
                        {
                            uses++;
                        }
                    }
                    tribesAndUses.Add(tribe, uses);
                    continue;
                }
                foreach (CardInfo card in cards)
                {
                    if (card != null && card.IsOfTribe(tribe))
                    {
                        uses++;
                    }
                }
                tribesAndUses.Add(tribe, uses);
            }
            return tribesAndUses;
        }

        public Totem totem;
    }
}
