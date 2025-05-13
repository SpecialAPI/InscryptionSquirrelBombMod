using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod
{
    public partial class Plugin
    {
        public static void AddLoop()
        {
            //loop node
            /*var loop = NewNodeManager.New<LoopSequencer>(GUID, "LoopNode", GenerationType.None, new()
            {
                LoadTexture("animated_loop_1"),
                LoadTexture("animated_loop_2"),
                LoadTexture("animated_loop_3"),
                LoadTexture("animated_loop_4")
            });
            RegionProgression.Instance.ascensionFinalRegion.predefinedNodes.nodeRows[1].Add(
                new CustomSpecialNodeData(loop)
                {
                    position = new(0.2f, 0.65f)
                }
            );
            RegionProgression.Instance.ascensionFinalBossRegion.predefinedNodes.nodeRows[1].Add(
                new CustomSpecialNodeData(loop)
                {
                    position = new(0.2f, 0.65f)
                }
            );
            
            //loop final
            LOOPFINALSEQUENCER = SpecialSequenceManager.Add(GUID, "TimeGuardianBattleSequencer", typeof(TimeGuardianBattleSequencer)).Id;
            LOOPFINALBOSS = OpponentManager.Add(GUID, "TimeGuardianBossOpponent", LOOPFINALSEQUENCER, typeof(TimeGuardianBossOpponent), new()
            {
                LoadTexture("animated_timeguardian_battle_1"),
                LoadTexture("animated_timeguardian_battle_2"),
                LoadTexture("animated_timeguardian_battle_3"),
                LoadTexture("animated_timeguardian_battle_4"),
            }).Id;
            LOOPFINAL = new()
            {
                cardsLightColor = new Color(0f, 0f, 0.83f),
                boardLightColor = new Color(0f, 0f, 0.83f),
                ambientLoopId = "",
                dominantTribes = new() { Tribe.Canine },
                bossPrepCondition = null,
                bossPrepEncounter = null,
                consumableItems = new(),
                dustParticlesDisabled = false,
                encounters = new(),
                fillerScenery = RegionProgression.Instance.regions.ConvertAll(x => x.fillerScenery).SelectMany(x => x).ToList(),
                scarceScenery = RegionProgression.Instance.regions.ConvertAll(x => x.scarceScenery).SelectMany(x => x).ToList(),
                fogAlpha = 0f,
                fogEnabled = false,
                fogProfile = null,
                likelyCards = new(),
                mapAlbedo = RegionProgression.Instance.regions[0].mapAlbedo,
                mapEmission = RegionProgression.Instance.regions[0].mapEmission,
                mapEmissionColor = Color.black,
                mapParticlesPrefabs = new(),
                predefinedNodes = new()
                {
                    nodeRows = new()
                    {
                        new()
                        {
                            new()
                            {
                                position = new(0.5f, 0.4f)
                            }
                        },
                        new()
                        {
                            new CustomSpecialNodeData(loop)
                            {
                                position = new(0.3f, 0.7f)
                            },
                            new BossBattleNodeData()
                            {
                                bossType = LOOPFINALBOSS,
                                difficulty = 20,
                                specialBattleId = LOOPFINALSEQUENCER,
                                position = new(0.7f, 0.7f)
                            }
                        }
                    }
                },
                bosses = new() { LOOPFINALBOSS },
                predefinedScenery = new() { scenery = new() },
                silenceCabinAmbience = false,
                terrainCards = new(),
                name = "LOOPFINALREGION"
            };

            //dialogue
            DialogueEventGenerator.GenerateEvent("LoopNodeIntroduction", new()
            {
                "When started approaching it, the bell started ringing.",
                "As you approached it, the ringing was getting louder and louder.",
                "As the ringing got louder, your eyes began to close.",
                "You suddenly started feeling tired, your body started to feel weak.",
                "You fell on the ground, overwhelmed by the ringing, as your eyes fully close."
            });
            DialogueEventGenerator.GenerateEvent("LoopNodePostLoop", new()
            {
                "Then, suddenly, you got up from the ground and began opening your eyes.",
                "But, when you opened your eyes, you found yourself in a completely different place, the bell nowhere to be found.",
                "You noticed that some of your unloyal beasts disappeared.",
                "You decided to proceed, as if nothing happened."
            });*/
        }
    }
}
