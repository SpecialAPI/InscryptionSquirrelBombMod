using DiskCardGame;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SquirrelBombMod.Spamton
{
    public class SpamtonSpecialSequencer : BossBattleSequencer
    {
        public bool didPhase1Introduction;
        public bool didGiveFreeReceiver;
        public int deathTurnCounter;
        public int receiverCounter;
        public bool playedLongFireworksDialogue;
        public readonly int TurnsUntilDeath = 5;
        public Ability helpCallAbility = FindRegisteredAbility("HelpCall");

        public override Opponent.Type BossType => SpamtonSetup.spamtonBoss;
        public override StoryEvent DefeatedStoryEvent => StoryEvent.NUM_EVENTS;
        public SpamtonBossOpponent Boss => TurnManager.Instance?.Opponent != null && (TurnManager.Instance.Opponent is SpamtonBossOpponent) ? (TurnManager.Instance.Opponent as SpamtonBossOpponent) : null;
        public PlayableCard HeartCard => Boss?.heartCard;
        public GameObject Chain => Boss?.chain;

        public override EncounterData BuildCustomEncounter(CardBattleNodeData nodeData)
        {
            return new()
            {
                opponentType = SpamtonSetup.spamtonBoss,
                Blueprint = SpamtonSetup.spamP1,
                opponentTurnPlan = EncounterBuilder.BuildOpponentTurnPlan(SpamtonSetup.spamP1, 20, true),
                startConditions = new()
                {
                    new()
                    {
                        cardsInPlayerSlots = new CardInfo[]
                        {
                            TryGetCard("BigShooter"),
                            null,
                            null,
                            null
                        }
                    }
                }
            };
        }


        #region Ending
        public bool ShouldIncrementDeathTurnCounter()
        {
            return !BoardManager.Instance.CardsOnBoard.Exists(x => x.HasAbility(helpCallAbility)) && (PlayerHand.Instance.CardsInHand.Exists(x => x.HasAbility(helpCallAbility)) || didGiveFreeReceiver);
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            if (Boss.isSpecialRoyalChallenge)
                return false;

            return playerTurnEnd;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            var receiver = BoardManager.Instance.CardsOnBoard.Find(x => x.HasAbility(helpCallAbility));

            if (receiver != null)
                yield return ReceiverSequence(receiver);

            else if (ShouldIncrementDeathTurnCounter())
            {
                deathTurnCounter++;
                if(deathTurnCounter == TurnsUntilDeath - 1)
                {
                    ViewManager.Instance.SwitchToView(View.Default, false, false);
                    yield return new WaitForSeconds(0.25f);
                    Boss.AimCannon(Vector3.Lerp(BoardManager.Instance.PlayerSlotsCopy[1].transform.position, BoardManager.Instance.PlayerSlotsCopy[2].transform.position, 0.5f) + Vector3.back);
                    yield return TextDisplayer.Instance.PlayDialogueEvent("SneoEndingAlmostDeath", TextDisplayer.MessageAdvanceMode.Input);
                }
                else if(deathTurnCounter >= TurnsUntilDeath)
                {
                    ViewManager.Instance.SwitchToView(View.Default, false, true);
                    yield return new WaitForSeconds(0.5f);
                    Boss.sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").Find("chargeflash").gameObject.SetActive(true);
                    Boss.AimCannon(Vector3.Lerp(BoardManager.Instance.PlayerSlotsCopy[1].transform.position, BoardManager.Instance.PlayerSlotsCopy[2].transform.position, 0.5f) + Vector3.back);
                    Tween.LocalScale(Boss.sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").Find("chargeflash"), Vector3.one * 0.2f, 0.5f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
                    yield return new WaitForSeconds(0.5f);
                    yield return TextDisplayer.Instance.PlayDialogueEvent("SneoEndingDeath", TextDisplayer.MessageAdvanceMode.Input);
                    yield return new WaitForSeconds(0.25f);
                    AudioController.Instance.PlaySound3D("cannonfire", MixerGroup.TableObjectsSFX, Boss.sneoAnimation.transform.Find("hand2attachPoint").Find("cannon").Find("shootPoint").transform.position, 2.5f);
                    Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(Color.white);
                    Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, 6f);
                    yield return new WaitForSeconds(1f);
                    AscensionMenuScreens.ReturningFromFailedRun = true;
                    AscensionStatsData.TryIncrementStat(AscensionStat.Type.Losses);
                    SaveManager.SaveToFile(true);
                    SceneLoader.Load("Ascension_Configure");
                }
            }
            yield break;
        }
        public IEnumerator ReceiverSequence(PlayableCard receiver)
        {
            Boss.ResetCannonAim();
            deathTurnCounter = 0;
            receiver.Anim.StrongNegationEffect();
            receiver.Anim.PlayRiffleSound();
            receiverCounter++;
            yield return new WaitForSeconds(0.5f);
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoReceiver" + receiverCounter, TextDisplayer.MessageAdvanceMode.Input);

            if (receiverCounter < 7)
                yield break;

            yield return EndingSequence(receiver);
        }

        public IEnumerator EndingSequence(PlayableCard receiver)
        {
            Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
            PauseMenu.pausingDisabled = true;
            ViewManager.Instance.SwitchToView(View.Default, false, true);

            yield return new WaitForSeconds(0.25f);
            yield return Boss.ShootSlot(receiver.Slot, true);
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoReceiver8", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.5f);

            AudioController.Instance.FadeOutLoop(0.5f, 0);
            yield return new WaitForSeconds(0.5f);
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoReceiver9", TextDisplayer.MessageAdvanceMode.Input);

            Singleton<CameraEffects>.Instance.SetCameraFarPlane(500f);
            LeshyAnimationController.Instance.gameObject.SetActive(false);
            (Singleton<ExplorableAreaManager>.Instance as CabinManager).SetNorthWallHidden(true);

            var audioSource = AudioController.Instance.PlaySound2D("pirate_ship_charge", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
            audioSource.time = 1f;
            audioSource.volume = 0f;
            AudioController.Instance.FadeSourceVolume(audioSource, 1f, 2f, true);

            yield return new WaitForSeconds(3f);
            GameObject ship = Instantiate(ResourceBank.Get<GameObject>("Prefabs/Environment/TableEffects/PirateShipAnim"));
            yield return new WaitForSeconds(6f);
            Singleton<CameraEffects>.Instance.Shake(0.01f, float.MaxValue);
            yield return new WaitForSeconds(2.5f);
            Singleton<CameraEffects>.Instance.Shake(0.03f, float.MaxValue);
            yield return new WaitForSeconds(1.5f);
            Singleton<CameraEffects>.Instance.Shake(0.25f, float.MaxValue);

            yield return new WaitForSeconds(0.9f);
            ship.GetComponentInChildren<Animator>().speed = 0f;
            ship.GetComponentInChildren<Animator>().enabled = false;

            yield return new WaitForSeconds(0.1f);
            Singleton<CameraEffects>.Instance.StopShake();
            Singleton<CameraEffects>.Instance.transform.localPosition = Vector3.zero;
            Singleton<CameraEffects>.Instance.SetCameraFarPlane(50f);

            LeshyAnimationController.Instance.gameObject.SetActive(true);
            AudioController.Instance.PlaySound2D("defeated", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);

            var root = Boss.sneoAnimation.transform;
            var limbs = new List<Transform>()
            {
                root.Find("leg1"),
                root.Find("leg2"),
                root.Find("hand1"),
                root.Find("hand2attachPoint"),
            };
            Tween.Shake(Singleton<CameraEffects>.Instance.transform, Vector3.zero, new Vector3(0.5f, 0.5f, 0f), 8f, 0f, Tween.LoopType.None, null, null, false);
            Boss.sneoAnimation.GetComponent<Animator>().enabled = false;
            foreach (var t in limbs)
            {
                t.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
                t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, -.03f);
                Tween.Shake(t, t.localPosition, new(0.1f, 0.1f), 12f, 0f, Tween.LoopType.None, null, null, false);
            }
            Tween.Shake(root, root.localPosition, new(0.1f, 0.1f), 12f, 0f, Tween.LoopType.None, null, null, false);

            Time.timeScale = 0.1f;
            TextDisplayer.Instance.ShowMessage("NOOOOOOOOOOOOOOO!!!!!!", speaker: SpamtonSetup.spamtonSpeaker);
            yield return new WaitForSecondsRealtime(6f);
            TextDisplayer.Instance.ShowMessage("DEFEATED BY A WEAKLING HUMAN...", speaker: SpamtonSetup.spamtonSpeaker);

            Tween.Position(Boss.sneoAnimation.transform, Boss.sneoAnimation.transform.position + Vector3.back * 20f + Vector3.up * 1.5f, 2.5f, 0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
            Tween.Rotation(Boss.sneoAnimation.transform, Quaternion.Euler(Boss.sneoAnimation.transform.rotation.eulerAngles.x + 180, Boss.sneoAnimation.transform.rotation.eulerAngles.y + 180, Boss.sneoAnimation.transform.rotation.eulerAngles.z + 180), 2.5f, 0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
            yield return new WaitForSecondsRealtime(4f);
            TextDisplayer.Instance.Clear();

            var ela = 0f;
            while (ela < 1f)
            {
                Time.timeScale = Mathf.Lerp(0.1f, 1f, ela);
                ela += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.FinalBoss))
            {
                yield return RoyalChallengeSequence(ship);
                yield break;
            }

            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(Color.black);
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, 3f);

            yield return new WaitForSecondsRealtime(4.5f);
            Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
            PauseMenu.pausingDisabled = false;
            Boss.EndAscensionRun();
            SceneLoader.Load("Ascension_Credits");
        }

        public IEnumerator RoyalChallengeSequence(GameObject ship)
        {
            Boss.isSpecialRoyalChallenge = true;

            yield return new WaitForSeconds(1f);
            AudioController.Instance.PlaySound2D("camera_flash_shorter", MixerGroup.TableObjectsSFX, 0.75f, 0f, null, null, null, null, false);
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(GameColors.Instance.nearWhite);
            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, 100f);

            yield return new WaitForSeconds(0.1f);
            Singleton<CameraEffects>.Instance.StopShake();
            Singleton<CameraEffects>.Instance.transform.localPosition = Vector3.zero;
            Destroy(ship);

            Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(0f, 6f);
            Singleton<CabinManager>.Instance.SetNorthWallHidden(false);
            Singleton<CameraEffects>.Instance.SetCameraFarPlane(50f);
            LeshyAnimationController.Instance.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);
            yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("!GIANTCARD_SHIP"), Singleton<BoardManager>.Instance.OpponentSlotsCopy[0], 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            AudioController.Instance.PlaySound3D("map_slam", MixerGroup.TableObjectsSFX, Singleton<BoardManager>.Instance.transform.position, 1f, 0f, null, null, null, null, false);

            yield return new WaitForSeconds(1.25f);
            Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
            Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);

            var royalSkull = Instantiate(ResourceBank.Get<GameObject>("Prefabs/CardBattle/PirateBossSkull"), transform);
            Boss.royalSkull = royalSkull;
            royalSkull.GetComponentInChildren<Animator>().SetTrigger("wake_up");
            royalSkull.GetComponentInChildren<Animator>().SetTrigger("stir");
            royalSkull.GetComponentInChildren<Animator>().SetTrigger("floating");
            royalSkull.transform.rotation = Quaternion.Euler(new Vector3(-36f, 0f, 0f));

            var baseSkullPos = new Vector3(0f, 7.8f, 7.8f);
            royalSkull.transform.position = baseSkullPos + Vector3.up * 5f;
            Tween.Position(royalSkull.transform, baseSkullPos, 1f, 0f, Tween.EaseOut, Tween.LoopType.None, null, delegate ()
            {
                Tween.Position(royalSkull.transform, royalSkull.transform.position + Vector3.up * 0.1f, 3f, 0f, Tween.EaseOut, Tween.LoopType.PingPong, null, null, true);
            }, true);

            yield return new WaitForSeconds(1.75f);
            Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
            yield return new WaitForSeconds(0.5f);
            Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
            yield return new WaitForSeconds(0.5f);
            PauseMenu.pausingDisabled = false;
        }
        #endregion

        #region Upkeep
        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            if (Boss.isSpecialRoyalChallenge)
                return false;

            if (TurnManager.Instance == null || TurnManager.Instance.Opponent is not Opponent opponent || opponent == null) // unity nullcheck
                return false;

            // Phase 1 intro
            if (opponent.NumLives == opponent.StartingLives && !didPhase1Introduction)
                return true;

            // Phase 3 pipis
            if (!playerUpkeep && opponent.NumLives == 4)
                return true;

            // Phase 4 shoot
            if (playerUpkeep && opponent.NumLives == 3)
                return true;

            // Ending fireworks
            if (playerUpkeep && opponent.NumLives <= 1)
                return true;

            return false;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            if (TurnManager.Instance == null || TurnManager.Instance.Opponent is not Opponent opponent || opponent == null) // unity nullcheck
                yield break;

            if (opponent.NumLives == opponent.StartingLives && !didPhase1Introduction)
                yield return Phase1Intro();
            else if (!playerUpkeep && opponent.NumLives == 4)
                yield return Phase3Pipis();
            else if (playerUpkeep && opponent.NumLives == 3)
                yield return Phase4Shoot();
            else if (playerUpkeep && opponent.NumLives <= 1)
                yield return EndingFireworks();
        }

        public IEnumerator Phase1Intro()
        {
            didPhase1Introduction = true;

            yield return new WaitForSeconds(0.25f);
            if (ViewManager.Instance.CurrentView != View.OpponentQueue)
            {
                ViewManager.Instance.SwitchToView(View.OpponentQueue);
                yield return new WaitForSeconds(0.25f);
            }
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoPhase1", TextDisplayer.MessageAdvanceMode.Input);

            yield return new WaitForSeconds(0.25f);
            if (ViewManager.Instance.CurrentView != View.Board)
            {
                ViewManager.Instance.SwitchToView(View.Board);
                yield return new WaitForSeconds(0.25f);
            }
            yield return BoardManager.Instance.CreateCardInSlot(TryGetCard("FlyingHead"), BoardManager.Instance.OpponentSlotsCopy[0]);
            yield return new WaitForSeconds(0.25f);
        }

        public IEnumerator Phase3Pipis()
        {
            ViewManager.Instance.SwitchToView(View.Default);
            yield return new WaitForSeconds(0.25f);

            var slots = BoardManager.Instance.OpponentSlotsCopy.FindAll(x => (x.Card == null || x.Card.Dead) && x.opposingSlot != null && x.opposingSlot.Card != null && !x.opposingSlot.Card.Dead);
            if (slots.Count <= 0)
                slots = BoardManager.Instance.OpponentSlotsCopy.FindAll(x => x.opposingSlot != null && x.opposingSlot.Card != null && !x.opposingSlot.Card.Dead);
            if (slots.Count <= 0)
                yield break;

            var randomslot = slots[SeededRandom.Range(0, slots.Count, SaveManager.SaveFile.GetCurrentRandomSeed() + TurnManager.Instance.TurnNumber * 100)];
            if (randomslot == null)
                yield break;
            if (randomslot.Card != null && !randomslot.Card.Dead)
                yield return randomslot.Card.Die(false);

            yield return Boss.SpawnPipisInSlot(randomslot);
        }

        public IEnumerator Phase4Shoot()
        {
            ViewManager.Instance.SwitchToView(View.Default);
            var slots = BoardManager.Instance.PlayerSlotsCopy;
            yield return new WaitForSeconds(0.25f);
            yield return Boss.ShootSlot(slots[SeededRandom.Range(0, slots.Count, SaveManager.SaveFile.GetCurrentRandomSeed() + TurnManager.Instance.TurnNumber * 250)]);
        }

        public IEnumerator EndingFireworks()
        {
            ViewManager.Instance.SwitchToView(View.Default);

            yield return new WaitForSeconds(0.25f);
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoEndingFireworks", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);

            foreach (CardSlot slot in BoardManager.Instance.OpponentSlotsCopy)
                yield return Boss.SpawnPipisInSlot(slot, false, false);
            foreach (CardSlot slot in BoardManager.Instance.OpponentSlotsCopy)
            {
                if (slot.Card == null)
                    continue;
                yield return slot.Card.Die(false, null, true);
            }

            Boss.ResetCannonAim();
            yield return new WaitForSeconds(0.25f);

            var piles = Singleton<CardDrawPiles3D>.Instance;
            if (TurnManager.Instance.TurnNumber - Boss.stallPhaseStartTurn <= 3)
                yield break;
            if (BoardManager.Instance.CardsOnBoard.Exists(x => x.HasAbility(helpCallAbility)))
                yield break;
            if (PlayerHand.Instance.CardsInHand.Exists(x => x.HasAbility(helpCallAbility)))
                yield break;
            if (piles.Deck != null && piles.Deck.CardsInDeck > 0)
                yield break;
            if (piles.SideDeck != null && piles.SideDeck.CardsInDeck > 0)
                yield break;

            didGiveFreeReceiver = true;
            yield return CardSpawner.Instance.SpawnCardToHand(TryGetCard("Receiver"), 0.25f);
        }
        #endregion

        public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
        {
            if (Boss.isSpecialRoyalChallenge)
                return false;

            return HeartCard != null && otherCard == HeartCard && otherCard.Slot != null && otherCard.Slot.IsPlayerSlot;
        }

        public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
        {
            yield return new WaitForSeconds(0.25f);
            if (ViewManager.Instance.CurrentView != View.Board)
            {
                ViewManager.Instance.SwitchToView(View.Board);
                yield return new WaitForSeconds(0.25f);
            }

            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoHeartStolen", TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);

            if (otherCard.Slot.opposingSlot.Card != null && !otherCard.Slot.opposingSlot.Card.Dead)
            {
                yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(otherCard.Slot.opposingSlot.Card, 0.25f);
            }

            otherCard.SetIsOpponentCard(true);
            otherCard.transform.eulerAngles += new Vector3(0f, 0f, -180f);
            yield return Singleton<BoardManager>.Instance.AssignCardToSlot(otherCard, otherCard.Slot.opposingSlot, 0.25f, null, true);
        }

        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            if (Boss.isSpecialRoyalChallenge)
                return false;

            if (HeartCard != null && HeartCard == card)
                return true;

            if (card.OpponentCard && card.Info.HasTrait(Trait.Giant) && Boss != null && Boss.BearChallengeActive)
                return true;

            return false;
        }

        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            if (HeartCard != null && HeartCard == card)
                yield return HeartDeath();
            else if (card.OpponentCard && card.Info.HasTrait(Trait.Giant) && (Boss?.BearChallengeActive).GetValueOrDefault())
                yield return BearChallengeGiantCardDeath();
        }

        public IEnumerator HeartDeath()
        {
            if (Chain != null)
            {
                Boss.lerpingOutChain = true;
                Tween.LocalScale(Chain.transform, Vector3.zero, 0.5f, 0f, Tween.EaseOut, Tween.LoopType.None, null, () => Destroy(Chain), true);
            }

            yield return new WaitForSeconds(0.25f);
            yield return TextDisplayer.Instance.PlayDialogueEvent("SneoHeartBroken", TextDisplayer.MessageAdvanceMode.Input);

            yield return new WaitForSeconds(0.25f);
            var before = ViewManager.Instance.CurrentView;
            yield return LifeManager.Instance.ShowDamageSequence(10, 10, false);
            ViewManager.Instance.SwitchToView(before, false, false);
        }

        public IEnumerator BearChallengeGiantCardDeath()
        {
            yield return new WaitForSeconds(0.25f);
            yield return Tutorial4BattleSequencer.BearGlitchSequence();
        }
    }
}
