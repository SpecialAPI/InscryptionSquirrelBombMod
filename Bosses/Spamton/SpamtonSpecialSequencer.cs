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
        public override Opponent.Type BossType => SpamtonSetup.spamboss;
        public override StoryEvent DefeatedStoryEvent => StoryEvent.NUM_EVENTS;
        public bool didPhase1Introduction;
        public SpamtonBossOpponent Boss => TurnManager.Instance?.Opponent != null && (TurnManager.Instance.Opponent is SpamtonBossOpponent) ? (TurnManager.Instance.Opponent as SpamtonBossOpponent) : null;
        public PlayableCard HeartCard => Boss?.heartCard;
        public GameObject Chain => Boss?.chain;
        public bool didGiveFreeReceiver;
        public int deathTurnCounter;
        public int receiverCounter;
        public bool playedLongFireworksDialogue;
        public readonly int TurnsUntilDeath = 5;

        public bool ShouldIncrementDeathTurnCounter()
        {
            return !BoardManager.Instance.CardsOnBoard.Exists(x => x.HasAbility(SpamtonSetup.helpCallAbility)) && (PlayerHand.Instance.CardsInHand.Exists(x => x.HasAbility(SpamtonSetup.helpCallAbility)) ||
                didGiveFreeReceiver);
        }

        public override EncounterData BuildCustomEncounter(CardBattleNodeData nodeData)
        {
            return new()
            {
                opponentType = SpamtonSetup.spamboss,
                Blueprint = SpamtonSetup.spamP1,
                opponentTurnPlan = EncounterBuilder.BuildOpponentTurnPlan(SpamtonSetup.spamP1, 20, true),
                startConditions = new()
                {
                    new()
                    {
                        cardsInPlayerSlots = new CardInfo[]
                        {
                            CardLoader.GetCardByName("morebosses_BigShooter"),
                            null,
                            null,
                            null
                        }
                    }
                }
            };
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return ((TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives == TurnManager.Instance.Opponent.StartingLives && !didPhase1Introduction) || 
                (!playerUpkeep && TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives == 4) ||
                (playerUpkeep && TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives == 3) ||
                (playerUpkeep && TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives <= 1)) && !Boss.isSpecialRoyalChallenge;
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return playerTurnEnd && !Boss.isSpecialRoyalChallenge;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            PlayableCard receiver = BoardManager.Instance.CardsOnBoard.Find(x => x.HasAbility(SpamtonSetup.helpCallAbility));
            if (receiver != null)
            {
                Boss.ResetCannonAim();
                deathTurnCounter = 0;
                receiver.Anim.StrongNegationEffect();
                receiver.Anim.PlayRiffleSound();
                receiverCounter++;
                yield return new WaitForSeconds(0.5f);
                yield return TextDisplayer.Instance.PlayDialogueEvent("SneoReceiver" + receiverCounter, TextDisplayer.MessageAdvanceMode.Input);
                if(receiverCounter >= 7)
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
                    AudioSource audioSource = AudioController.Instance.PlaySound2D("pirate_ship_charge", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
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
                    //AudioController.Instance.PlaySound2D("camera_flash_shorter", MixerGroup.TableObjectsSFX, 0.75f, 0f, null, null, null, null, false);
                    //Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(GameColors.Instance.nearWhite);
                    //Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, 100f);
                    //yield return new WaitForSeconds(0.1f);
                    Singleton<CameraEffects>.Instance.StopShake();
                    Singleton<CameraEffects>.Instance.transform.localPosition = Vector3.zero;
                    Singleton<CameraEffects>.Instance.SetCameraFarPlane(50f);
                    //(Singleton<ExplorableAreaManager>.Instance as CabinManager).SetNorthWallHidden(false);
                    LeshyAnimationController.Instance.gameObject.SetActive(true);
                    AudioController.Instance.PlaySound2D("defeated", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
                    Transform root = Boss.sneoAnimation.transform;
                    List<Transform> limbs = new()
                    {
                        root.Find("leg1"),
                        root.Find("leg2"),
                        root.Find("hand1"),
                        root.Find("hand2attachPoint"),
                    };
                    Tween.Shake(Singleton<CameraEffects>.Instance.transform, Vector3.zero, new Vector3(0.5f, 0.5f, 0f), 8f, 0f, Tween.LoopType.None, null, null, false);
                    Boss.sneoAnimation.GetComponent<Animator>().enabled = false;
                    foreach (Transform t in limbs)
                    {
                        t.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
                        t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, -.03f);
                        Tween.Shake(t, t.localPosition, new(0.1f, 0.1f), 12f, 0f, Tween.LoopType.None, null, null, false);
                    }
                    Tween.Shake(root, root.localPosition, new(0.1f, 0.1f), 12f, 0f, Tween.LoopType.None, null, null, false);
                    Time.timeScale = 0.1f;
                    TextDisplayer.Instance.ShowMessage("NOOOOOOOOOOOOOOO!!!!!!", speaker: SpamtonSetup.spamtonSpeaker);
                    yield return new WaitForSecondsRealtime(6f);
                    TextDisplayer.Instance.ShowMessage("DEFEATED BY A WEAKLING HUMAN...", speaker: SpamtonSetup.spamtonSpeaker);
                    Tween.Position(Boss.sneoAnimation.transform, Boss.sneoAnimation.transform.position + Vector3.back * 20f + Vector3.up * 1.5f, 2.5f, 0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
                    Tween.Rotation(Boss.sneoAnimation.transform, Quaternion.Euler(Boss.sneoAnimation.transform.rotation.eulerAngles.x + 180, Boss.sneoAnimation.transform.rotation.eulerAngles.y + 180, 
                        Boss.sneoAnimation.transform.rotation.eulerAngles.z + 180), 2.5f, 
                        0f, Tween.EaseIn, Tween.LoopType.None, null, null, true);
                    yield return new WaitForSecondsRealtime(4f);
                    TextDisplayer.Instance.Clear();
                    float ela = 0f;
                    while(ela < 1f)
                    {
                        Time.timeScale = Mathf.Lerp(0.1f, 1f, ela);
                        ela += Time.unscaledDeltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.FinalBoss))
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
                        (Singleton<ExplorableAreaManager>.Instance as CabinManager).SetNorthWallHidden(false);
                        Singleton<CameraEffects>.Instance.SetCameraFarPlane(50f);
                        LeshyAnimationController.Instance.gameObject.SetActive(true);
                        yield return new WaitForSeconds(1.5f);
                        yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("!GIANTCARD_SHIP"), Singleton<BoardManager>.Instance.OpponentSlotsCopy[0], 0.2f, true);
                        yield return new WaitForSeconds(0.2f);
                        AudioController.Instance.PlaySound3D("map_slam", MixerGroup.TableObjectsSFX, Singleton<BoardManager>.Instance.transform.position, 1f, 0f, null, null, null, null, false);
                        yield return new WaitForSeconds(1.25f);
                        Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
                        Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                        GameObject royalSkull = Instantiate(ResourceBank.Get<GameObject>("Prefabs/CardBattle/PirateBossSkull"), transform);
                        Boss.royalSkull = royalSkull;
                        royalSkull.GetComponentInChildren<Animator>().SetTrigger("wake_up");
                        royalSkull.GetComponentInChildren<Animator>().SetTrigger("stir");
                        royalSkull.GetComponentInChildren<Animator>().SetTrigger("floating");
                        royalSkull.transform.rotation = Quaternion.Euler(new Vector3(-36f, 0f, 0f));
                        Vector3 baseSkullPos = new(0f, 7.8f, 7.8f);
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
                    else
                    {
                        Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(Color.black);
                        Singleton<UIManager>.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(1f, 3f);
                        yield return new WaitForSecondsRealtime(4.5f);
                        Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
                        PauseMenu.pausingDisabled = false;
                        Boss.EndAscensionRun();
                        SceneLoader.Load("Ascension_Credits");
                    }
                }
            }
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

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            if(TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives == TurnManager.Instance.Opponent.StartingLives && !didPhase1Introduction)
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
                yield return BoardManager.Instance.CreateCardInSlot(CardLoader.GetCardByName("morebosses_FlyingHead"), BoardManager.Instance.OpponentSlotsCopy[0]);
                yield return new WaitForSeconds(0.25f);
            }
            else if (!playerUpkeep && TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives == 4)
            {
                ViewManager.Instance.SwitchToView(View.Default);
                yield return new WaitForSeconds(0.25f);
                List<CardSlot> slots = BoardManager.Instance.OpponentSlotsCopy.FindAll(x => (x.Card == null || x.Card.Dead) && x.opposingSlot != null && x.opposingSlot.Card != null && !x.opposingSlot.Card.Dead);
                if (slots.Count <= 0)
                {
                    slots = BoardManager.Instance.OpponentSlotsCopy.FindAll(x => x.opposingSlot != null && x.opposingSlot.Card != null && !x.opposingSlot.Card.Dead);
                }
                if (slots.Count > 0)
                {
                    CardSlot randomslot = slots[SeededRandom.Range(0, slots.Count, SaveManager.SaveFile.GetCurrentRandomSeed() + TurnManager.Instance.TurnNumber * 100)];
                    if (randomslot != null)
                    {
                        if (randomslot.Card != null && !randomslot.Card.Dead)
                        {
                            yield return randomslot.Card.Die(false);
                        }
                        yield return Boss.SpawnPipisInSlot(randomslot);
                    }
                }
            }
            else if(playerUpkeep && TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives == 3)
            {
                ViewManager.Instance.SwitchToView(View.Default);
                List<CardSlot> slots = BoardManager.Instance.PlayerSlotsCopy;
                yield return new WaitForSeconds(0.25f);
                yield return Boss.ShootSlot(slots[SeededRandom.Range(0, slots.Count, SaveManager.SaveFile.GetCurrentRandomSeed() + TurnManager.Instance.TurnNumber * 250)]);
            }
            else if(playerUpkeep && TurnManager.Instance != null && TurnManager.Instance.Opponent != null && TurnManager.Instance.Opponent.NumLives <= 1)
            {
                ViewManager.Instance.SwitchToView(View.Default);
                yield return new WaitForSeconds(0.25f);
                yield return TextDisplayer.Instance.PlayDialogueEvent("SneoEndingFireworks", TextDisplayer.MessageAdvanceMode.Input);
                yield return new WaitForSeconds(0.25f);
                foreach (CardSlot slot in BoardManager.Instance.OpponentSlotsCopy)
                {
                    yield return Boss.SpawnPipisInSlot(slot, false, false);
                }
                foreach(CardSlot slot in BoardManager.Instance.OpponentSlotsCopy)
                {
                    if(slot.Card != null)
                    {
                        yield return slot.Card.Die(false, null, true);
                    }
                }
                Boss.ResetCannonAim();
                yield return new WaitForSeconds(0.25f);
                CardDrawPiles3D piles = Singleton<CardDrawPiles3D>.Instance;
                if(!BoardManager.Instance.CardsOnBoard.Exists(x => x.HasAbility(SpamtonSetup.helpCallAbility)) && !PlayerHand.Instance.CardsInHand.Exists(x => x.HasAbility(SpamtonSetup.helpCallAbility)) && 
                    ((piles.SideDeck?.CardsInDeck).GetValueOrDefault() + (piles.Deck?.CardsInDeck).GetValueOrDefault()) <= 0 && TurnManager.Instance.TurnNumber - Boss.stallPhaseStartTurn > 3)
                {
                    didGiveFreeReceiver = true;
                    yield return CardSpawner.Instance.SpawnCardToHand(CardLoader.GetCardByName("morebosses_Receiver"), 0.25f);
                }
            }
            yield break;
        }

        public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
        {
            return (HeartCard != null && otherCard == HeartCard && otherCard.Slot != null && otherCard.Slot.IsPlayerSlot) && !Boss.isSpecialRoyalChallenge;
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
            yield break;
        }

        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            return ((HeartCard != null && HeartCard == card) || (card.OpponentCard && card.Info.HasTrait(Trait.Giant)) && (Boss?.BearChallengeActive).GetValueOrDefault()) && !Boss.isSpecialRoyalChallenge;
        }

        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            if((HeartCard != null && HeartCard == card))
            {
                if (Chain != null)
                {
                    Boss.lerpingOutChain = true;
                    Tween.LocalScale(Chain.transform, Vector3.zero, 0.5f, 0f, Tween.EaseOut, Tween.LoopType.None, null, () => Destroy(Chain), true);
                }
                yield return new WaitForSeconds(0.25f);
                yield return TextDisplayer.Instance.PlayDialogueEvent("SneoHeartBroken", TextDisplayer.MessageAdvanceMode.Input);
                yield return new WaitForSeconds(0.25f);
                View before = ViewManager.Instance.CurrentView;
                yield return LifeManager.Instance.ShowDamageSequence(10, 10, false);
                ViewManager.Instance.SwitchToView(before, false, false);
            }
            else if (card.OpponentCard && card.Info.HasTrait(Trait.Giant) && (Boss?.BearChallengeActive).GetValueOrDefault())
            {
                yield return new WaitForSeconds(0.25f);
                yield return Tutorial4BattleSequencer.BearGlitchSequence();
            }
            yield break;
        }
    }
}
