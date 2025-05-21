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
                "HOLY [CUNGADERO] DO I FEEL GOOD"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoFinalIntro1", new()
            {
                "HERE I AM!!!",
                "BIG",
                "<size=10>BIG!!!</size>",
                "<size=15>[BIGGER AND BETTER THAN EVER]</size>",
                "AND NOW THAT I'M IN THIS [GAME OF CARDS]",
                "I CAN'T BE DEFEATED!!!",
                "PREPARE TO GET [ABSOLUTELY OBLITERATED] BY [THE POWER OF NEO]!!!"
            }, new()
            {
                new()
                {
                    "[THIS] AGAIN???",
                    "PREPARE TO GET [DESTROYED]!!!"
                },
                new()
                {
                    "YOU WON'T STAND A CHANGE, YOU [LITTLE RING WORM]!!!"
                },
                new()
                {
                    "PREPARE TO [GET PUNCHED] IN YOUR [FACE]!!!"
                },
                new()
                {
                    "BEHOLED [ME]!!! BEHOLD [YOU]!!!"
                },
                new()
                {
                    "[HYPERLINK BLOCKED]"
                }
            }, DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase1", new()
            {
                "ISN'T THIS BODY JUST [HEAVEN]LY?",
                "2X THE [FIRE]POWER",
                "3X THE [WATER]POWER",
                "AND MOST IMPORTANTLY",
                "FLYING HEADS!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase2", new()
            {
                "WE'LL TURN THOSE [SCHMOES] AND [DAVES]",
                "INTO [ROSEN GRAVES]",
                "THOSE [CATHODE SCREENS]",
                "INTO [CATHODE SCREAMS]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase3", new()
            {
                "DON'T YOU WANNA BE [PART] OF MY BEAUTIFUL [HEART]?!",
                "OR... DID YOU NEED A LITTLE [SPECIL TOUR]?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoHeartStolen", new()
            {
                "[WHAT]?!",
                "YOU [STOLE] MY [HEART]?",
                "YOU CAN'T BE [STEALING] MY [HEART], IT IS MY [HEART]!"
            }, new() { new() { "STOP [STEALING] MY HEART YOU [!@#$]" } }, DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoHeartBroken", new()
            {
                "O U C [HYPERLINK BLOCKED]",
                "YOU JUST [BROKE] MY [$4.99] HEART",
                "DO YOU KNOW THAT IT [HURTS] WHEN YOU [DO] THAT?!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase4", new()
            {
                "DON'T YOU WANNA HELP YOUR OLD PAL SPAMTON?",
                "THINK! WHAT ARE MY [EGGS] GOING TO DO!?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase5", new()
            {
                "IT'S CALLING...",
                "MY [HEART]...",
                "MY [HANDS]..."
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoPhase6", new()
            {
                "LOOK AT THE [POWER OF NEO] AND ASK YOURSELF...",
                "DON'T YOU?",
                "DON'T YOU WANNA BE A [BIG SHOT]!?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingPhase", new()
            {
                "MY ESTEEM CUSTOMER I SEE YOU ARE ATTEMPTING TO DEPLETE MY HP!",
                "I'LL ADMIT YOU'VE GOT SOME [GUTS]!",
                "BUT IN A [1 FOR 1] BATTLE, NEO NEVER LOSES!!!",
                "IT'S TIME FOR A LITTLE [BLUELIGHT SPECIL]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEnding1Damage", new()
            {
                "[WHAT]?",
                "ARE YOU [SURPRISED] THAT YOU CAN'T JUST [DESTROY] ME WITH YOUR [PATHETIC LITTLE] [CARDS]?",
                "DIDN'T YOU KNOW [NEO] IS FAMOUS FOR ITS HIGH DEFENSE!?"
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
                "[WHO] ARE YOU TRYING TO [CALL]???",
                "YOU HAVE NO [FRIENDS] IN THIS [WORLD]!!!",
                "THEY'RE ALL [PAPER CARDS]!!!",
                "WHO'RE YOU GONNA CALL, THE [POLICE]?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver2", new()
            {
                "YOU'RE [PATHETIC]!!",
                "[SCREAMING] FOR [HELP] [AT THE BOTTOM OF A DUMPSTER]!!!",
                "THERE'S NO [HELP] FOR YOU!!!",
                "YOUR [VOICE] IS GOING TO RUN OUT EVENTUALLY!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver3", new()
            {
                "GIVE UP!!!",
                "THERE'S NO [HELP] FOR YOU!!!",
                "YOU CAN'T WIN!!!",
                "I AM UNSTOPPABLE!!!",
                "YOUR [FREE TRIAL OF] LIFE IS ABOUT TO [EXPIRE]!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver4", new()
            {
                "[GOOBERT]?",
                "IS IT THAT [PATHETIC] LITTLE [JAR] OF [GOO]?",
                "WHAT [HELP] WILL IT [PROVIDE] TO [YOU]???"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver5", new()
            {
                "[MAGNIFICUS]??",
                "WHAT IS A [MAGNIFICUS]? IS IT A KIND OF [BLOWFISH]?",
                "WHAT'S A [BLOWFISH] GONNA [DO TO ME]??"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver6", new()
            {
                "[LESHY]???",
                "IS THAT THE [GUY] WHO I [HIT] AT THE BEGGINING OF OUR [FIGHT]???",
                "HA! I'M NOT EVEN SURE THAT HE IS [ALIVE AND WELL]!!!"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver7", new()
            {
                "[[ROYAL]]?",
                "ROYAL [WHAT]? ROYAL [WHO]?",
                "[WHO/WHAT/WHEN/WHY/HOW] IS [ROYAL]?",
                "YOU'RE SO PATHETIC",
                "MUMBLING NONSENSE WORDS THROUGH A [PLAYING CARD]",
                "DO YOU THINK YOU CAN [AVOID] YOUR [UNAVOIDABLE, QUICKLY APPROACHING] DEMISE?"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver8", new()
            {
                "[THERE]",
                "WHAT ARE YOU GONNA DO WITHOUT YOUR [COMMUNICATION DEVICE]???"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoReceiver9", new()
            {
                "WAIT IS IT JUST [ME]",
                "OR DOES IT SOUND LIKE A [SHIP] IS COMING [OUR] WAY??",
                "IT'S PROBABLY [NOTHING] [AT HALF PRICE]",
                "IT'S PROBABLY JUST A SOUND IN MY [HEAD], IN MY [EARS]",
                "THERE'S NO WAY A [SHIP] COULD FIT IN THIS LITTLE [CABIN FOR SALE]",
                "YOU WILL NEVER DEFEAT-"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingAlmostDeath", new()
            {
                "SAY [GOODBYE] [TO YOUR FRIENDS AND FAMILY]"
            }, new(), DialogueEvent.MaxRepeatsBehaviour.RandomDefinedRepeat, spamtonSpeaker);
            DialogueManager.GenerateEvent(GUID, "SneoEndingDeath", new()
            {
                "THE [POWER OF NEO] WAS TOO MUCH FOR YOU"
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
