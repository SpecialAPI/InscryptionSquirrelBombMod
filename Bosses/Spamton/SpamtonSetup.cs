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
                "HOLY [_spamton_0,CUNGADERO] DO I FEEL GOOD"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoFinalIntro1", new()
            {
                "HERE I AM!!!",
                "BIG",
                "<size=10>BIG!!!</size>",
                "<size=15>[_spamton_0,BIGGER AND BETTER THAN EVER]</size>",
                "AND NOW THAT I'M IN THIS [_spamton_0,GAME OF CARDS]",
                "I CAN'T BE DEFEATED!!!",
                "PREPARE TO GET [_spamton_0,ABSOLUTELY OBLITERATED] BY [_spamton_0,THE POWER OF NEO]!!!"
            }, new()
            {
                new()
                {
                    "[_spamton_0,THIS] AGAIN???",
                    "PREPARE TO GET [_spamton_0,DESTROYED]!!!"
                },
                new()
                {
                    "YOU WON'T STAND A CHANGE, YOU [_spamton_0,LITTLE RING WORM]!!!"
                },
                new()
                {
                    "PREPARE TO [_spamton_0,GET PUNCHED] IN YOUR [_spamton_0,FACE]!!!"
                },
                new()
                {
                    "BEHOLED [_spamton_0,ME]!!! BEHOLD [_spamton_0,YOU]!!!"
                },
                new()
                {
                    "[_spamton_0,HYPERLINK BLOCKED]"
                }
            }, DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase1", new()
            {
                "ISN'T THIS BODY JUST [_spamton_0,HEAVEN]LY?",
                "2X THE [_spamton_0,FIRE]POWER",
                "3X THE [_spamton_0,WATER]POWER",
                "AND MOST IMPORTANTLY",
                "FLYING HEADS!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase2", new()
            {
                "WE'LL TURN THOSE [_spamton_0,SCHMOES] AND [_spamton_0,DAVES]",
                "INTO [_spamton_0,ROSEN GRAVES]",
                "THOSE [_spamton_0,CATHODE SCREENS]",
                "INTO [_spamton_0,CATHODE SCREAMS]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase3", new()
            {
                "DON'T YOU WANNA BE [_spamton_0,PART] OF MY BEAUTIFUL [_spamton_0,HEART]?!",
                "OR... DID YOU NEED A LITTLE [_spamton_0,SPECIL TOUR]?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoHeartStolen", new()
            {
                "[_spamton_0,WHAT]?!",
                "YOU [_spamton_0,STOLE] MY [_spamton_0,HEART]?",
                "YOU CAN'T BE [_spamton_0,STEALING] MY [_spamton_0,HEART], IT IS MY [_spamton_0,HEART]!"
            }, new() { new() { "STOP [_spamton_0,STEALING] MY HEART YOU [_spamton_0,!@#$]" } }, DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoHeartBroken", new()
            {
                "O U C [_spamton_0,HYPERLINK BLOCKED]",
                "YOU JUST [_spamton_0,BROKE] MY [_spamton_0,$4.99] HEART",
                "DO YOU KNOW THAT IT [_spamton_0,HURTS] WHEN YOU [_spamton_0,DO] THAT?!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase4", new()
            {
                "DON'T YOU WANNA HELP YOUR OLD PAL SPAMTON?",
                "THINK! WHAT ARE MY [_spamton_0,EGGS] GOING TO DO!?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase5", new()
            {
                "IT'S CALLING...",
                "MY [_spamton_0,HEART]...",
                "MY [_spamton_0,HANDS]..."
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase6", new()
            {
                "LOOK AT THE [_spamton_0,POWER OF NEO] AND ASK YOURSELF...",
                "DON'T YOU?",
                "DON'T YOU WANNA BE A [_spamton_0,BIG SHOT]!?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingPhase", new()
            {
                "MY ESTEEM CUSTOMER I SEE YOU ARE ATTEMPTING TO DEPLETE MY HP!",
                "I'LL ADMIT YOU'VE GOT SOME [_spamton_0,GUTS]!",
                "BUT IN A [_spamton_0,1 FOR 1] BATTLE, NEO NEVER LOSES!!!",
                "IT'S TIME FOR A LITTLE [_spamton_0,BLUELIGHT SPECIL]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEnding1Damage", new()
            {
                "[_spamton_0,WHAT]?",
                "ARE YOU [_spamton_0,SURPRISED] THAT YOU CAN'T JUST [_spamton_0,DESTROY] ME WITH YOUR [_spamton_0,PATHETIC LITTLE] [_spamton_0,CARDS]?",
                "DIDN'T YOU KNOW [_spamton_0,NEO] IS FAMOUS FOR ITS HIGH DEFENSE!?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingFireworksLong", new()
            {
                "NOW... ENJ0Y THE FIR3WORKS!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingFireworks", new()
            {
                "ENJOY THE FIREWORKS!!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver1", new()
            {
                "WHAT?",
                "[_spamton_0,WHO] ARE YOU TRYING TO [_spamton_0,CALL]???",
                "YOU HAVE NO [_spamton_0,FRIENDS] IN THIS [_spamton_0,WORLD]!!!",
                "THEY'RE ALL [_spamton_0,PAPER CARDS]!!!",
                "WHO'RE YOU GONNA CALL, THE [_spamton_0,POLICE]?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver2", new()
            {
                "YOU'RE [_spamton_0,PATHETIC]!!",
                "[_spamton_0,SCREAMING] FOR [_spamton_0,HELP] [_spamton_0,AT THE BOTTOM OF A DUMPSTER]!!!",
                "THERE'S NO [_spamton_0,HELP] FOR YOU!!!",
                "YOUR [_spamton_0,VOICE] IS GOING TO RUN OUT EVENTUALLY!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver3", new()
            {
                "GIVE UP!!!",
                "THERE'S NO [_spamton_0,HELP] FOR YOU!!!",
                "YOU CAN'T WIN!!!",
                "I AM UNSTOPPABLE!!!",
                "YOUR [_spamton_0,FREE TRIAL OF] LIFE IS ABOUT TO [_spamton_0,EXPIRE]!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver4", new()
            {
                "[_spamton_0,GOOBERT]?",
                "IS IT THAT [_spamton_0,PATHETIC] LITTLE [_spamton_0,JAR] OF [_spamton_0,GOO]?",
                "WHAT [_spamton_0,HELP] WILL IT [_spamton_0,PROVIDE] TO [_spamton_0,YOU]???"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver5", new()
            {
                "[_spamton_0,MAGNIFICUS]??",
                "WHAT IS A [_spamton_0,MAGNIFICUS]? IS IT A KIND OF [_spamton_0,BLOWFISH]?",
                "WHAT'S A [_spamton_0,BLOWFISH] GONNA [_spamton_0,DO TO ME]??"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver6", new()
            {
                "[_spamton_0,LESHY]???",
                "IS THAT THE [_spamton_0,GUY] WHO I [_spamton_0,HIT] AT THE BEGGINING OF OUR [_spamton_0,FIGHT]???",
                "HA! I'M NOT EVEN SURE THAT HE IS [_spamton_0,ALIVE AND WELL]!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver7", new()
            {
                "[_spamton_0,_spamton_1,ROYAL]]?",
                "ROYAL [_spamton_0,WHAT]? ROYAL [_spamton_0,WHO]?",
                "[_spamton_0,WHO/WHAT/WHEN/WHY/HOW] IS [_spamton_0,ROYAL]?",
                "YOU'RE SO PATHETIC",
                "MUMBLING NONSENSE WORDS THROUGH A [_spamton_0,PLAYING CARD]",
                "DO YOU THINK YOU CAN [_spamton_0,AVOID] YOUR [_spamton_0,UNAVOIDABLE, QUICKLY APPROACHING] DEMISE?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver8", new()
            {
                "[_spamton_0,THERE]",
                "WHAT ARE YOU GONNA DO WITHOUT YOUR [_spamton_0,COMMUNICATION DEVICE]???"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver9", new()
            {
                "WAIT IS IT JUST [_spamton_0,ME]",
                "OR DOES IT SOUND LIKE A [_spamton_0,SHIP] IS COMING [_spamton_0,OUR] WAY??",
                "IT'S PROBABLY [_spamton_0,NOTHING] [_spamton_0,AT HALF PRICE]",
                "IT'S PROBABLY JUST A SOUND IN MY [_spamton_0,HEAD], IN MY [_spamton_0,EARS]",
                "THERE'S NO WAY A [_spamton_0,SHIP] COULD FIT IN THIS LITTLE [_spamton_0,CABIN FOR SALE]",
                "YOU WILL NEVER DEFEAT-"
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
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
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
                    BuildCardBlueprint("morebosses_SpamMail"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_SpamMail"),
                    BuildCardBlueprint("morebosses_SpamMail"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_SpamMail"),
                    BuildCardBlueprint("morebosses_SpamMail"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_SpamMail"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
                },
                new()
                {
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead"),
                    BuildCardBlueprint("morebosses_FlyingHead")
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
            if (__instance.PlayableCard.Info.name != "morebosses_!GIANTCARD_NEO")
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
