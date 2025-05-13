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
        public static AscensionChallenge challenge;
        public static RegionData morebossesregion;
        public static Opponent.Type spamboss;
        public static EncounterBlueprintData spamP1;
        public static EncounterBlueprintData spamP2;
        public static Speaker spamtonSpeaker;

        public static Ability neoStrikeAbility;
        public static Ability helpCallAbility;

        public static void Awake()
        {
            challenge = ChallengeManager.Add(GUID, "Final Boss v2", "Replaces Leshy as the final boss with a true BIG SHOT.", 60, LoadTexture("ascensionicon_finalbossv2.png"),
                LoadTexture("ascensionicon_activated_finalbossv2.png"), 12, false).Challenge.challengeType;
            using (Stream strem = CurrentAssembly.GetManifestResourceStream("SquirrelBombMod.Bundles.bigshotbundle"))
            {
                AssetBundle bundl = AssetBundle.LoadFromStream(strem);
                addedSfx = new()
                {
                    bundl.LoadAsset<AudioClip>("car_crash"),
                    bundl.LoadAsset<AudioClip>("car_honk"),
                    bundl.LoadAsset<AudioClip>("car_screech"),
                    bundl.LoadAsset<AudioClip>("sneovoice_calm"),
                    bundl.LoadAsset<AudioClip>("defeated"),
                    bundl.LoadAsset<AudioClip>("cannonfire")
                };
                addedLoops = new()
                {
                    bundl.LoadAsset<AudioClip>("lastphase"),
                    bundl.LoadAsset<AudioClip>("battle")
                };
                sneoPrefab = bundl.LoadAsset<GameObject>("SpamtonNEOAnimation");
                giantSpamtonPrefab = bundl.LoadAsset<GameObject>("SneoGiantPortrait");
                giantSpamtonPrefab.GetComponentsInChildren<Transform>().ToList().ForEach(x => x.gameObject.layer = LayerMask.NameToLayer("CardOffscreen"));
                giantSpamtonPrefab.AddComponent<NeoAnimatedPortrait>();
            }
            Texture2D chaintexture = LoadTexture("heartchain.png");
            chainSprite = Sprite.Create(chaintexture, new Rect(0f, 0f, chaintexture.width, chaintexture.height), new Vector2(0.5f, 0.5f));
            spamtonSpeaker = GuidManager.GetEnumValue<DialogueEvent.Speaker>(GUID, "SpamtonSpeaker");
            AddCards();
            SpecialSequenceManager.Add(GUID, "SpamtonSpecialSequencer", typeof(SpamtonSpecialSequencer));
            spamboss = OpponentManager.Add(GUID, "SpamtonBossOpponent", "SpamtonSpecialSequencer", typeof(SpamtonBossOpponent)).Id;
            morebossesregion = new()
            {
                bosses = new() { spamboss },
                dominantTribes = new() { Tribe.Insect },
                predefinedNodes = new()
                {
                    nodeRows = new()
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
                                bossType = spamboss,
                                specialBattleId = "SpamtonSpecialSequencer"
                            }
                        }
                    }
                },
                predefinedScenery = null,
                fillerScenery = new(),
                scarceScenery = new(),
                encounters = new(),
                ambientLoopId = "cabin_ambience",
                silenceCabinAmbience = false,
                boardLightColor = new Color32(76, 45, 57, 255),
                bossPrepCondition = new(),
                bossPrepEncounter = null,
                cardsLightColor = new Color32(76, 45, 57, 255),
                consumableItems = new(),
                dustParticlesDisabled = false,
                fogAlpha = 0f,
                fogEnabled = false,
                fogProfile = null,
                likelyCards = new(),
                mapAlbedo = RegionProgression.Instance.regions[0].mapAlbedo,
                mapEmission = RegionProgression.Instance.regions[0].mapEmission,
                mapEmissionColor = RegionProgression.Instance.regions[0].mapEmissionColor,
                mapParticlesPrefabs = new(),
                terrainCards = new()
            };
            #region dialogue
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
            spamP1 = new()
            {
                dominantTribes = new() { Tribe.Squirrel },
                minDifficulty = 0,
                maxDifficulty = 100,
                turns = new()
                {
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            minDifficulty = 0,
                            maxDifficulty = 100
                        }
                    }
                }
            };
            spamP2 = new()
            {
                dominantTribes = new() { Tribe.Squirrel },
                minDifficulty = 0,
                maxDifficulty = 100,
                turns = new()
                {
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_SpamMail"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_SpamMail"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_SpamMail"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_SpamMail"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_SpamMail"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_SpamMail"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        }
                    },
                    new()
                    {
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        },
                        new()
                        {
                            card = CardLoader.GetCardByName("morebosses_FlyingHead"),
                            maxDifficulty = 100,
                            minDifficulty = 0
                        }
                    }
                }
            };
            #endregion
        }

        public static void AddCards()
        {
            AbilityInfo pipisab = AbilityManager.New(GUID, "Pipis", "Pipis.", typeof(Pipis), LoadTexture("ability_pipis.png"));
            pipisab.AddMetaCategories(AbilityMetaCategory.Part1Rulebook);
            pipisab.canStack = true;
            Pipis.ab = pipisab.ability;

            AbilityInfo dieheal = AbilityManager.New(GUID, "Help", "At the end of the owner's turn [creature] will sacrifice itself to heal the adjacent cards by 1.", typeof(DieHeal), LoadTexture("ability_dieheal.png"));
            dieheal.AddMetaCategories(AbilityMetaCategory.Part1Rulebook);
            DieHeal.ab = dieheal.ability;

            AbilityInfo passthrough = AbilityManager.New(GUID, "Pass-through", "When [creature] is about to get attacked by a card with an attack higher than this card's health, this card perishes.", typeof(PassThrough),
                LoadTexture("ability_passthrough.png"));
            passthrough.AddMetaCategories(AbilityMetaCategory.Part1Rulebook);
            PassThrough.ab = passthrough.ability;

            AbilityInfo spamCall = AbilityManager.New(GUID, "Spam Call", "Empty spaces within a circuit completed by [creature] spawn Spam Mail at the end of the owner's turn.", typeof(CreateSpamMailConduit),
                LoadTexture("ability_spamcall.png"));
            spamCall.conduit = true;
            spamCall.AddMetaCategories(AbilityMetaCategory.Part1Rulebook);
            CreateSpamMailConduit.ab = spamCall.ability;

            AbilityInfo neoStrike = AbilityManager.New(GUID, "NEO Strike", "[creature] will strike each opposing space that is occupied by a creature and additionally every side of the board occupied by a creature. " +
                "It will strike directly if no creatures oppose it.", typeof(AbilityBehaviour), LoadTexture("ability_neostrike.png"));
            neoStrike.passive = true;
            neoStrike.flipYIfOpponent = true;
            neoStrike.AddMetaCategories(AbilityMetaCategory.Part1Rulebook);
            neoStrikeAbility = neoStrike.ability;

            AbilityInfo helpcall = AbilityManager.New(GUID, "A Call for Help", "At the end of the owner's turn, [creature] will call for help.", typeof(AbilityBehaviour),
                LoadTexture("ability_helpcall.png"));
            helpcall.passive = true;
            helpcall.AddMetaCategories(AbilityMetaCategory.Part1Rulebook);
            helpCallAbility = helpcall.ability;

            CardInfo pipis = CardManager.New("morebosses", "Pipis", "Pipis", 0, 1, "Pipis.");
            pipis.SetPortrait(LoadTexture("portrait_pipis.png"));
            pipis.AddAbilities(Pipis.ab);

            CardInfo flyinghead = CardManager.New("morebosses", "FlyingHead", "Flying Head", 1, 1, "The most important part.");
            flyinghead.SetPortrait(LoadTexture("portrait_flyinghead.png"));
            flyinghead.AddAbilities(PassThrough.ab);
            flyinghead.SetTribes(Tribe.Squirrel);

            CardInfo spamcart = CardManager.New("morebosses", "SpamCart", "Spam Cart", 0, 3, "Filled with spam mail. Absorbs all kinds of attacks.");
            spamcart.SetPortrait(LoadTexture("portrait_spamcart.png"));
            spamcart.AddAbilities(Ability.StrafeSwap, Ability.PreventAttack);

            CardInfo spammail = CardManager.New("morebosses", "SpamMail", "Spam Mail", 0, 2, "Has all kinds of unforgettable deals.");
            spammail.SetPortrait(LoadTexture("portrait_spammail.png"));
            spammail.AddAbilities(Ability.WhackAMole, Ability.Sharp, Ability.Reach);
            spammail.SetTribes(Tribe.Squirrel);

            CardInfo bigshooter = CardManager.New("morebosses", "BigShooter", "Big Shooter", 2, 1, "Shoots big shots.");
            bigshooter.SetPortrait(LoadTexture("portrait_bigshooter.png"));
            bigshooter.AddAbilities(Ability.Brittle);
            bigshooter.AddTraits(Trait.Terrain);
            bigshooter.AddAppearances(CardAppearanceBehaviour.Appearance.TerrainBackground);

            CardInfo chainheart = CardManager.New("morebosses", "ChainHeart", "Heart on a Chain", 1, 9, "Beautiful.");
            chainheart.SetPortrait(LoadTexture("portrait_chainheart.png"));
            chainheart.AddAbilities(Ability.Strafe, Ability.SplitStrike);

            CardInfo spamgel = CardManager.New("morebosses", "Spamgel", "Spamgel", 1, 2, "The result of pressing F1.");
            spamgel.SetPortrait(LoadTexture("portrait_spamgel.png"));
            spamgel.AddAbilities(DieHeal.ab);
            spamgel.SetTribes(Tribe.Squirrel);

            CardInfo lefthandphone = CardManager.New("morebosses", "HandPhoneLeft", "Hand Phone", 0, 5, "Do you really need anyone else?");
            lefthandphone.SetPortrait(LoadTexture("portrait_leftphonehand.png"));
            lefthandphone.AddAbilities(CreateSpamMailConduit.ab);

            CardInfo righthandphone = CardManager.New("morebosses", "HandPhoneRight", "Hand Phone", 0, 5, "Do you really need anyone else?");
            righthandphone.SetPortrait(LoadTexture("portrait_rightphonehand.png"));
            righthandphone.AddAbilities(Ability.ConduitBuffAttack);

            //giantcard
            CardInfo powerofneo = CardManager.New("morebosses", "!GIANTCARD_NEO", "THE POWER OF NEO!!!", 1, 40, "Don't you wanna be a big shot?");
            powerofneo.AddAppearances(CardAppearanceBehaviour.Appearance.GiantAnimatedPortrait);
            powerofneo.AddTraits(Trait.Giant, Trait.Uncuttable);
            powerofneo.AddSpecialAbilities(SpecialTriggeredAbility.GiantCard);
            powerofneo.animatedPortrait = giantSpamtonPrefab;
            powerofneo.AddAbilities(neoStrikeAbility, Ability.Reach, Ability.MadeOfStone);

            //ending
            CardInfo receiver = CardManager.New("morebosses", "Receiver", "Receiver", -999, 99999, "The voice runs out eventually.");
            receiver.SetPortrait(LoadTexture("portrait_receiver.png"));
            receiver.hideAttackAndHealth = true;
            receiver.AddTraits(Trait.Terrain);
            receiver.AddAbilities(helpCallAbility);
            receiver.AddAppearances(CardAppearanceBehaviour.Appearance.TerrainBackground);

            //bear challenge
            CardInfo bearpipis = CardManager.New("morebosses", "BearPipis", "Grizzlipis", 0, 1, "Grizzlipis.");
            bearpipis.SetPortrait(LoadTexture("portrait_bearpipis.png"));
            bearpipis.AddAbilities(Pipis.ab, Pipis.ab);
            bearpipis.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardInfo bearcart = CardManager.New("morebosses", "BearCart", "Bear Cart", 2, 4, "Filled with bears. Absorbs all kinds of attacks.");
            bearcart.SetPortrait(LoadTexture("portrait_bearcart.png"));
            bearcart.AddAbilities(Ability.StrafeSwap, Ability.PreventAttack, Ability.Reach);
            bearcart.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardInfo bearmail = CardManager.New("morebosses", "BearMail", "Spam Mail", 0, 3, "Mail for bears.");
            bearmail.SetPortrait(LoadTexture("portrait_bearmail.png"));
            bearmail.AddAbilities(Ability.WhackAMole, Ability.Sharp, Ability.DeathShield, Ability.Reach);
            bearmail.SetTribes(Tribe.Squirrel);
            bearmail.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardInfo bearheart = CardManager.New("morebosses", "BearHeart", "Bear on a Chain", 1, 10, "Bearutiful.");
            bearheart.SetPortrait(LoadTexture("portrait_bearheart.png"));
            bearheart.AddAbilities(Ability.Strafe, Ability.TriStrike, Ability.MadeOfStone);
            bearheart.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardInfo lefthandbear = CardManager.New("morebosses", "HandBearLeft", "Bear Hand", 0, 5, "Do you really need anyone else?");
            lefthandbear.SetPortrait(LoadTexture("portrait_leftbearhand.png"));
            lefthandbear.AddAbilities(Ability.ConduitBuffAttack, Ability.Reach, Ability.MadeOfStone);
            lefthandbear.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);

            CardInfo righthandbear = CardManager.New("morebosses", "HandBearRight", "Bear Hand", 0, 5, "Do you really need anyone else?");
            righthandbear.SetPortrait(LoadTexture("portrait_rightbearhand.png"));
            righthandbear.AddAbilities(CreateSpamMailConduit.ab, Ability.Reach, Ability.MadeOfStone);
            righthandbear.AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground);
        }

        [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.GetOpposingSlots))]
        [HarmonyPrefix]
        public static bool AddNeoStrike(PlayableCard __instance, ref List<CardSlot> __result)
        {
            if (__instance.HasAbility(neoStrikeAbility))
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
            if (__instance.PlayableCard.Info.name == "morebosses_!GIANTCARD_NEO")
            {
                NeoAnimatedPortrait neo = Singleton<CardRenderCamera>.Instance.GetLiveRenderCamera(__instance.Card.StatsLayer as RenderLiveStatsLayer).GetComponentInChildren<NeoAnimatedPortrait>();
                neo.AttackSpot();
                GameObject lineobj = new("Line");
                LineRenderer line = lineobj.AddComponent<LineRenderer>();
                line.startColor = Color.white;
                line.endColor = Color.white;
                line.material = neo.GetGlowyMaterial();
                line.startWidth = 0;
                line.endWidth = 1.5f;
                line.widthMultiplier = 0f;
                Vector3 start = __instance.Card.transform.position + Vector3.up * 0.05f + Vector3.right * 2.05f + Vector3.forward;
                line.SetPositions(new Vector3[] { start, targetSlot.transform.position + (attackPlayer ? Vector3.back : Vector3.zero) + Vector3.down * 0.05f });
                line.alignment = LineAlignment.TransformZ;
                CustomCoroutine.Instance.StartCoroutine(TweenLineWidth(line, attackPlayer, __instance));
                AudioController.Instance.PlaySound3D("cannonfire", MixerGroup.TableObjectsSFX, start, 2.5f);
                return false;
            }
            return true;
        }

        public static IEnumerator TweenLineWidth(LineRenderer line, bool attackPlayer, CardAnimationController controller)
        {
            if (line == null)
            {
                if (attackPlayer)
                {
                    controller.OnImpactAttackPlayerKeyframe();
                }
                else
                {
                    Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.2f);
                    controller.OnImpactKeyframe();
                }
                yield break;
            }
            float ela = 0f;
            line.widthMultiplier = 0f;
            while (ela < 0.25f)
            {
                if (line == null)
                {
                    if (attackPlayer)
                    {
                        controller.OnImpactAttackPlayerKeyframe();
                    }
                    else
                    {
                        Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.2f);
                        controller.OnImpactKeyframe();
                    }
                    yield break;
                }
                ela += Time.deltaTime;
                line.widthMultiplier = Mathf.Lerp(0f, 1f, ela / 0.25f);
                yield return new WaitForEndOfFrame();
            }
            if (attackPlayer)
            {
                controller.OnImpactAttackPlayerKeyframe();
            }
            else
            {
                Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.2f);
                controller.OnImpactKeyframe();
            }
            if (line == null)
            {
                yield break;
            }
            line.widthMultiplier = 1f;
            ela = 0f;
            while (ela < 0.25f)
            {
                if (line == null)
                {
                    yield break;
                }
                ela += Time.deltaTime;
                line.widthMultiplier = Mathf.Lerp(1f, 0f, ela / 0.25f);
                yield return new WaitForEndOfFrame();
            }
            if (line == null)
            {
                yield break;
            }
            Object.Destroy(line.gameObject);
            yield break;
        }

        [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.UpdateSpecialSequencer))]
        [HarmonyPrefix]
        public static bool DoAPIsJobForIt(TurnManager __instance, string specialBattleId)
        {
            if (specialBattleId == "SpamtonSpecialSequencer")
            {
                Object.Destroy(__instance.SpecialSequencer);
                __instance.SpecialSequencer = null;
                if (!string.IsNullOrEmpty(specialBattleId))
                {
                    Type type = typeof(SpamtonSpecialSequencer);
                    __instance.SpecialSequencer = __instance.gameObject.AddComponent<SpamtonSpecialSequencer>();
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(RunState), nameof(RunState.CurrentMapRegion), MethodType.Getter)]
        [HarmonyPostfix]
        public static void ReplaceRegion(ref RegionData __result)
        {
            if (SaveManager.SaveFile.IsPart3 || SaveManager.SaveFile.IsGrimora)
            {
                return;
            }
            if (!SaveFile.IsAscension)
            {
                return;
            }
            if (RunState.Run.regionTier != RegionProgression.Instance.regions.Count - 1)
            {
                return;
            }
            if (AscensionSaveData.Data.ChallengeIsActive(challenge))
            {
                __result = morebossesregion;
            }
        }
    }
}
