using DiskCardGame;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;

namespace SquirrelBombMod.Spamton
{
    [HarmonyPatch]
    public class SpamtonBossOpponent : Part1BossOpponent
    {
        public GameObject sneoAnimation;
        public PlayableCard heartCard;
        public GameObject chain;
        private readonly float chainScale = 0.3f;
        public bool lerpingOutChain;
        public bool createdHeart;
        public int stallPhaseStartTurn;
        public bool hasStartedStallPhase;
        public bool isSpecialRoyalChallenge;
        public GameObject royalSkull;
        public static GameObject pipisBomb = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb");
        public static int LastCachedDamage;

        public override string DefeatedPlayerDialogue => "I'M [STEALING] YOUR KROMER";
        public override Speaker DefeatedPlayerDialogueSpeaker => SpamtonSetup.spamtonSpeaker;
        public override bool GiveCurrencyOnDefeat => false;
        public override int StartingLives => 7;
        public bool BearChallengeActive => AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.GrizzlyMode);

        public override IEnumerator QueueNewCards(bool doTween = true, bool changeView = true)
        {
            if(NumLives <= 2)
                yield break;

            if(heartCard == null)
            {
                yield return base.QueueNewCards(doTween, changeView);
                yield break;
            }

            var list = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
            list.RemoveAll(x => Queue.Exists((PlayableCard y) => y.QueuedSlot == x));

            if (list.Count <= 0)
                yield break;

            var cardId = "morebosses_FlyingHead";
            if (heartCard.Health < heartCard.MaxHealth)
                cardId = "morebosses_Spamgel";
            yield return QueueCard(CardLoader.GetCardByName(cardId), list[SeededRandom.Range(0, list.Count, SaveManager.SaveFile.GetCurrentRandomSeed() + TurnManager.Instance.TurnNumber)], doTween, changeView, true);
        }

        public override IEnumerator IntroSequence(EncounterData encounter)
        {
            AudioController.Instance.FadeOutLoop(0.75f, []);
            yield return base.IntroSequence(encounter);

            yield return new WaitForSeconds(0.75f);
            Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, true);

            yield return new WaitForSeconds(3f);
            AudioController.Instance.PlaySound2D("car_honk", MixerGroup.TableObjectsSFX);
            yield return new WaitForSeconds(1.25f);
            AudioController.Instance.PlaySound2D("car_screech", MixerGroup.TableObjectsSFX);
            yield return new WaitForSeconds(0.75f);
            AudioController.Instance.PlaySound2D("car_crash", MixerGroup.TableObjectsSFX);

            LeshyAnimationController.Instance.SetHeadOffset(new Vector3(0f, 0f, -20f), false);
            Tween.Position(bossSkull.transform, bossSkull.transform.position + new Vector3(0f, 0f, -20f), 0.25f, 0f, Tween.EaseInOut);
            yield return new WaitForSeconds(0.75f);

            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoPreIntro", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);

