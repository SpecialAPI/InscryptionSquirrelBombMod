using System;
using System.Collections.Generic;
using System.Text;

namespace SquirrelBombMod.Nodes
{
    public class SpecialRareCardChoicesSequencer : RareCardChoicesSequencer, IInherit, ICustomNodeSequencer
    {
        public IEnumerator DoCustomSequence(CustomSpecialNodeData nodeData)
        {
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
			yield return new WaitForSeconds(1f);
			Singleton<TableRuleBook>.Instance.SetOnBoard(true);
			deckPile.gameObject.SetActive(true);
			StartCoroutine(deckPile.SpawnCards(RunState.DeckList.Count, 1f));
			Vector3 vector = new(box.position.x, box.position.y, 0.5f);
			Vector3 startPos = new(vector.x, vector.y, 9f);
			box.position = startPos;
			box.gameObject.SetActive(true);
			AudioController.Instance.PlaySound3D("woodbox_slide", MixerGroup.TableObjectsSFX, vector, 1f, 0f, null, null, null, null, false);
			Tween.Position(box, vector, 0.3f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
			yield return new WaitForSeconds(0.3f);
			yield return new WaitForSeconds(0.5f);
			if (!SaveFile.IsAscension || !DialogueEventsData.EventIsPlayed("ChallengeNoBossRares"))
			{
				yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("RareCardsIntro", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
			}
			selectableCards = SpawnCards(3, box.transform, new Vector3(-1.55f, 0.2f, 0f), 1.5f);
			List<CardChoice> list;
			if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.NoBossRares))
			{
				list = choiceGenerator.GenerateChoices(new CardChoicesNodeData(), SaveManager.SaveFile.GetCurrentRandomSeed());
			}
			else
			{
				list = rareChoiceGenerator.GenerateChoices(SaveManager.SaveFile.GetCurrentRandomSeed());
			}
			for (int i = 0; i < selectableCards.Count; i++)
			{
				selectableCards[i].gameObject.SetActive(true);
				selectableCards[i].ChoiceInfo = list[i];
				selectableCards[i].Initialize(list[i].CardInfo, new Action<SelectableCard>(OnRewardChosen), new Action<SelectableCard>(OnCardFlipped), true, new Action<SelectableCard>(OnCardInspected));
				selectableCards[i].SetEnabled(false);
				selectableCards[i].SetFaceDown(true, true);
				SpecialCardBehaviour[] components = selectableCards[i].GetComponents<SpecialCardBehaviour>();
				for (int j = 0; j < components.Length; j++)
				{
					components[j].OnShownForCardChoiceNode();
				}
			}
			box.GetComponentInChildren<Animator>().Play("open", 0, 0f);
			AudioController.Instance.PlaySound3D("woodbox_open", MixerGroup.TableObjectsSFX, box.transform.position, 1f, 0f, null, null, null, null, false);
			yield return new WaitForSeconds(2f);
			Singleton<ViewManager>.Instance.SwitchToView(choicesView, false, false);
			ChallengeActivationUI.TryShowActivation(AscensionChallenge.NoBossRares);
			if (AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.NoBossRares) && !DialogueEventsData.EventIsPlayed("ChallengeNoBossRares"))
			{
				yield return new WaitForSeconds(0.5f);
				yield return Singleton<TextDisplayer>.Instance.PlayDialogueEvent("ChallengeNoBossRares", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
			}
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
			SetCollidersEnabled(true);
			gamepadGrid.enabled = true;
			EnableViewDeck(viewControlMode, basePosition);
			chosenReward = null;
			yield return new WaitUntil(() => chosenReward != null);
			DisableViewDeck();
			chosenReward.transform.parent = null;
			Singleton<RuleBookController>.Instance.SetShown(false, true);
			gamepadGrid.enabled = false;
			if (chosenReward.ChoiceInfo.resourceType != ResourceType.None)
			{
				bool flag = chosenReward.ChoiceInfo.resourceType == ResourceType.Blood;
				CardInfo cardInfo;
				if (flag)
				{
					cardInfo = GetRandomChoosableRareCardWithCost(SaveManager.SaveFile.GetCurrentRandomSeed(), chosenReward.ChoiceInfo.resourceAmount);
				}
				else
				{
					cardInfo = GetRandomChoosableRareBonesCard(SaveManager.SaveFile.GetCurrentRandomSeed());
				}
				if (this.choiceGenerator is Part1CardChoiceGenerator)
				{
					CardInfo cardInfo2 = (this.choiceGenerator as Part1CardChoiceGenerator).RollRandomDeathCardOfCost(flag, chosenReward.ChoiceInfo.resourceAmount, SaveManager.SaveFile.GetCurrentRandomSeed());
					if (cardInfo2 != null)
					{
						cardInfo = cardInfo2;
					}
				}
				if (cardInfo == null)
				{
					cardInfo = CardLoader.GetCardByName("Geck");
				}
				chosenReward.SetInfo(cardInfo);
				chosenReward.SetFaceDown(false, false);
				chosenReward.SetInteractionEnabled(false);
				yield return TutorialTextSequence(chosenReward);
				chosenReward.SetCardbackToDefault();
				yield return WaitForCardToBeTaken(chosenReward);
			}
			else if (chosenReward.ChoiceInfo.tribe != Tribe.None)
			{
				CardInfo cardInfo = GetRandomChoosableRareCardOfTribe(SaveManager.SaveFile.GetCurrentRandomSeed(), chosenReward.ChoiceInfo.tribe);
				if (cardInfo == null)
				{
					cardInfo = CardLoader.GetCardByName("Amalgam");
				}
				chosenReward.SetInfo(cardInfo);
				chosenReward.SetFaceDown(false, false);
				chosenReward.SetInteractionEnabled(false);
				yield return TutorialTextSequence(chosenReward);
				chosenReward.SetCardbackToDefault();
				yield return WaitForCardToBeTaken(chosenReward);
			}
			deckPile.MoveCardToPile(chosenReward, true, 0.25f, 1.15f);
			AddChosenCardToDeck();
			CleanupMushrooms();
			yield return new WaitForSeconds(0.5f);
			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
			yield return new WaitForSeconds(0.5f);
			box.GetComponentInChildren<Animator>().Play("close", 0, 0f);
			AudioController.Instance.PlaySound3D("woodbox_close", MixerGroup.TableObjectsSFX, box.transform.position, 1f, 0f, null, null, null, null, false);
			yield return new WaitForSeconds(0.6f);
			CleanUpCards(false);
			Tween.Position(box, startPos, 0.3f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
			yield return new WaitForSeconds(0.3f);
			box.gameObject.SetActive(false);
			yield return StartCoroutine(deckPile.DestroyCards(0.5f));
			deckPile.gameObject.SetActive(false);
			ProgressionData.SetMechanicLearned(MechanicsConcept.RareCards);
			if (RunState.Run.eyeState == EyeballState.Missing)
			{
				Singleton<GameFlowManager>.Instance.TransitionToGameState(GameState.SpecialCardSequence, new ChooseEyeballNodeData());
			}
			else
			{
				Singleton<GameFlowManager>.Instance.TransitionToGameState(GameState.Map, null);
			}
			yield break;
		}

        public virtual void Inherit(CustomSpecialNodeData node)
        {
            var target = EasyAccess.RareCardChoices;
            if (target != null)
            {
                transform.position = target.transform.position;
                transform.rotation = Quaternion.Euler(target.transform.rotation.eulerAngles);
                if (target.deckPile != null)
                {
                    deckPile = Instantiate(target.deckPile, target.deckPile.transform.position, target.deckPile.transform.rotation);
                    Vector3 position = deckPile.transform.position;
                    Quaternion rotation = deckPile.transform.rotation;
                    deckPile.transform.parent = transform;
                    deckPile.transform.position = position;
                    deckPile.transform.rotation = Quaternion.Euler(rotation.eulerAngles);
                }
                selectableCardPrefab = target.selectableCardPrefab;
                if (target.gamepadGrid != null)
                {
                    gamepadGrid = Instantiate(target.gamepadGrid, target.gamepadGrid.transform.position, target.gamepadGrid.transform.rotation);
                    Vector3 position = gamepadGrid.transform.localPosition;
                    Quaternion rotation = gamepadGrid.transform.localRotation;
                    gamepadGrid.transform.parent = transform;
                    gamepadGrid.transform.localPosition = position;
                    gamepadGrid.transform.localRotation = rotation;
                }
            }
        }
    }
}
