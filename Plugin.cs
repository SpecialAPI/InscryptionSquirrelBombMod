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
global using InscryptionAPI.Helpers;
global using InscryptionAPI.Nodes;
global using BepInEx;
global using InscryptionAPI.Saves;
global using DiskCardGame;
global using InscryptionAPI.Boons;
global using System.Collections;
global using System.Collections.Generic;
global using InscryptionAPI.Helpers.Extensions;
global using System.Linq;
global using UnityEngine;
global using InscryptionAPI.Guid;
global using Debug = UnityEngine.Debug;
global using InscryptionAPI.Encounters;
global using EncounterBuilder = DiskCardGame.EncounterBuilder;
global using APIEncounterBuilder = InscryptionAPI.Encounters.EncounterBuilder;
using System;
using InscryptionAPI.Dialogue;
using SquirrelBombMod.Spamton;

namespace SquirrelBombMod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public partial class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Setup();

            new Harmony(GUID).PatchAll();

            AddAbilities();
            AddCards();
            AddChallenges();
            AddStarterDecks();
            SpamtonSetup.Awake();

            DialogueManager.GenerateEvent(GUID, "PlayerSkipTurn", new()
            {
                "You need to pass, remember?"
            });
        }

        [HarmonyPatch(typeof(AscensionIconInteractable), nameof(AscensionIconInteractable.AssignInfo))]
        [HarmonyPostfix]
        public static void FixBigShotChallenge(AscensionIconInteractable __instance, AscensionChallengeInfo info)
        {
            if (info.challengeType != FinalBossV2Challenge)
                return;

            __instance.activatedRenderer.sortingOrder = 1;
        }
    }
}
