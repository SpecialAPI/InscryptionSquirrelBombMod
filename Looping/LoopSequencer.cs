using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Looping
{
    [HarmonyPatch]
    public class LoopSequencer : CustomNodeSequencer
    {
        public const string LOOP_TIER_KEY = "LoopTier";
        public const string HIGHEST_LOOP_KEY = "HighestLoopTier";
        public const int LOOP_RUN_DIFFICULTY = 19;
        public const int LOOP_MIN_CARDS = 4;
        public static readonly Texture2D loopmodBG = LoadTexture("boon_loopmod");

        public static readonly BoonData.Type Weights = BoonManager.New<StartingDamage>(GUID, 
            "Weights",
            "Start the battle with an additional weight at your start of the scales.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type StrongerMonsters = BoonManager.New<StrongerMonsters>(GUID,
            "Stronger Monsters",
            "All enemy cards get +1/+2 when played.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type WanderingSouls = BoonManager.New<WanderingSouls>(GUID,
            "Wandering Souls",
            "All enemy cards get a random sigil when played.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type AnnoyingCards = BoonManager.New<AnnoyingCards>(GUID,
            "So Annoying!",
            "All your cards get the Annoying sigil when drawn.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type BiteySquirrels = BoonManager.New<BiteySquirrels>(GUID,
            "Bitey Squirrels",
            "The first squirrel you draw will deal you 2 damage when played.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type CrowdedHand = BoonManager.New<CrowdedHand>(GUID,
            "Crowded Hand",
            "All cards that are drawn when there are 5 or more cards in hand will instantly die.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type Deathcards = BoonManager.New<Deathcards>(GUID,
            "Shadows of the Past",
            "The opponent will play an above average strength deathcard on the first turn.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type OpponentExtraLife = BoonManager.New<EnemyExtraLives>(GUID,
            "Resilient Foes",
            "Non-boss opponents will get 1 extra life.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type HealingWounds = BoonManager.New<HealingWounds>(GUID,
            "Healing Wounds",
            "Opponents recover 1 life each turn.",
            LoadTexture("boonicon_scales"), loopmodBG, true, false);

        public static readonly BoonData.Type[][] LoopModifiers = new BoonData.Type[][]
        {
            new BoonData.Type[]
            {
                Weights,
                StrongerMonsters
            },
            new BoonData.Type[]
            {
                WanderingSouls,
                BiteySquirrels
            },
            new BoonData.Type[]
            {
                Deathcards,
                AnnoyingCards,
                CrowdedHand
            },
            new BoonData.Type[]
            {
                OpponentExtraLife,
                HealingWounds,
                StrongerMonsters
            },
            new BoonData.Type[]
            {
                OpponentExtraLife,
                Deathcards,
                Weights,
                HealingWounds
            },
        };

        public override void Inherit(CustomSpecialNodeData node)
        {
            base.Inherit(node);
            bigassBell = Instantiate(bellPrefab, transform);
            bigassBell.name = "LoopBell";
            bigassBell.transform.localPosition = Vector3.zero;
            bigassBell.transform.localScale = Vector3.one * 0.5f;
            transform.localPosition = new(0.125f, 8f, 0f);
            Destroy(bigassBell.GetComponentInChildren<CombatBell>());
            bigassBell.SetActive(false);
        }

        public override IEnumerator DoCustomSequence(CustomSpecialNodeData node)
        {
            ViewManager.Instance.SwitchToView(View.Default, false, true);
            yield return new WaitForSeconds(0.25f);
            bigassBell.SetActive(true);
            bigassBell.GetComponentInChildren<Animator>().Play("move towards", 0, 0f);
            yield return new WaitForSeconds(1.25f);
            bool shouldIntroduce = !DialogueEventsData.EventIsPlayed("LoopNodeIntroduction");
            if (shouldIntroduce)
            {
                yield return TextDisplayer.Instance.ShowUntilInput("You came across an old bell.");
                yield return new WaitForSeconds(0.25f);
            }
            Coroutine bellRingCR = CustomCoroutine.Instance.StartCoroutine(HandleBellRinging());
            yield return new WaitForSeconds(0.75f);
            if (shouldIntroduce)
            {
                yield return TextDisplayer.Instance.PlayDialogueEvent("LoopNodeIntroduction", TextDisplayer.MessageAdvanceMode.Input);
            }
            float transitionDuration = 4f;
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(Color.black);
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, transitionDuration);
            yield return new WaitForSeconds(transitionDuration);
            DoLoopEffects();
            Singleton<ExplorableAreaManager>.Instance.TweenHangingLightColors(RunState.CurrentMapRegion.boardLightColor, RunState.CurrentMapRegion.cardsLightColor, 0.25f);
            yield return new WaitForSeconds(0.25f);
            CustomCoroutine.Instance.StopCoroutine(bellRingCR);
            bigassBell.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            transitionDuration = 0.5f;
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(0f, transitionDuration);
            yield return new WaitForSeconds(transitionDuration);
            yield return new WaitForSeconds(0.5f);
            if (shouldIntroduce)
            {
                yield return TextDisplayer.Instance.PlayDialogueEvent("LoopNodePostLoop", TextDisplayer.MessageAdvanceMode.Input);
                yield return new WaitForSeconds(0.25f);
            }
            RunState.CurrentMapRegion.FadeInAmbientAudio();
            yield break;
        }

        public void DoLoopEffects()
        {
            RunState.Run.regionTier = 0;
            RunState.Run.GenerateMapDataForCurrentRegion(null);
            List<int> source = new()
            {
                0,
                1,
                2
            };
            if (LoopTier < LoopModifiers.Length)
            {
                var modsToAdd = LoopModifiers[LoopTier];
                foreach (var m in modsToAdd)
                {
                    RunState.Run.playerDeck.AddBoon(m);
                }
            }
            RemoveBadCards();
            if (RunState.Run.playerDeck.Cards.Count < LOOP_MIN_CARDS)
            {
                int cardsToGive = LOOP_MIN_CARDS - RunState.Run.playerDeck.Cards.Count;
                int randomseed = SaveManager.SaveFile.GetCurrentRandomSeed();
                for (int i = 0; i < cardsToGive; i++)
                {
                    RunState.Run.playerDeck.AddCard(CardLoader.GetRandomChoosableCard(randomseed++));
                }
            }
            RunState.Run.survivorsDead = false;
            RunState.Run.trapperKnifeBought = false;
            RunState.Run.regionOrder = (from a in source.AsEnumerable()
                                        orderby Random.Range(0, 100)
                                        select a).ToArray();
            LoopTier += 1;
            HighestLoop = LoopTier;
            RunState.Run.currentNodeId = RunState.Run.map.RootNode.id;
            PaperGameMap.Instance?.DataReader?.DestroyScenery();
            RunState.Run.currentNodeId = RunState.Run.map.RootNode.id;
            SaveManager.SaveToFile();
            CustomCoroutine.WaitOnConditionThenExecute(() => !Singleton<GameFlowManager>.Instance.Transitioning, delegate
            {
                RunState.Run.currentNodeId = RunState.Run.map.RootNode.id;
                CustomCoroutine.WaitThenExecute(0f, () =>
                {
                    RunState.Run.currentNodeId = RunState.Run.map.RootNode.id;
                });
            });
        }

        public IEnumerator HandleBellRinging()
        {
            while (true)
            {
                bigassBell.GetComponentInChildren<Animator>().Play("ring", 0, 0f);
                yield return new WaitForSeconds(0.75f);
            }
        }

        public static void RemoveBadCards()
        {
            RunState.Run.playerDeck.Cards.ToList().ForEach(x =>
            {
                if (CardIsUnloyal(x))
                {
                    RunState.Run.playerDeck.RemoveCard(x);
                }
            });
        }

        public static bool CardIsUnloyal(CardInfo c)
        {
            return !c.IsRareCard() && !c.Mods.Exists(x => x.fromCardMerge || x.fromDuplicateMerge || x.attackAdjustment > 0 || x.attackAdjustment > 0);
        }

        public static int LoopTier
        {
            get
            {
                return ModdedSaveManager.RunState.GetValueAsInt(GUID, LOOP_TIER_KEY);
            }
            set
            {
                ModdedSaveManager.RunState.SetValueAsObject(GUID, LOOP_TIER_KEY, value);
            }
        }

        public static int HighestLoop
        {
            get
            {
                return ModdedSaveManager.SaveData.GetValueAsInt(GUID, HIGHEST_LOOP_KEY);
            }
            set
            {
                if (value > HighestLoop)
                {
                    ModdedSaveManager.SaveData.SetValueAsObject(GUID, HIGHEST_LOOP_KEY, value);
                }
            }
        }

        public static bool IsLoopRun => LoopTier > 0;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RunState), nameof(RunState.DifficultyModifier), MethodType.Getter)]
        public static void FixDifficulty(ref int __result)
        {
            if (IsLoopRun)
            {
                int gridy = 0;
                try
                {
                    gridy = (MapNodeManager.Instance?.CurrentGridY).GetValueOrDefault();
                }
                catch { }
                __result = LOOP_RUN_DIFFICULTY - (RunState.Run.regionTier * 6 + (gridy + 1) / 3 - 1);
            }
        }

        public GameObject bigassBell;
    }
}
