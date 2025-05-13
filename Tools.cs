using DiskCardGame;
using InscryptionAPI.Guid;
using InscryptionAPI.Helpers;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod
{
    [HarmonyPatch]
    public static class Tools
    {
        public const string GUID = "spapi.inscryption.squirrelbombmod";
        public const string NAME = "The Squirrel Bomb Mod";
        public const string VERSION = "1.1.0";
        public const string PREFIX = "spapi";
        public const string ASSETPATH = "SquirrelBombMod.Assets.";

        public static GameObject playablecard_part1 = Resources.Load<GameObject>("prefabs/cards/playablecard");
        public static GameObject playablecardCam_part1 = Resources.Load<GameObject>("prefabs/cards/CardRenderCamera_Live");
        public static GameObject playablecard_part3 = Resources.Load<GameObject>("prefabs/cards/playablecard_part3");
        public static GameObject playablecardCam_part3 = Resources.Load<GameObject>("prefabs/cards/CardRenderCamera_Part3_Live");
        public static GameObject playablecard_grimora = Resources.Load<GameObject>("prefabs/cards/playablecard_grimora");
        public static GameObject playablecardCam_grimora = Resources.Load<GameObject>("prefabs/cards/CardRenderCamera_Grimora");
        public static GameObject playablecard_magnificus = Resources.Load<GameObject>("prefabs/cards/playablecard_magnificus");
        public static GameObject playablecardCam_magnificus = Resources.Load<GameObject>("prefabs/cards/CardRenderCamera_Magnificus");
        public static GameObject bellPrefab = Resources.Load<GameObject>("prefabs/cardbattle/CardBattle_Magnificus").transform.Find("CombatBell_Magnificus").Find("Anim").gameObject;
        public static AssetCollection assets = LoadAllAssets();
        public static List<AudioClip> addedSfx = new();
        public static List<AudioClip> addedLoops = new();
        public static readonly Color ColorCurse = new Color32(40, 0, 52, 255);
        public static readonly AbilityMetaCategory CurseMetacategory = GuidManager.GetEnumValue<AbilityMetaCategory>(GUID, "Curse");
        private static Assembly _assembly;
        public static Assembly CurrentAssembly => _assembly ??= Assembly.GetExecutingAssembly();

        public static void Setup()
        {
        }

        public static int BetterCostTier(this CardInfo self)
        {
            return self.BloodCost * 3 + self.BonesCost + self.energyCost + self.gemsCost.Count * 3;
        }

        public static Texture2D LoadTexture(string name)
        {
            return TextureHelper.GetImageAsTexture(name + (name.EndsWith(".png") ? "" : ".png"), CurrentAssembly);
        }

        public static Sprite LoadSprite(string name)
        {
            return LoadTexture(name)?.ConvertTexture();
        }

        public static AssetBundle LoadAssets(string name)
        {
            try
            {
                return AssetBundle.LoadFromMemory(TextureHelper.GetResourceBytes(name, CurrentAssembly));
            }
            catch
            {
                return null;
            }
        }

        public static AssetCollection LoadAllAssets()
        {
            return new(ASSETPATH);
        }

        public static bool IsFromSideDeck(this PlayableCard card)
        {
            return card.GetComponent<CardFromSideDeck>();
        }

        public static bool IsFromMainDeck(this PlayableCard card)
        {
            return card.GetComponent<CardFromMainDeck>();
        }

        public static CardInfo TryGetCard(string name)
        {
            try
            {
                return CardLoader.GetCardByName(name);
            }
            catch
            {
                try
                {
                    return CardLoader.GetCardByName(PREFIX + "_" + name);
                }
                catch
                {
                    Debug.LogError("Unable to get card: " + name);
                    return null;
                }
            }
        }

        public static bool IsRareCard(this CardInfo c)
        {
            return c?.metaCategories != null && c.metaCategories.Contains(CardMetaCategory.Rare);
        }

        public static IEnumerator Explode(CardSlot slot, int damage = 10, bool bombOpposing = true)
        {
            List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot);
            if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slot.Index)
            {
                if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
                {
                    yield return BombCard(adjacentSlots[0].Card, slot, damage);
                }
                adjacentSlots.RemoveAt(0);
            }
            if (bombOpposing && slot.opposingSlot.Card != null && !slot.opposingSlot.Card.Dead)
            {
                yield return BombCard(slot.opposingSlot.Card, slot, damage);
            }
            if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
            {
                yield return BombCard(adjacentSlots[0].Card, slot, damage);
            }
            yield break;
        }

        public static CardInfo GetRandomChoosableRareCardWithCost(int randomSeed, int bloodCost)
        {
            List<CardInfo> list = CardLoader.GetUnlockedCards(CardMetaCategory.Rare, CardTemple.Nature).FindAll((CardInfo x) => x.BloodCost == bloodCost);
            if (list.Count == 0)
            {
                return null;
            }
            return CardLoader.Clone(list[SeededRandom.Range(0, list.Count, randomSeed)]);
        }

        public static CardInfo GetRandomChoosableRareCardOfTribe(int randomSeed, Tribe tribe)
        {
            List<CardInfo> list = CardLoader.GetUnlockedCards(CardMetaCategory.Rare, CardTemple.Nature).FindAll((CardInfo x) => x.IsOfTribe(tribe));
            if (list.Count == 0)
            {
                return null;
            }
            return CardLoader.Clone(list[SeededRandom.Range(0, list.Count, randomSeed)]);
        }

        public static List<CardSlot> OpponentSlotsCopy => BoardManager.Instance?.OpponentSlotsCopy ?? new();
        public static List<CardSlot> PlayerSlotsCopy => BoardManager.Instance?.PlayerSlotsCopy ?? new();
        public static List<CardSlot> AllSlotsCopy => BoardManager.Instance?.AllSlotsCopy ?? new();
        public static BoardManager Board => BoardManager.Instance;
        public static TurnManager Game => TurnManager.Instance;

        public static List<PlayableCard> GetAliveCards(List<CardSlot> slots)
        {
            return slots.FindAll(x => x.Card != null && !x.Card.Dead).ConvertAll(x => x.Card);
        }

        public static List<CardSlot> GetEmptySlots(List<CardSlot> slots)
        {
            return slots.FindAll(x => x.Card == null || x.Card.Dead);
        }

        public static List<CardSlot> GetFilledSlots(List<CardSlot> slots)
        {
            return slots.FindAll(x => x.Card != null && !x.Card.Dead);
        }

        public static CardInfo GetRandomChoosableRareBonesCard(int randomSeed)
        {
            List<CardInfo> list = CardLoader.GetUnlockedCards(CardMetaCategory.Rare, CardTemple.Nature).FindAll((CardInfo x) => x.BonesCost > 0);
            if (list.Count == 0)
            {
                return null;
            }
            return CardLoader.Clone(list[SeededRandom.Range(0, list.Count, randomSeed)]);
        }

        public static EncounterBlueprintData.CardBlueprint BuildCardBlueprint(string card)
        {
            return new()
            {
                card = TryGetCard(card),
                difficultyReplace = false,
                difficultyReq = 0,
                maxDifficulty = 1000,
                minDifficulty = 0,
                randomReplaceChance = 0,
                replacement = null
            };
        }

        public static void GlitchModelDeactivate(Transform parent, bool onlyGlitchParent = false, bool doShake = true)
        {
            if (parent != null)
            {
                GlitchOutAssetEffect.PlayGlitchSound(parent.position);
                GlitchOutAssetEffect.SwitchModelToGlitchMaterial(parent, onlyGlitchParent);
                if (doShake)
                {
                    Tween.Shake(parent, parent.localPosition, Vector3.one * 0.2f, 0.1f, 0f, Tween.LoopType.Loop, null, null, false);
                }
                GlitchOutAssetEffect.ShowDeletionInUI(parent);
                CustomCoroutine.WaitThenExecute(0.2f, delegate
                {
                    if (parent != null && parent != null)
                    {
                        parent.gameObject.SetActive(false);
                    }
                }, true);
            }
        }

        public static IEnumerator BombCard(PlayableCard target, CardSlot attacker, int damage = 10)
        {
            PlayableCard possibleAttacker = attacker.Card;
            GameObject bomb = Object.Instantiate(detonatorBomb).ApplyTransparent();
            bomb.transform.position = attacker.transform.position + Vector3.up * 0.1f;
            Tween.Position(bomb.transform, target.transform.position + Vector3.up * 0.1f, 0.5f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
            yield return new WaitForSeconds(0.5f);
            target.Anim.PlayHitAnimation();
            Object.Destroy(bomb);
            yield return target.TakeDamage(damage, possibleAttacker);
            yield break;
        }

        public static GameObject ApplyTransparent(this GameObject go)
        {
            foreach(Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                r.material = new(royalCannonTransparent);
            }
            return go;
        }

        public static T MaxElement<T>(this IEnumerable<T> self, Func<T, float> selector)
        {
            T max = default;
            float? highest = null;
            foreach(T e in self)
            {
                if(e != null && !e.Equals(max))
                {
                    float value = selector(e);
                    if(!highest.HasValue || value > highest)
                    {
                        max = e;
                        highest = value;
                    }
                }
            }
            return max;
        }

        public static List<T> SortedCopy<T>(this List<T> list, Comparison<T> comparison)
        {
            List<T> ret = new(list);
            ret.Sort(comparison);
            return ret;
        }

        public static IEnumerator DealDamageNoKill(int damage, bool toplayer)
        {
            var life = LifeManager.Instance;
            if(life == null)
            {
                yield break;
            }
            if (damage < 0)
            {
                toplayer = !toplayer;
            }
            int currentbalance = life.Balance;
            if (toplayer)
            {
                currentbalance = -currentbalance;
            }
            var dam = Mathf.Min(damage, Mathf.Max(4 - currentbalance, 0));
            if(dam <= 0)
            {
                yield break;
            }
            yield return life.ShowDamageSequence(dam, dam, toplayer);
            yield break;
        }

        public static List<CardSlot> GetEmptyQueueSlots()
        {
            if(TurnManager.Instance?.Opponent?.queuedCards == null)
            {
                return new();
            }
            List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
            opponentSlotsCopy.RemoveAll((CardSlot x) => TurnManager.Instance.Opponent.queuedCards.Find((PlayableCard y) => y.QueuedSlot == x));
            return opponentSlotsCopy;
        }

        public static CardModificationInfo CreateRandomizedAbilitiesStatsModSeeded(int randomSeed, List<AbilityInfo> validAbilities, int totalStatPoints, int minAttack, int minHealth)
        {
            CardModificationInfo cardModificationInfo = new();
            int availableStatPoints = totalStatPoints - minAttack * 2 - minHealth;
            validAbilities.RemoveAll((AbilityInfo x) => x.powerLevel > availableStatPoints);
            float num = 0.25f;
            while (validAbilities.Count > 0 && SeededRandom.Value(randomSeed++) > num)
            {
                AbilityInfo abilityInfo = validAbilities[SeededRandom.Range(0, validAbilities.Count, randomSeed++)];
                cardModificationInfo.abilities.Add(abilityInfo.ability);
                availableStatPoints -= abilityInfo.powerLevel;
                validAbilities.Remove(abilityInfo);
                validAbilities.RemoveAll(x => x.powerLevel > availableStatPoints);
                num += 0.25f;
            }
            int num2 = SeededRandom.Range(0, availableStatPoints + 1, randomSeed++);
            availableStatPoints -= num2;
            int num3 = availableStatPoints / 2;
            cardModificationInfo.attackAdjustment = minAttack + num3;
            cardModificationInfo.healthAdjustment = minHealth + num2;
            return cardModificationInfo;
        }

        public static void SkipNextPlayerTurn()
        {
            var hold = TurnManager.Instance?.Opponent?.GetOrAddComponent<SkipNextPlayerTurnHolder>();
            if(hold != null)
            {
                hold.skipNextPlayerTurn = true;
            }
        }

        public static void UnskipNextPlayerTurn()
        {
            var hold = TurnManager.Instance?.Opponent?.GetComponent<SkipNextPlayerTurnHolder>();
            if (hold != null)
            {
                hold.skipNextPlayerTurn = false;
            }
        }

        public static bool DoSkipPlayerTurn()
        {
            return (TurnManager.Instance?.Opponent?.GetComponent<SkipNextPlayerTurnHolder>()?.skipNextPlayerTurn).GetValueOrDefault();
        }

        private class SkipNextPlayerTurnHolder : MonoBehaviour
        {
            public bool skipNextPlayerTurn;
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go?.GetComponent<T>() ?? go?.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.GetOrAddComponent<T>();
        }

        public static T AddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.AddComponent<T>();
        }

        public static Ability FindFromContext()
        {
            return FindRegisteredAbility(new StackTrace().GetFrames().Skip(1).FirstOrDefault()?.GetMethod()?.DeclaringType?.Name);
        }

        public static Ability FindRegisteredAbility<T>() where T : AbilityBehaviour
        {
            return FindRegisteredAbility(typeof(T)?.Name);
        }

        public static Ability FindRegisteredAbility(string name)
        {
            if (!string.IsNullOrEmpty(name) && registeredAbilities.TryGetValue(name, out var ab))
            {
                return ab;
            }
            return Ability.None;
        }

        public static AbilityInfo RegisterAbility(this AbilityInfo info)
        {
            string name = info?.name;
            if (!string.IsNullOrEmpty(name) && !registeredAbilities.ContainsKey(name))
            {
                registeredAbilities.Add(name, info.ability);
            }
            return info;
        }

        public static AbilityInfo NewAbility(string rulebookName, string rulebookDescription, Type behavior, string tex)
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = rulebookName;
            info.rulebookDescription = rulebookDescription;
            info.name = behavior?.Name ?? "";
            info.opponentUsable = true;
            AbilityManager.Add(GUID, info, behavior, LoadTexture(tex));
            info.RegisterAbility();
            return info;
        }

        public static AbilityInfo SetColor(this AbilityInfo info, Color c)
        {
            info.hasColorOverride = true;
            info.colorOverride = c;
            return info;
        }

        public static AbilityInfo SetOpponentUnusable(this AbilityInfo info)
        {
            info.opponentUsable = false;
            return info;
        }

        public static AbilityInfo SetPower(this AbilityInfo info, int power)
        {
            info.powerLevel = power;
            return info;
        }

        public static AbilityInfo SetPart1Rulebook(this AbilityInfo info)
        {
            info.AddMetaCategories(AbilityMetaCategory.Part1Rulebook);
            return info;
        }

        public static AbilityInfo SetPart3Rulebook(this AbilityInfo info)
        {
            info.AddMetaCategories(AbilityMetaCategory.Part3Rulebook);
            return info;
        }

        public static AbilityInfo SetCurse(this AbilityInfo info, int effect)
        {
            return info
                .SetPart1Rulebook()
                .SetColor(ColorCurse)
                .SetPower(-effect)
                .AddMetaCategories(CurseMetacategory);
        }

        public static AbilityInfo SetColor(this AbilityInfo info, byte r, byte g, byte b, byte a = 255)
        {
            return info.SetColor(new Color32(r, g, b, a));
        }

        public static T RandomElement<T>(this IList<T> l, int? seed = null)
        {
            if(seed == null)
            {
                return l[Random.Range(0, l.Count)];
            }
            else
            {
                return l[SeededRandom.Range(0, l.Count, seed.GetValueOrDefault())];
            }
        }

        [HarmonyPatch(typeof(AudioController), nameof(AudioController.GetAudioClip))]
        [HarmonyPrefix]
        public static void AddSFX(AudioController __instance)
        {
            __instance.SFX.AddRange(assets.Audio.Where(x => !__instance.SFX.Contains(x)));
        }

        [HarmonyPatch(typeof(AudioController), nameof(AudioController.GetLoop))]
        [HarmonyPrefix]
        public static void AddLoops1(AudioController __instance)
        {
            __instance.Loops.AddRange(assets.Audio.Where(x => !__instance.SFX.Contains(x)));
        }

        [HarmonyPatch(typeof(AudioController), nameof(AudioController.GetLoopClip))]
        [HarmonyPrefix]
        public static void AddLoops2(AudioController __instance)
        {
            __instance.Loops.AddRange(assets.Audio.Where(x => !__instance.SFX.Contains(x)));
        }

        public static readonly Dictionary<string, Ability> registeredAbilities = new();
        public static GameObject royalCrosshair = Resources.Load<GameObject>("Prefabs/Cards/SpecificCardModels/CannonTargetIcon");
        public static Material royalCannonTransparent = new(Resources.Load<GameObject>("Prefabs/Cards/SpecificCardModels/CannonTargetIcon").GetComponentInChildren<Renderer>().material);
        public static GameObject detonatorBomb = Resources.Load<GameObject>("Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb");
    }
}
