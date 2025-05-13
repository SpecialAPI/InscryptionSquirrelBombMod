using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Bosses.TimeGuardian
{
    public class TimeGuardianBattleSequencer : BossBattleSequencer
    {
        public const int NUM_LIVES_FOR_CORRUPTION = 2;
        public override Opponent.Type BossType => Plugin.LOOPFINALBOSS;
        public override StoryEvent DefeatedStoryEvent => StoryEvent.NUM_EVENTS;
        public TimeGuardianBossOpponent Opponent => TurnManager.Instance?.Opponent as TimeGuardianBossOpponent;

        public override EncounterData BuildCustomEncounter(CardBattleNodeData nodeData)
        {
            return new()
            {
                aiId = "",
                opponentType = Plugin.LOOPFINALBOSS,
                startConditions = new()
                {
                    new()
                    {
                        cardsInOpponentSlots = new CardInfo[]
                        {
                            null,
                            null,
                            null,
                            TryGetCard("MoleEnigman")
                        }
                    }
                },
                opponentTotem = null,
                opponentTurnPlan = EncounterBuilder.BuildOpponentTurnPlan(TimeGuardianEncounter.loopfinalBlueprint, 20, false),
                Blueprint = TimeGuardianEncounter.loopfinalBlueprint
            };
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return !playerUpkeep;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            nextSideDrawShouldHurt = false;
            yield return NextTimeSpell();
            if(Opponent.NumLives <= NUM_LIVES_FOR_CORRUPTION)
            {
                yield return CastRandomCorruption();
            }
            yield break;
        }

        public IEnumerator NextTimeSpell()
        {
            CurrentTime++;
            if(CurrentTime > 12)
            {
                CurrentTime = 1;
            }
            yield return CastCurrentTimeSpell();
            yield break;
        }

        public IEnumerator CastCurrentTimeSpell()
        {
            SelectableCardPrefab ??= EasyAccess.CardSingleChoices.selectableCardPrefab;
            var oslot1 = OpponentSlotsCopy[0];
            var oslot4 = OpponentSlotsCopy.Last();
            var pslot1 = PlayerSlotsCopy[0];
            var pslot4 = PlayerSlotsCopy.Last();
            var spellCard = Instantiate(SelectableCardPrefab, new Vector3(
                Mathf.Lerp(oslot1.transform.position.x, oslot4.transform.position.x, 0.5f),
                oslot1.transform.position.y + 0.2f,
                Mathf.Lerp(oslot1.transform.position.z, pslot1.transform.position.z, 0.5f)), Quaternion.identity);
            var spell = spellCard.GetComponentInChildren<SelectableCard>();
            spellCard.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            spell.SetInfo(TryGetCard("!CLOCK_" + CurrentTime));
            spell.EnterBoard(0.3f, new Vector3(1f, 4f, 3f));
            yield return new WaitForSeconds(0.5f);
            switch (CurrentTime)
            {
                default:
                case 1:
                    var aliveCards = GetAliveCards(OpponentSlotsCopy);
                    if (aliveCards.Count > 0)
                    {
                        aliveCards.ForEach(x => x.AddTemporaryMod(new(1, 0)));
                    }
                    else
                    {
                        var random = OpponentSlotsCopy.RandomElement(GetRandomSeed());
                        yield return Board.CreateCardInSlot(TryGetCard("!CLOCK_POWER"), random, 0.1f, true);
                    }
                    break;
                case 2:
                    var cards = GetAliveCards(OpponentSlotsCopy);
                    if (cards.Count > 0)
                    {
                        cards.ForEach(x => x.AddTemporaryMod(new(0, 2)));
                    }
                    else
                    {
                        var random = OpponentSlotsCopy.RandomElement(GetRandomSeed());
                        yield return Board.CreateCardInSlot(TryGetCard("!CLOCK_HEALTH"), random, 0.1f, true);
                    }
                    break;
                case 3:
                    yield return DealDamageNoKill(3, true);
                    break;
                case 4:
                    var playerCards = GetAliveCards(PlayerSlotsCopy);
                    if (playerCards.Count > 0)
                    {
                        foreach (var c in playerCards)
                        {
                            yield return c.TakeDamage(1, null);
                        }
                    }
                    else
                    {
                        var random = PlayerSlotsCopy.RandomElement(GetRandomSeed());
                        yield return Board.CreateCardInSlot(TryGetCard("!CLOCK_SUFFERING"), random, 0.1f);
                    }
                    break;
                case 5:
                    nextSideDrawShouldHurt = true;
                    break;
                case 6:
                    var damage = Mathf.Clamp(LifeManager.Instance.OpponentDamage, 0, 6);
                    LifeManager.Instance.OpponentDamage -= damage;
                    if (LifeManager.Instance.Scales3D != null)
                    {
                        LifeManager.Instance.Scales3D.opponentWeight -= damage;
                        if (LifeManager.Instance.Scales3D.opponentWeights.Count > 0)
                        {
                            for (int i = 0; i < damage && LifeManager.Instance.Scales3D.opponentWeights.Count > 0; i++)
                            {
                                LifeManager.Instance.Scales3D.DestroyWeight(LifeManager.Instance.Scales3D.opponentWeights[0]);
                                LifeManager.Instance.Scales3D.opponentWeights.RemoveAt(0);
                            }
                        }
                        LifeManager.Instance.Scales3D.SetBalanceBasedOnWeights();
                    }
                    break;
                case 7:
                    var possibleCards = CardLoader.AllData.FindAll(x => x.metaCategories.Contains(CardMetaCategory.ChoiceNode) && x.BetterCostTier() >= 6 && x.BetterCostTier() <= 8);
                    var ally = TryGetCard("!CLOCK_ALLY");
                    var seed = GetRandomSeed();
                    if (possibleCards.Count > 0)
                    {
                        ally = possibleCards.RandomElement(seed++);
                    }
                    var queueSlots = GetEmptyQueueSlots();
                    if (queueSlots.Count > 0)
                    {
                        yield return Opponent.QueueCard(ally, queueSlots.RandomElement(seed++), true, true, true);
                    }
                    else
                    {
                        Opponent.Queue.RandomElement(seed++)?.AddTemporaryMod(new(ally.Attack, ally.Health) { abilities = new(ally.abilities) });
                    }
                    break;
                case 8:
                    var possibleDestructTargets = GetAliveCards(PlayerSlotsCopy);
                    var initialCount = possibleDestructTargets.Count;
                    var randomSeed = GetRandomSeed();
                    var damageToDeal = 8;
                    for (int i = 0; i < initialCount && possibleDestructTargets.Count > 0; i++)
                    {
                        var random = possibleDestructTargets.RandomElement(randomSeed++);
                        yield return random.TakeDamage(damageToDeal, null);
                        if (!random.Dead)
                        {
                            break;
                        }
                        else
                        {
                            damageToDeal = Mathf.Min(damageToDeal, random.MaxHealth - random.Status.damageTaken);
                        }
                    }
                    break;
                case 9:
                    var seed2 = GetRandomSeed();
                    if (Opponent.NumLives < Opponent.StartingLives)
                    {
                        //power 5
                        List<IXItem> powerfulItems = new();
                        if (GetAliveCards(PlayerSlotsCopy).Count > 0 && GetEmptySlots(OpponentSlotsCopy).Exists(x => GetFilledSlots(PlayerSlotsCopy).Contains(x.opposingSlot)))
                        {
                            powerfulItems.Add(IXItem.FishHook);
                        }
                        var trapperSlotsNeeded = 2;
                        if (GetAliveCards(PlayerSlotsCopy).Count > 0 && (GetEmptyQueueSlots().Count + GetEmptySlots(OpponentSlotsCopy).Count) >= trapperSlotsNeeded)
                        {
                            powerfulItems.Add(IXItem.TrapperKnife);
                        }
                        if (powerfulItems.Count < 0)
                        {
                            powerfulItems.Add(IXItem.Hourglass);
                        }
                        var powerfulItem = powerfulItems.RandomElement(seed2++);
                        switch (powerfulItem)
                        {
                            default:
                            case IXItem.Hourglass:
                                SkipNextPlayerTurn();
                                break;
                            case IXItem.TrapperKnife:
                                var cardToSnip = GetAliveCards(PlayerSlotsCopy).MaxElement(x => x.PowerLevel);
                                cardToSnip.Anim.PlayDeathAnimation();
                                yield return new WaitForSeconds(0.75f);
                                Destroy(cardToSnip.gameObject);
                                ViewManager.Instance.SwitchToView(View.Hand);
                                yield return new WaitForSeconds(0.25f);
                                yield return CardSpawner.Instance.SpawnCardToHand(TryGetCard("PeltWolf"), 0.25f);
                                yield return new WaitForSeconds(0.75f);
                                ViewManager.Instance.SwitchToView(View.OpponentQueueTradePhase, false, true);
                                yield return new WaitForSeconds(0.25f);
                                var queueSlotsForTradeCards = GetEmptyQueueSlots().Take(3).Where(x => x != null).ToList();
                                List<CardSlot> slotsForTradeCards = new();
                                CardInfo chosenCardInfo = null;
                                if (queueSlotsForTradeCards.Count < trapperSlotsNeeded)
                                {
                                    slotsForTradeCards = GetEmptySlots(OpponentSlotsCopy).Take(trapperSlotsNeeded - queueSlotsForTradeCards.Count).Where(x => x != null).ToList();
                                }
                                foreach (var s in queueSlotsForTradeCards)
                                {
                                    var card = TryGetCard("EnigmaStaff");
                                    card.Mods.Add(new(AbilitiesUtil.GetRandomLearnedAbility(seed2++, true, 1, 5, AbilityMetaCategory.Part1Modular)));
                                    yield return Opponent.QueueCard(card, s, true, true, true);
                                }
                                foreach (var s in slotsForTradeCards)
                                {
                                    yield return Board.CreateCardInSlot(TryGetCard("EnigmaStaff"), s, 0.1f, true);
                                }
                                bool pickedCard = false;
                                List<HighlightedInteractable> queueTradeSlots = Board.OpponentQueueSlots.FindAll(x => queueSlotsForTradeCards.Exists(x2 => x2.Index == Board.OpponentQueueSlots.IndexOf(x)));
                                queueTradeSlots.ForEach(tradeSlot => tradeSlot.CursorSelectStarted += x =>
                                {
                                    pickedCard = true;
                                    var card = Opponent.Queue.Find(x => x.QueuedSlot == queueSlotsForTradeCards[queueTradeSlots.IndexOf(tradeSlot)]);
                                    card?.Anim?.PlayDeathAnimation();
                                    if (card != null)
                                    {
                                        chosenCardInfo ??= card?.Info;
                                        Destroy(card.gameObject, 0.75f);
                                    }
                                });
                                slotsForTradeCards.ForEach(x => x.CursorSelectStarted += _ =>
                                {
                                    pickedCard = true;
                                    x?.Card?.UnassignFromSlot();
                                    x?.Card?.Anim?.PlayDeathAnimation();
                                    if (x?.Card != null)
                                    {
                                        chosenCardInfo ??= x?.Card?.Info;
                                        Destroy(x.Card.gameObject, 0.75f);
                                    }
                                });
                                var crosshairs = slotsForTradeCards.OfType<HighlightedInteractable>().Concat(queueTradeSlots).Select(x => Instantiate(royalCrosshair, x.transform)).ToList();
                                crosshairs.ForEach(x =>
                                {
                                    x.transform.localPosition = new Vector3(0f, 0.25f, 0f);
                                    x.transform.localRotation = Quaternion.identity;
                                });
                                yield return new WaitUntil(() => pickedCard);
                                slotsForTradeCards.ForEach(x => x.ClearDelegates());
                                queueTradeSlots.ForEach(x => x.ClearDelegates());
                                crosshairs.ForEach(x => Tween.LocalScale(x.transform, Vector3.zero, 0.1f, 0f, Tween.EaseIn, Tween.LoopType.None, null, delegate ()
                                {
                                    Destroy(x);
                                }, true));
                                crosshairs.Clear();
                                break;
                            case IXItem.FishHook:
                                var possibleCardsForHook = 
                                    GetFilledSlots(PlayerSlotsCopy)
                                    .FindAll(x => x != null && x.opposingSlot != null && x.Card != null && (x.opposingSlot.Card == null || x.opposingSlot.Card.Dead))
                                    .ConvertAll(x => x.Card)
                                    .SortedCopy((x, x2) => x.PowerLevel - x2.PowerLevel)
                                    .FirstOrDefault();
                                if(possibleCardsForHook != null)
                                {
                                    AudioController.Instance.PlaySound3D("angler_use_hook", MixerGroup.TableObjectsSFX, possibleCardsForHook.transform.position, 1f, 0f, null, null, null, null, false);
                                    LeshyAnimationController.Instance.RightArm.SetTrigger("angler_pullhook");
                                    yield return new WaitForSeconds(0.75f);
                                    if (possibleCardsForHook.Slot.opposingSlot.Card != null)
                                    {
                                        yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(possibleCardsForHook.Slot.opposingSlot.Card, 0.25f);
                                    }
                                    if (possibleCardsForHook.Status != null)
                                    {
                                        possibleCardsForHook.Status.anglerHooked = true;
                                    }
                                    possibleCardsForHook.SetIsOpponentCard(true);
                                    possibleCardsForHook.transform.eulerAngles += new Vector3(0f, 0f, -180f);
                                    yield return Singleton<BoardManager>.Instance.AssignCardToSlot(possibleCardsForHook, possibleCardsForHook.Slot.opposingSlot, 0.25f, null, true);
                                    yield return new WaitForSeconds(0.25f);
                                    yield return new WaitForSeconds(0.4f);
                                }
                                break;
                        }
                    }
                    else {
                        //power 4
                        List<IXItem> items = new();
                        if (Opponent?.TurnPlan != null && Opponent.TurnPlan.Count > 0 && Opponent.TurnPlan.Exists(x => x != null && x.Count > 0 && x.Exists(x => x != null)) && GetEmptyQueueSlots().Count > 0)
                        {
                            items.Add(IXItem.MagpieGlass);
                        }
                        if (GetAliveCards(PlayerSlotsCopy).Count > 0)
                        {
                            items.Add(IXItem.Scissors);
                        }
                        if (items.Count <= 0)
                        {
                            items.Add(IXItem.Dagger);
                        }
                        var item = items.RandomElement(seed2++);
                        switch (item)
                        {
                            default:
                            case IXItem.Dagger:
                                yield return DealDamageNoKill(4, true);
                                break;
                            case IXItem.Scissors:
                                var cardToSnip = GetAliveCards(PlayerSlotsCopy).RandomElement(seed2++);
                                cardToSnip.Anim.PlayDeathAnimation();
                                yield return new WaitForSeconds(0.75f);
                                Destroy(cardToSnip.gameObject);
                                break;
                            case IXItem.MagpieGlass:
                                List<CardInfo> strongestCards = new();
                                int strongestPowerLevel = int.MinValue;
                                var plannedCards = Opponent.TurnPlan.FindAll(x => x != null && x.Count > 0 && x.Exists(x => x != null)).SelectMany(x => x.FindAll(x => x != null)).ToList();
                                foreach (var card in plannedCards)
                                {
                                    if (card.PowerLevel >= strongestPowerLevel)
                                    {
                                        if (card.PowerLevel > strongestPowerLevel)
                                        {
                                            strongestPowerLevel = card.PowerLevel;
                                            strongestCards.Clear();
                                        }
                                        strongestCards.Add(card);
                                    }
                                }
                                if (strongestCards.Count > 0)
                                {
                                    yield return Opponent.QueueCard(strongestCards.RandomElement(seed2++), GetEmptyQueueSlots().RandomElement(seed2++), true, true, true);
                                }
                                break;
                        }
                    }
                    break;
                case 10:
                    //todo
                    break;
                case 11:
                    break;
                case 12:
                    foreach(var slot in AllSlotsCopy)
                    {
                        if(slot.Card != null)
                        {
                            slot.Card.UnassignFromSlot();
                            Destroy(slot.Card.gameObject);
                        }
                    }
                    yield return Board.CreateCardInSlot(TryGetCard("MoleEnigman"), OpponentSlotsCopy[3]);
                    break;
            }
            yield return new WaitForSeconds(0.5f);
            spell.Anim.PlayDeathAnimation();
            yield return new WaitForSeconds(0.5f);
            Destroy(spellCard);
            yield break;
        }

        public IEnumerator CastRandomCorruption()
        {
            yield break;
        }

        public override bool RespondsToOtherCardDrawn(PlayableCard card)
        {
            return card.IsFromSideDeck() && nextSideDrawShouldHurt;
        }

        public override IEnumerator OnOtherCardDrawn(PlayableCard card)
        {
            nextSideDrawShouldHurt = false;
            yield return DealDamageNoKill(5, true);
            yield break;
        }

        public bool nextSideDrawShouldHurt;
        public int CurrentTime;
        public GameObject SelectableCardPrefab;
        public enum IXItem
        {
            Scissors,
            MagpieGlass,
            Dagger,
            FishHook,
            TrapperKnife,
            Hourglass
        }
    }
}
