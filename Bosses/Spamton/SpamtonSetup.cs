global using Speaker = DialogueEvent.Speaker;
using BepInEx;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Ascension;
using InscryptionAPI.Card;
using InscryptionAPI.Dialogue;
using InscryptionAPI.Encounters;
using InscryptionAPI.Guid;
using InscryptionAPI.Helpers;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SquirrelBombMod.Spamton
{
    [HarmonyPatch]
    public class SpamtonSetup
    {
        public static GameObject sneoPrefab;
        public static GameObject giantSpamtonPrefab;
        public static Sprite chainSprite;
        public static RegionData spamtonRegion;
        public static string spamtonSequencer;
        public static Opponent.Type spamtonBoss;
        public static EncounterBlueprintData spamP1;
        public static EncounterBlueprintData spamP2;
        public static EncounterBlueprintData spamP2Bear;
        public static Speaker spamtonSpeaker;

        public static void Awake()
        {
            sneoPrefab = assets.LoadAsset<GameObject>("SpamtonNEOAnimation");

            var chaintexture = LoadTexture("heartchain.png");
            chainSprite = Sprite.Create(chaintexture, new Rect(0f, 0f, chaintexture.width, chaintexture.height), new Vector2(0.5f, 0.5f));
            spamtonSpeaker = GuidManager.GetEnumValue<Speaker>(GUID, "SpamtonSpeaker");
            spamtonSequencer = SpecialSequenceManager.Add(GUID, "SpamtonSpecialSequencer", typeof(SpamtonSpecialSequencer)).Id;
            spamtonBoss = OpponentManager.Add(GUID, "SpamtonBossOpponent", spamtonSequencer, typeof(SpamtonBossOpponent)).Id;

            #region Final Boss Region

            spamtonRegion = ScriptableObject.CreateInstance<RegionData>();
            spamtonRegion.bosses = [spamtonBoss];
            spamtonRegion.dominantTribes = [Tribe.Insect];
            spamtonRegion.predefinedScenery = null;
            spamtonRegion.fillerScenery = [];
            spamtonRegion.scarceScenery = [];
            spamtonRegion.encounters = [];
            spamtonRegion.ambientLoopId = "cabin_ambience";
            spamtonRegion.silenceCabinAmbience = false;
            spamtonRegion.boardLightColor = new Color32(76, 45, 57, 255);
            spamtonRegion.bossPrepCondition = new();
            spamtonRegion.bossPrepEncounter = null;
            spamtonRegion.cardsLightColor = new Color32(76, 45, 57, 255);
            spamtonRegion.consumableItems = [];
            spamtonRegion.dustParticlesDisabled = false;
            spamtonRegion.fogAlpha = 0f;
            spamtonRegion.fogEnabled = false;
            spamtonRegion.fogProfile = null;
            spamtonRegion.likelyCards = [];
            spamtonRegion.mapAlbedo = RegionProgression.Instance.regions[0].mapAlbedo;
            spamtonRegion.mapEmission = RegionProgression.Instance.regions[0].mapEmission;
            spamtonRegion.mapEmissionColor = RegionProgression.Instance.regions[0].mapEmissionColor;
            spamtonRegion.mapParticlesPrefabs = [];
            spamtonRegion.terrainCards = [];

            var predefinedNodes = ScriptableObject.CreateInstance<PredefinedNodes>();
            predefinedNodes.nodeRows = new()
            {
                new()
                {
                    new NodeData()
                    {
                        position = new(0.5f, 0.4f)
                    }
                },
                new()
                {
                    new DuplicateMergeNodeData()
                    {
                        position = new(0.3f, 0.7f)
                    },
                    new CardMergeNodeData()
                    {
                        position = new(0.4f, 0.7f)
                    },
                    new CardStatBoostNodeData()
                    {
                        position = new(0.5f, 0.7f)
                    },
                    new CopyCardNodeData()
                    {
                        position = new(0.6f, 0.7f)
                    },
                    new DeckTrialNodeData()
                    {
                        position = new(0.7f, 0.7f)
                    }
                },
                new()
                {
                    new SpamtonBattleNodeData()
                    {
                        position = new(0.5f, 0.9f),
                        bossType = spamtonBoss,
                        specialBattleId = spamtonSequencer
                    }
                }
            };
            spamtonRegion.predefinedNodes = predefinedNodes;
            #endregion

            #region Dialogue
            DialogueManager.GenerateEvent(GUID, "SneoPreIntro", new()
            {
                "HOLY         [_spamton_1,CUNGADERO]!1"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoFinalIntro1", new()
            {
                "TIME FOR A [_spamton_0,Change of Scenery], AND THIS TIME YOU W0N'T [_spamton_1,Relaxing Getaway]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase1", new()
            {
                "LOOK AT ME [_spamton_1,Cowbo]! LOOK INTO MY [_spamton_0,Heads] AND [_spamton_1,REACH FOR THE SKY]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase2", new()
            {
                "COME ON [_spamton_0,FRIEND]! THIS [_spamton_1,Lakeside Cabin For Rent] NEEDS SOme 1 LIKE [_spamton_0,Yours Truly] TO REALLY [_spamton_1,Show 'em who's]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase3", new()
            {
                "ONE [_spamton_0,Lonely Heart] TO ANOTHER! I'VE GOT THE. IM GONNA HSOW IT OFF!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoHeartStolen", new()
            {
                "AWW LOOK AT [_spamton_0,this], SOME LITTLE [_spamton_0,phisher] STOLE MY HEART. I COULD [_spamton_0,WEEP].",
                "[_spamton_1,BUT WAIT THERE'S MORE]",
                "CHECK OUT OUR [_spamton_0,Consumer Friendly Return Policy]!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoHeartBroken", new()
            {
                "LOOK AT THIS [_spamton_1,Cyber]! You're break ing my  [_spamton_1,HEAR]!!!",
                "DONT YOU KNOW HOW MUCH THAT [_spamton_1,BURNS OH GOD IT BURNS]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase4", new()
            {
                "TIME FOR YOU TO [_spamton_1,Meet the Boys]!!!",
                "MY [_spamton_1,EGGS] ARE [_spamton_0,Quivering.]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase5", new()
            {
                "WAIT. WHAT'S THAT?!",
                "LOOKS LIKE YOU'RE GETTING A [_spamton_1,CALL TO THE VOID]!!!",
                "WHY DON'T YOU [_spamton_1,Take my hand]!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase6", new()
            {
                "COME [_spamton_0,Off]!!!  JUST LOOK AT ME! I'M THE [_spamton_1,BIG]!!!",
                "YOU'RE JUST A [_spamton_0,Underpaid Developer]!  AND I HAVE [_spamton_1,Just the thing you need] TO GET OVER IT.",
                "YOU AND ME. LET IN [_spamton_1,THE POWER OF NEO] AND WE'LL BE [_spamton_1,Prince of Bel-Air]!!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingPhase", new()
            {
                "GUESS THAT WASNT ENOUGH. WE COULD HAVE BEEN SO [_spamton_1,BIG]",
                "[_spamton_1,BIG SHOTS]",
                "[_spamton_1,BIG FRIENDS]",
                "[_spamton_1,BIG DATA]",
                "BUT OH WELL. GUESS THERE'S [_spamton_0,Only so much] [_spamton_1,BIG] TO GO AROUND!!!",
                "I'LL [_spamton_0,not] THINK OF YOU WHEN I'M [_spamton_1,Soaring To The Top]!!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEnding1Damage", new()
            {
                "WHAT, YOU THINK I'M JUST GONNA [_spamton_1,Stand there and take it!?]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingFireworksLong", new()
            {
                "JUST YOU, ME, AND MY [_spamton_1,Business Associates]. TIME TO SIT BACK AND WATCH THE FIREWORKS!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingFireworks", new()
            {
                "I LOVE A [_spamton_1,Light Show]"
            }, new()
            {
                new()
                {
                    "SIT BACK AND WATCH THE FIREWORKS!!"
                },
                new()
                {
                    "COULD YOU [_spamton_1,Died] A LITTLE FASTER!!"
                },
                new()
                {
                    "I LOVE A [_spamton_1,Light Show]"
                }
            }, DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver1", new()
            {
                "WHAT!? WHO ARE YOU TRYING TO CALL!?",
                "THE [_spamton_1,FUZZ]!? THE [_spamton_1,FIVE-O]s HAVE NOTHING ON NEO!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver2", new()
            {
                "[_spamton_1,Mommy, Daddy, Somebody help me!]",
                "SORRY [_spamton_0,Friend], BUT ITS  [_spamton_0,Just The Two Of Us]",
                "TIME TO [_spamton_1,Burn]",
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver3", new()
            {
                "PATHETIC!!! [_spamton_1,Crooning] FOR HELP AT THE BOTTOM OF A DUMPSTER!!!",
                "YOU'VE GOT NO FRIENDS ANYMORE.",
                "NOBODY'S COMING TO [_spamton_1,SAVE BIG AND WIN INCREDIBLE PRIZES]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver4", new()
            {
                "[_spamton_1,Goober]?",
                "YELPING FOR HELP AT A [_spamton_1,Little Slime]?",
                "SORRY, BUT I [_spamton_1,Obliterate dirt and grime better than any competitor's product, or your money back!]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver5", new()
            {
                "[_spamton_1,Magnificus]??",
                "WHAT IS A [_spamton_1,Magnifigus]??",
                "IS IT A TYPE OF [_spamton_1,Tetraodontidae is a family of marine and freshwater fish in the order Tetraodontiformes.]!?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver6", new()
            {
                "[_spamton_1,Leshy]!?",
                "PRETTY SURE [_spamton_1,Granny Clampett's Roadkill Stew] ALREADY FELT THE [_spamton_1,Smooth Taste of Cundagero]!!",
                "I DON'T EVEN KNOW IF HE'S [_spamton_1,Alive and Well]!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver7", new()
            {
                "[_spamton_1,Royal]!?",
                "THE ONLY [_spamton_0,Royal] HER3 IS THE [_spamton_1,Royal Wee]!!!",
                "NO [_spamton_1,KINGS, QUEENS, OR JACKS] TRUMP THE [_spamton_1,DEUCE] BUDDY!!",
                "NOW STOP YELLING AT A PLAYING CARD",
                "AND DIE"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver8", new()
            {
                "UH OH. LOOKS LIKE A [_spamton_1,Connection Issue]!!",
                "WHAT ARE YOU GOING TO DO WITHOUT YOUR [_spamton_1,Lifeline]?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver9", new()
            {
                "WAIT. DO YOU HEAR SOMETHING?",
                "THOUGHT I HEARD A [_spamton_1,Brand New Yacht] TAKING A SWING AROIUND THE [_spamton_1,BAijou] !",
                "MUST JUST BE [_spamton_1,All in my head]. TOUGH [_spamton_0,Tity]",
                "THIS BODY ISNT [_spamton_1,Waterfront Property]!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingAlmostDeath", new()
            {
                "SAY [_spamton_0,GOODBYE] [_spamton_0,TO YOUR FRIENDS AND FAMILY]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingDeath", new()
            {
                "THE [_spamton_0,POWER OF NEO] WAS TOO MUCH FOR YOU"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            #endregion

            #region Blueprint 1
            spamP1 = ScriptableObject.CreateInstance<EncounterBlueprintData>();
            spamP1.dominantTribes = [Tribe.Squirrel];
            spamP1.minDifficulty = 0;
            spamP1.maxDifficulty = 100;
            spamP1.turns = new()
            {
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                }
            };
            #endregion

            #region Blueprint 2
            spamP2 = ScriptableObject.CreateInstance<EncounterBlueprintData>();
            spamP2.dominantTribes = [Tribe.Squirrel];
            spamP2.minDifficulty = 0;
            spamP2.maxDifficulty = 100;
            spamP2.turns = new()
            {
                new()
                {
                    BuildModCardBlueprint("SpamMail"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("SpamMail"),
                    BuildModCardBlueprint("SpamMail"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("SpamMail"),
                    BuildModCardBlueprint("SpamMail"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("SpamMail"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                }
            };
            #endregion

            #region Blueprint 2 (Bear Edition)
            spamP2Bear = ScriptableObject.CreateInstance<EncounterBlueprintData>();
            spamP2Bear.dominantTribes = [Tribe.Squirrel];
            spamP2Bear.minDifficulty = 0;
            spamP2Bear.maxDifficulty = 100;
            spamP2Bear.turns = new()
            {
                new()
                {
                    BuildModCardBlueprint("BearMail"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("BearMail"),
                    BuildModCardBlueprint("BearMail"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("BearMail"),
                    BuildModCardBlueprint("BearMail"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("BearMail"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                },
                new()
                {
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead"),
                    BuildModCardBlueprint("FlyingHead")
                }
            };
            #endregion
        }

        [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.GetOpposingSlots))]
        [HarmonyPrefix]
        public static bool AddNeoStrike(PlayableCard __instance, ref List<CardSlot> __result)
        {
            if (__instance.HasAbility(FindRegisteredAbility("NeoStrike")))
            {
                __result = new List<CardSlot>();
                List<CardSlot> list2 = __instance.OpponentCard ? Singleton<BoardManager>.Instance.PlayerSlotsCopy : Singleton<BoardManager>.Instance.OpponentSlotsCopy;
                if (list2.Exists((CardSlot x) => x.Card != null && !__instance.CanAttackDirectly(x)))
                {
                    List<CardSlot> slotsWithCards = list2.FindAll((CardSlot x) => x.Card != null && !__instance.CanAttackDirectly(x));
                    if (slotsWithCards.Exists(x => x.Index <= 1))
                    {
                        __result.Add(list2[0]);
                        __result.Add(list2[1]);
                    }
                    if (slotsWithCards.Exists(x => x.Index > 1))
                    {
                        __result.Add(list2[2]);
                        __result.Add(list2[3]);
                    }
                    foreach (CardSlot cardSlot in list2)
                    {
                        if (cardSlot.Card != null && !__instance.CanAttackDirectly(cardSlot))
                        {
                            __result.Add(cardSlot);
                        }
                    }
                }
                else
                {
                    __result.Add(list2[1]);
                    __result.Add(list2[2]);
                }
                if (__instance.HasAbility(Ability.SplitStrike))
                {
                    ProgressionData.SetAbilityLearned(Ability.SplitStrike);
                    __result.Remove(__instance.Slot.opposingSlot);
                    __result.AddRange(Singleton<BoardManager>.Instance.GetAdjacentSlots(__instance.Slot.opposingSlot));
                }
                if (__instance.HasTriStrike())
                {
                    ProgressionData.SetAbilityLearned(Ability.TriStrike);
                    __result.AddRange(Singleton<BoardManager>.Instance.GetAdjacentSlots(__instance.Slot.opposingSlot));
                    if (!__result.Contains(__instance.Slot.opposingSlot))
                    {
                        __result.Add(__instance.Slot.opposingSlot);
                    }
                }
                if (__instance.HasAbility(Ability.DoubleStrike))
                {
                    ProgressionData.SetAbilityLearned(Ability.DoubleStrike);
                    __result.Add(__instance.slot.opposingSlot);
                }
                __result.Sort((CardSlot a, CardSlot b) => a.Index - b.Index);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GiantCardAnimationController), nameof(GiantCardAnimationController.PlayAttackAnimation))]
        [HarmonyPrefix]
        public static bool AddNeoAnimation(GiantCardAnimationController __instance, bool attackPlayer, CardSlot targetSlot)
        {
            if (__instance.PlayableCard.Info.name != $"{PREFIX}_!GIANTCARD_NEO")
                return true;

            var neo = Singleton<CardRenderCamera>.Instance.GetLiveRenderCamera(__instance.Card.StatsLayer as RenderLiveStatsLayer).GetComponentInChildren<NeoAnimatedPortrait>();
            neo.AttackSpot();

            var lineobj = new GameObject("Line");
            var line = lineobj.AddComponent<LineRenderer>();
            line.startColor = Color.white;
            line.endColor = Color.white;
            line.material = neo.GetGlowyMaterial();
            line.startWidth = 0;
            line.endWidth = 1.5f;
            line.widthMultiplier = 0f;

            var start = __instance.Card.transform.position + Vector3.up * 0.05f + Vector3.right * 2.05f + Vector3.forward;
            line.SetPositions([start, targetSlot.transform.position + (attackPlayer ? Vector3.back : Vector3.zero) + Vector3.down * 0.05f]);
            line.alignment = LineAlignment.TransformZ;

            CustomCoroutine.Instance.StartCoroutine(TweenLineWidth(line, attackPlayer, __instance));
            AudioController.Instance.PlaySound3D("cannonfire", MixerGroup.TableObjectsSFX, start, 2.5f);
            return false;
        }

        public static IEnumerator TweenLineWidth(LineRenderer line, bool attackPlayer, CardAnimationController controller)
        {
            if (line == null)
            {
                if (attackPlayer)
                    controller.OnImpactAttackPlayerKeyframe();
                else
                {
                    Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.2f);
                    controller.OnImpactKeyframe();
                }

                yield break;
            }

            var ela = 0f;
            line.widthMultiplier = 0f;
            while (ela < 0.25f && line != null)
            {
                ela += Time.deltaTime;
                line.widthMultiplier = Mathf.Lerp(0f, 1f, ela / 0.25f);
                yield return new WaitForEndOfFrame();
            }

            if (attackPlayer)
                controller.OnImpactAttackPlayerKeyframe();
            else
            {
                Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.2f);
                controller.OnImpactKeyframe();
            }

            if (line == null)
                yield break;

            line.widthMultiplier = 1f;
            ela = 0f;
            while (ela < 0.25f && line != null)
            {
                ela += Time.deltaTime;
                line.widthMultiplier = Mathf.Lerp(1f, 0f, ela / 0.25f);
                yield return new WaitForEndOfFrame();
            }

            if (line != null)
                Object.Destroy(line.gameObject);

            yield break;
        }

        //[HarmonyPatch(typeof(TurnManager), nameof(TurnManager.UpdateSpecialSequencer))]
        //[HarmonyPrefix]
        //public static bool DoAPIsJobForIt(TurnManager __instance, string specialBattleId)
        //{
        //    if (specialBattleId == "SpamtonSpecialSequencer")
        //    {
        //        Object.Destroy(__instance.SpecialSequencer);
        //        __instance.SpecialSequencer = null;
        //        if (!string.IsNullOrEmpty(specialBattleId))
        //        {
        //            Type type = typeof(SpamtonSpecialSequencer);
        //            __instance.SpecialSequencer = __instance.gameObject.AddComponent<SpamtonSpecialSequencer>();
        //        }
        //        return false;
        //    }
        //    return true;
        //}

        [HarmonyPatch(typeof(RunState), nameof(RunState.CurrentMapRegion), MethodType.Getter)]
        [HarmonyPostfix]
        public static void ReplaceRegion(ref RegionData __result)
        {
            if (SaveManager.SaveFile.IsPart3 || SaveManager.SaveFile.IsGrimora)
                return;

            if (!SaveFile.IsAscension)
                return;

            if (RunState.Run.regionTier != RegionProgression.Instance.regions.Count - 1)
                return;

            if (!AscensionSaveData.Data.ChallengeIsActive(Plugin.FinalBossV2Challenge))
                return;

            __result = spamtonRegion;
        }
    }
}