            sneoAnimation = Instantiate(SpamtonSetup.sneoPrefab, new Vector3(0.3f, 7.15f, 5.6f), Quaternion.identity);
            sneoAnimation.transform.localScale = Vector3.one * 6f;
            var renderers = sneoAnimation.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var rend in renderers)
                rend.color = Color.black;

            var ela = 0f;
            var appearDuration = 1.5f;
            var target = 0.7f;
            var targetColor = new Color(target, target, target);
            while(ela < appearDuration)
            {
                foreach(var rend in renderers)
                {
                    rend.color = Color.Lerp(Color.black, targetColor, ela / appearDuration);
                }
                ela += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            foreach (var rend in renderers)
                rend.color = targetColor;

            yield return new WaitForSeconds(0.5f);
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoFinalIntro1", TextDisplayer.MessageAdvanceMode.Input);

            yield return new WaitForSeconds(0.25f);
            ExplorableAreaManager.Instance.TweenHangingLightColors(new Color32(126, 126, 204, 255), new Color32(126, 126, 204, 255), 0.5f);
            AudioController.Instance.SetLoopAndPlay("battle");
            AudioController.Instance.SetLoopVolumeImmediate(0.5f, 0);
            yield break;
        }

        public override IEnumerator StartNewPhaseSequence()
        {
            if(NumLives <= 1 && !hasStartedStallPhase)
            {
                yield return StartEndingPhase();
                yield break;
            }

            // This is REALLY evil but it works
            yield return NumLives switch
            {
                6 => StartPhase2Sequence(),
                5 => StartPhase3Sequence(),
                4 => StartPhase4Sequence(),
                3 => StartPhase5Sequence(),
                2 => StartGiantCardPhase(),

                _ => null
            };
        }

        public override IEnumerator LifeLostSequence()
        {
            if (!isSpecialRoyalChallenge)
            {
                NumLives = Mathf.Max(NumLives, 1);
                yield break;
            }

            if (NumLives <= 0)
            {
                AudioController.Instance.SetLoopVolume(0f, 1f, 0, true);
                AudioController.Instance.SetLoopVolume(0f, 1f, 1, true);
                yield return new WaitForSeconds(0.4f);

                Singleton<ViewManager>.Instance.SwitchToView(View.BossCloseup, false, true);
                yield return new WaitForSeconds(0.4f);
                yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("Avast ye!", -2.5f, 0.5f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.PirateSkull, null, true);

                yield return new WaitForSeconds(0.2f);
                royalSkull.GetComponentInChildren<Animator>().SetTrigger("close_eye");
                yield return new WaitForSeconds(1.25f);
                yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("Farewell.", -2.5f, 0.5f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.PirateSkull, null, true);

                yield return new WaitForSeconds(0.1f);
                Singleton<InteractionCursor>.Instance.SetHidden(true);
                Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(Color.black);
                Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, 3f);

                yield return new WaitForSeconds(0.8f);
                EndAscensionRun();
                SceneLoader.Load("Ascension_Credits");
            }
        }

        public IEnumerator StartPhase2Sequence()
        {
            yield return new WaitForSeconds(0.25f);
            yield return ClearQueue();

            yield return new WaitForSeconds(0.25f);
            if (ViewManager.Instance.CurrentView != View.Board)
            {
                ViewManager.Instance.SwitchToView(View.Board);
                yield return new WaitForSeconds(0.25f);
            }

            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoPhase2", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);

            var slot1 = BoardManager.Instance.OpponentSlotsCopy[0];
            if (slot1.Card != null && !slot1.Card.Dead)
                yield return slot1.Card.Die(false);

            var slot2 = BoardManager.Instance.OpponentSlotsCopy[1];
            if (slot2.Card != null && !slot2.Card.Dead)
                yield return slot2.Card.Die(false);

            yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName(BearChallengeActive ? "morebosses_BearCart" : "morebosses_SpamCart"), slot1);
            yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName("morebosses_SpamMail"), slot2);
            yield return new WaitForSeconds(0.25f);
            yield return ReplaceBlueprint(SpamtonSetup.spamP2);
        }

        public IEnumerator StartPhase3Sequence()
        {
            yield return new WaitForSeconds(0.25f);
            yield return ClearQueue();
            yield return ClearBoard();

            var spamCarts = BoardManager.Instance.CardsOnBoard.FindAll(x => x != null && !x.Dead && x.Info.name == "morebosses_SpamCart" || x.Info.name == "morebosses_BearCart");
            foreach(PlayableCard spam in spamCarts)
            {
                if (spam == null || spam.Dead || !spam.OnBoard)
                    continue;

                yield return spam.Die(false);
            }

            yield return new WaitForSeconds(0.25f);
            if (ViewManager.Instance.CurrentView != View.Board)
            {
                ViewManager.Instance.SwitchToView(View.Board);
                yield return new WaitForSeconds(0.25f);
            }

            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoPhase3", TextDisplayer.MessageAdvanceMode.Input);
            var middleSlot = BoardManager.Instance.OpponentSlotsCopy[1];
            chain = new("chain parent");
            chain.transform.parent = sneoAnimation.transform;
            chain.transform.localPosition = Vector3.forward * 0.01f;
            chain.transform.localScale = Vector3.zero;
            var childCount = 10;
            for(var i = 0; i < childCount; i++)
            {
                var chainPiece = new GameObject("chain piece " + i);
                chainPiece.transform.parent = chain.transform;
                chainPiece.transform.localPosition = Vector3.zero;
                chainPiece.transform.localScale = Vector3.one;
                var sprite = chainPiece.AddComponent<SpriteRenderer>();
                sprite.sprite = SpamtonSetup.chainSprite;
            }

            yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName(BearChallengeActive ? "morebosses_BearHeart" : "morebosses_ChainHeart"), middleSlot);
            heartCard = middleSlot.Card;
            createdHeart = true;

            Tween.LocalScale(chain.transform, Vector3.one * chainScale, 0.25f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
            yield return QueueNewCards(true, true);
        }

        public IEnumerator StartPhase4Sequence()
        {
            yield return new WaitForSeconds(0.25f);
            if (ViewManager.Instance.CurrentView != View.Default)
            {
                ViewManager.Instance.SwitchToView(View.Default);
                yield return new WaitForSeconds(0.25f);
            }

            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoPhase4", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);

            Tween.LocalScale(sneoAnimation.transform.Find("hand2attachPoint").Find("hand2"), new Vector3(0f, 1f, 1f), 0.25f, 0f, Tween.EaseIn, Tween.LoopType.None, null, () =>
            {
                sneoAnimation.transform.Find("hand2attachPoint").Find("hand2").localScale = Vector3.one;
                sneoAnimation.transform.Find("hand2attachPoint").Find("hand2").gameObject.SetActive(false);
            }, true);

            sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").localScale = new Vector3(0f, 1f, 1f);
            Tween.LocalScale(sneoAnimation.transform.Find("hand2attachPoint").Find("cannon"), Vector3.one, 0.25f, 0.25f, Tween.EaseOut, Tween.LoopType.None, () =>
            {
                sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").gameObject.SetActive(true);
            }, null, true);

            yield return new WaitForSeconds(0.75f);
            var slots = BoardManager.Instance.OpponentSlotsCopy.FindAll(x => (x.Card == null || x.Card.Dead) && x.opposingSlot != null && x.opposingSlot.Card != null && !x.opposingSlot.Card.Dead);
            if(slots.Count <= 0)
            {
                slots = BoardManager.Instance.OpponentSlotsCopy.FindAll(x => x.opposingSlot != null && x.opposingSlot.Card != null && !x.opposingSlot.Card.Dead);
                if(slots.Count <= 0)
                {
                    slots = BoardManager.Instance.OpponentSlotsCopy.FindAll(x => x.Card == null || x.Card.Dead);
                    if(slots.Count <= 0)
                    {
                        slots = BoardManager.Instance.OpponentSlotsCopy;
                    }
                }
            }
            if(slots.Count > 0)
            {
                var randomslot = slots[SeededRandom.Range(0, slots.Count, SaveManager.SaveFile.GetCurrentRandomSeed() + TurnManager.Instance.TurnNumber * 100)];
                if(randomslot != null)
                {
                    if(randomslot.Card != null && !randomslot.Card.Dead)
                    {
                        yield return randomslot.Card.Die(false);
                    }
                    yield return SpawnPipisInSlot(randomslot, true);
                }
            }

            yield return ReplaceBlueprint(SpamtonSetup.spamP1);
        }

        public IEnumerator SpawnPipisInSlot(CardSlot slot, bool killImmediately = false, bool resetAim = true)
        {
            AimCannon(slot.transform.position, !resetAim);
            yield return new WaitForSeconds(resetAim ? 0.7f : 0.5f);

            var bomb = Instantiate(pipisBomb);
            bomb.transform.localScale = Vector3.zero;
            bomb.GetComponentInChildren<Animator>().speed = resetAim ? 1 : 2;
            Tween.LocalScale(bomb.transform, Vector3.one, 0.1f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
            bomb.transform.position = sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").transform.position;
            AudioController.Instance.PlaySound3D("cannonfire", MixerGroup.TableObjectsSFX, bomb.transform.position, 2.5f);
            Tween.Position(bomb.transform, slot.transform.position, resetAim ? 0.5f : 0.25f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
            Tween.LocalScale(bomb.transform, Vector3.zero, 0.1f, resetAim ? 0.5f : 0.25f, Tween.EaseIn, Tween.LoopType.None, null, () => Destroy(bomb), true);
            yield return new WaitForSeconds(0.5f);

            if ((slot != null && slot.Card == null) || slot.Card.Dead)
            {
                var playableCard = CardSpawner.SpawnPlayableCard(CardLoader.GetCardByName(BearChallengeActive && resetAim ? "morebosses_BearPipis" : "morebosses_Pipis"));
                playableCard.transform.localScale = Vector3.zero;
                Tween.LocalScale(playableCard.transform, Vector3.one, 0.15f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
                if (!slot.IsPlayerSlot)
                {
                    playableCard.SetIsOpponentCard(true);
                    Singleton<TurnManager>.Instance.Opponent.ModifySpawnedCard(playableCard);
                }
                playableCard.Anim.PlayRiffleSound();

                yield return BoardManager.Instance.TransitionAndResolveCreatedCard(playableCard, slot, 0f, true);
                if (killImmediately)
                {
                    yield return new WaitForSeconds(0.5f);
                    yield return playableCard.Die(false);
                }
            }

            if (!resetAim)
                yield break;

            ResetCannonAim();
            yield return new WaitForSeconds(0.25f);
        }

        public IEnumerator StartPhase5Sequence()
        {
            yield return new WaitForSeconds(0.25f);
            yield return ClearBoard();

            yield return new WaitForSeconds(0.25f);
            if (ViewManager.Instance.CurrentView != View.Default)
            {
                ViewManager.Instance.SwitchToView(View.Default);
                yield return new WaitForSeconds(0.25f);
            }

            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoPhase5", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);

            yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName(BearChallengeActive ? "morebosses_HandBearLeft" : "morebosses_HandPhoneLeft"), BoardManager.Instance.OpponentSlotsCopy[0]);
            yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName(BearChallengeActive ? "morebosses_BearMail" : "morebosses_SpamMail"), BoardManager.Instance.OpponentSlotsCopy[1]);
            yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName(BearChallengeActive ? "morebosses_HandBearRight" : "morebosses_HandPhoneRight"), BoardManager.Instance.OpponentSlotsCopy[3]);
            yield return ReplaceBlueprint(SpamtonSetup.spamP2);
        }

        public IEnumerator ShootSlot(CardSlot slot, bool instantDeath = false)
        {
            AimCannon(slot.transform.position);
            sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").Find("chargeflash").gameObject.SetActive(true);
            Tween.LocalScale(sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").Find("chargeflash"), Vector3.one * 0.2f, 0.5f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
            yield return new WaitForSeconds(0.7f);

            Tween.LocalScale(sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").Find("chargeflash"), Vector3.zero, 0.15f, 0f, Tween.EaseOut, Tween.LoopType.None, () => 
                sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").Find("chargeflash").gameObject.SetActive(false), null, true);

            var bomb = Instantiate(sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").Find("chargeflash").gameObject);
            bomb.transform.parent = null;
            bomb.transform.localScale = Vector3.one;
            bomb.transform.position = sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").transform.position;
            AudioController.Instance.PlaySound3D("cannonfire", MixerGroup.TableObjectsSFX, bomb.transform.position, 2.5f);
            Tween.Position(bomb.transform, slot.transform.position, 0.5f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
            Tween.LocalScale(bomb.transform, Vector3.zero, 0.1f, 0.5f, Tween.EaseIn, Tween.LoopType.None, null, () => Destroy(bomb), true);
            yield return new WaitForSeconds(0.5f);

            if(slot.Card != null)
            {
                if (instantDeath)
                    yield return slot.Card.Die(false, null, true);
                else
                    yield return slot.Card.TakeDamage(10, null);
            }

            ResetCannonAim();
            yield return new WaitForSeconds(0.25f);
        }

        public IEnumerator StartGiantCardPhase()
        {
            yield return new WaitForSeconds(0.25f);
            yield return ClearBoard();
            yield return ClearQueue();

            yield return new WaitForSeconds(0.25f);
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoPhase6", TextDisplayer.MessageAdvanceMode.Input);

            yield return new WaitForSeconds(0.25f);
            yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("morebosses_!GIANTCARD_NEO"), Singleton<BoardManager>.Instance.OpponentSlotsCopy[0], 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            AudioController.Instance.PlaySound3D("map_slam", MixerGroup.TableObjectsSFX, Singleton<BoardManager>.Instance.transform.position, 1f, 0f, null, null, null, null, false);

            yield return new WaitForSeconds(0.25f);
            Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
            yield return new WaitForSeconds(0.75f);
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
            yield return new WaitForSeconds(0.25f);

            var bigshotslots = new List<CardSlot>();
            var neoCard = (BoardManager.Instance.OpponentSlotsCopy[0].Card?.transform ?? BoardManager.Instance.OpponentSlotsCopy[0].transform).position;

            NeoAnimatedPortrait neo = null;
            if (BoardManager.Instance.OpponentSlotsCopy[0].Card != null)
                neo = Singleton<CardRenderCamera>.Instance.GetLiveRenderCamera(BoardManager.Instance.OpponentSlotsCopy[0].Card.StatsLayer as RenderLiveStatsLayer).GetComponentInChildren<NeoAnimatedPortrait>();
            if(neo != null)
                neo.OpenMouth();

            foreach (var slot in BoardManager.Instance.PlayerSlotsCopy)
            {
                if (slot.Card == null)
                    continue;

                bigshotslots.Add(slot);
                var card = slot.Card;
                card.UnassignFromSlot();
                card.Anim.PlayRiffleSound();
                Tween.LocalScale(card.transform, Vector3.zero, 0.5f, 0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
                Tween.Position(card.transform, neoCard + Vector3.up * 0.05f + Vector3.right * 2.05f + Vector3.forward, 0.5f, 0f, Tween.EaseIn, Tween.LoopType.None, null, () => Destroy(card.gameObject), true);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.75f);

            if(neo != null)
                neo.CloseMouth();

            if (bigshotslots.Count > 1)
            {
                foreach (CardSlot slot in bigshotslots)
                {
                    yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName("morebosses_BigShooter"), slot, 0.1f, true);
                    yield return new WaitForSeconds(0.05f);
                }
                yield return ShootSlot(bigshotslots[SeededRandom.Range(0, bigshotslots.Count, SaveManager.SaveFile.GetCurrentRandomSeed() + TurnManager.Instance.TurnNumber * 1000)]);
            }

            yield return new WaitForSeconds(0.25f);
        }

        public IEnumerator StartEndingPhase()
        {
            hasStartedStallPhase = true;
            stallPhaseStartTurn = TurnManager.Instance.TurnNumber + 1;
            AudioController.Instance.FadeOutLoop(0.5f, 0);
            yield return new WaitForSeconds(0.25f);

            yield return ClearBoard();
            yield return ClearQueue();
            yield return new WaitForSeconds(0.25f);

            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoEndingPhase", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);

            yield return new WaitForSeconds(0.25f);
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(GameColors.Instance.blue);
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, 100f);

            yield return new WaitForSeconds(0.1f);
            Singleton<TableVisualEffectsManager>.Instance.ChangeTableColors(GameColors.Instance.blue, GameColors.Instance.blue, GameColors.Instance.blue, GameColors.Instance.darkBlue, GameColors.Instance.blue, GameColors.Instance.brightBlue, GameColors.Instance.blue, GameColors.Instance.brightBlue, GameColors.Instance.glowSeafoam);
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(0f, 6f);

            yield return new WaitForSeconds(0.5f);
            AudioController.Instance.SetLoopAndPlay("lastphase");
            AudioController.Instance.SetLoopVolumeImmediate(0.5f, 0);
        }

        public void AimCannon(Vector3 point, bool quick = false)
        {
            var cannon = sneoAnimation.transform.Find("hand2attachPoint").Find("cannon");
            var before = cannon.rotation;
            cannon.LookAt(point);
            cannon.forward = -cannon.forward;
            var target = cannon.rotation;
            cannon.rotation = before;

            Tween.Rotation(cannon, Quaternion.Euler(target.eulerAngles + new Vector3(75f, 0f, 0f)), quick ? 0.15f : 0.5f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
        }

        public void ResetCannonAim()
        {
            Tween.Rotation(sneoAnimation.transform.Find("hand2attachPoint").Find("cannon"), Quaternion.identity, 0.5f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
        }

        public override void ManagedLateUpdate()
        {
            if (chain == null)
                return;

            if (heartCard != null)
            {
                var camera = Camera.main.transform;
                var chainCount = chain.transform.childCount;
                for (int i = 0; i < chainCount; i++)
                {
                    var child = chain.transform.GetChild(i);
                    child.transform.position = Vector3.Lerp(chain.transform.position, Vector3.Lerp(chain.transform.position, heartCard.transform.position, (float)i / chainCount), child.transform.localScale.x / chainScale);
                    child.transform.LookAt(camera);
                }
            }
            else if (!lerpingOutChain && createdHeart)
            {
                lerpingOutChain = true;
                Tween.LocalScale(chain.transform, Vector3.zero, 0.5f, 0f, Tween.EaseOut, Tween.LoopType.None, null, () => Destroy(chain), true);
            }
        }

        public IEnumerator ReplaceBlueprint(EncounterBlueprintData blueprint, bool removeLockedCards = false)
        {
            Blueprint = blueprint;
            var difficulty = 0;
            if (Singleton<TurnManager>.Instance.BattleNodeData != null)
                difficulty = Singleton<TurnManager>.Instance.BattleNodeData.difficulty + RunState.Run.DifficultyModifier;

            var plan = EncounterBuilder.BuildOpponentTurnPlan(Blueprint, difficulty, removeLockedCards);
            ReplaceAndAppendTurnPlan(plan);
            yield return QueueNewCards(true, true);
        }

        [HarmonyPatch(typeof(LifeManager), nameof(LifeManager.ShowDamageSequence))]
        [HarmonyPrefix]
        public static void ReduceDamage(ref int damage, ref int numWeights, bool toPlayer)
        {
            if (toPlayer || (TurnManager.Instance?.Opponent) == null || TurnManager.Instance.Opponent is not SpamtonBossOpponent || TurnManager.Instance.Opponent.NumLives > 1 ||
                (TurnManager.Instance.Opponent as SpamtonBossOpponent).isSpecialRoyalChallenge)
                return;

            LastCachedDamage = damage;
            damage = Mathf.Min(1, damage);
            numWeights = Mathf.Min(1, numWeights);
        }

        [HarmonyPatch(typeof(LifeManager), nameof(LifeManager.ShowDamageSequence))]
        [HarmonyPostfix]
        public static IEnumerator PreventDirectDamage(IEnumerator res, LifeManager __instance, int damage, bool toPlayer)
        {
            if(TurnManager.Instance == null || TurnManager.Instance.Opponent is not SpamtonBossOpponent spamtonOpponent || spamtonOpponent == null || spamtonOpponent.isSpecialRoyalChallenge)
            {
                yield return res;
                yield break;
            }

            if (!toPlayer && spamtonOpponent.heartCard != null && !spamtonOpponent.heartCard.Dead)
                yield break;

            yield return res;

            if (spamtonOpponent.NumLives > 1)
                yield break;

            if (!toPlayer && damage < LastCachedDamage && !DialogueEventsData.EventIsPlayed("SneoEnding1Damage"))
                yield return TextDisplayer.Instance.PlayDialogueEvent("SneoEnding1Damage", TextDisplayer.MessageAdvanceMode.Input);
            if (__instance.Balance >= 4)
                yield return __instance.ShowResetSequence();
        }

        [HarmonyPatch(typeof(Deck), nameof(Deck.Draw), typeof(CardInfo))]
        [HarmonyPostfix]
        public static void ReplaceFinalCard(Deck __instance, ref CardInfo __result)
        {
            var piles = Singleton<CardDrawPiles3D>.Instance;
            if (piles == null)
                return;
            if (__instance != piles.SideDeck && __instance != piles.Deck)
                return;
            if (TurnManager.Instance == null || TurnManager.Instance.Opponent is not SpamtonBossOpponent spamtonOpponent || spamtonOpponent == null)
                return;
            if (spamtonOpponent.NumLives > 1 || spamtonOpponent.isSpecialRoyalChallenge)
                return;
            if (piles.SideDeck != null && piles.SideDeck.CardsInDeck > 0)
                return;
            if (piles.Deck != null && piles.SideDeck.CardsInDeck > 0)
                return;

            __result = CardLoader.GetCardByName("morebosses_Receiver");
        }

        [HarmonyPatch(typeof(CardDrawPiles), nameof(CardDrawPiles.ExhaustedSequence))]
        [HarmonyPostfix]
        public static IEnumerator OverrideSneoExhausted(IEnumerator res, CardDrawPiles __instance)
        {
            if (TurnManager.Instance == null || TurnManager.Instance.Opponent is not SpamtonBossOpponent spamtonOpponent || spamtonOpponent == null || Singleton<TurnManager>.Instance.Opponent.NumLives > 2)
            {
                yield return res;
                yield break;
            }

            Singleton<ViewManager>.Instance.SwitchToView(View.CardPiles, false, true);
            yield return new WaitForSeconds(1f);

            var moonCardSlot = Singleton<BoardManager>.Instance.GetSlots(false).Find(x => x.Card != null && x.Card.Info.HasTrait(Trait.Giant));
            if (moonCardSlot != null && !spamtonOpponent.isSpecialRoyalChallenge)
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                yield return new WaitForSeconds(0.25f);
                moonCardSlot.Card.AddTemporaryMod(new CardModificationInfo(1, 0));
                moonCardSlot.Card.Anim.StrongNegationEffect();
                yield return new WaitForSeconds(1f);
            }

            Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
            Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
            __instance.turnsSinceExhausted++;
        }
    }
}
