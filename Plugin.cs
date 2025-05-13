global using static SquirrelBombMod.Tools;
global using Object = UnityEngine.Object;
global using Random = UnityEngine.Random;
global using SystemRandom = System.Random;
global using InscryptionAPI;
global using HarmonyLib;
global using Pixelplacement;
global using InscryptionAPI.Ascension;
global using SquirrelBombMod.Challenges;
global using InscryptionAPI.Card;
global using SquirrelBombMod.Abilities.Curses;
global using InscryptionAPI.Helpers;
global using InscryptionAPI.Nodes;
global using BepInEx;
global using SquirrelBombMod.Abilities;
global using SquirrelBombMod.Looping;
global using InscryptionAPI.Saves;
global using DiskCardGame;
global using InscryptionAPI.Boons;
global using SquirrelBombMod.Looping.Modifiers;
global using System.Collections;
global using System.Collections.Generic;
global using InscryptionAPI.Helpers.Extensions;
global using System.Linq;
global using UnityEngine;
global using InscryptionAPI.Guid;
global using Debug = UnityEngine.Debug;
global using InscryptionAPI.Encounters;
global using SquirrelBombMod.Bosses.TimeGuardian;
global using EncounterBuilder = DiskCardGame.EncounterBuilder;
global using APIEncounterBuilder = InscryptionAPI.Encounters.EncounterBuilder;
using System;
using InscryptionAPI.Dialogue;

namespace SquirrelBombMod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public partial class Plugin : BaseUnityPlugin
    {
        public static RegionData LOOPFINAL;
        public static Opponent.Type LOOPFINALBOSS;
        public static string LOOPFINALSEQUENCER;
        public const int LOOP_LEVEL_REQUIRED_FOR_FINAL = 2;

        public void Awake()
        {
            Setup();

            new Harmony(GUID).PatchAll();

            Spamton.SpamtonSetup.Awake();
            AddAbilities();
            AddCards();
            //AddLoop();
            AddChallenges();
            AddStarterDecks();

            DialogueManager.GenerateEvent(GUID, "PlayerSkipTurn", new()
            {
                "You need to pass, remember?"
            });
        }

        [HarmonyPatch(typeof(RunState), nameof(RunState.CurrentMapRegion), MethodType.Getter)]
        [HarmonyPostfix]
        public static void ReplaceFinalLoopRegion(ref RegionData __result)
        {
            //if(LoopSequencer.LoopTier >= LOOP_LEVEL_REQUIRED_FOR_FINAL && RunState.Run.regionTier >= 3)
            //{
                //__result = LOOPFINAL;
            //}
        }

        [HarmonyPatch(typeof(AudioController), nameof(AudioController.GetAudioClip))]
        [HarmonyPrefix]
        public static void AddAudios(AudioController __instance, string soundId)
        {
            __instance.SFX.AddRange(addedSfx.Where(x => !__instance.SFX.Contains(x)));
        }

        [HarmonyPatch(typeof(AudioController), nameof(AudioController.GetLoopClip))]
        [HarmonyPrefix]
        public static void AddLoops(AudioController __instance, string loopId)
        {
            __instance.Loops.AddRange(addedLoops.Where(x => !__instance.Loops.Contains(x)));
        }
    }
}
