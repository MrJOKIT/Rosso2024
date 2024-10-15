using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;
using Random = UnityEngine.Random;

public class RandomCardManager : MonoBehaviour
{
    public PlayerArtifact player;
    public GameObject cardRandomCanvas;
    [Header("Random Card")] 
    public bool cardOutOfStock;
    public GameObject cardSlotPrefab;
    public Transform cardSlotParent;
    [Space(10)]
    public List<ArtifactData> cardList;
    [Space(10)] 
    public List<ArtifactData> currentCardRandom;
    [Space(10)] 
    public List<CardSlot> cardSlots;
    [Space(10)] 
    [Range(1,4)]public int randomCount;
    [Space(10)]
    public int randomCost;
    public int currentCost;
    public TextMeshProUGUI costText;

    [Space(20)] 
    [Header("UI")] 
    public bool artifactUsed;
    public bool haveArtifact;
    public Button randomAgainButton;
    public Button randomAgainWithArtifactButton;

    
    private void Awake()
    {
        Debug.Log(cardList.Count);
        currentCost = randomCost / 2;
    }
    [Button("RandomCard")]
    public void StartRandomCard()
    {
        if (cardOutOfStock)
        {
            AnnouncementManager.Instance.ShowTextTimer("Out of cards",1.5f);
            return;
        }
        player.GetComponent<PlayerMovementGrid>().currentState = MovementState.Freeze;
        currentCost *= 2; 
        cardRandomCanvas.SetActive(true);
        if (cardList.Count > 1)
        {
            for (int a = 0; a < randomCount; a++)
            {
                if (a > cardList.Count + 1)
                { 
                    continue;
                }
                GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                card.GetComponent<CardSlot>().SetCard(a,RandomCardInList());
                cardSlots.Add(card.GetComponent<CardSlot>());
            }
        }
        else
        {
            GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
            card.GetComponent<CardSlot>().SetCard(0,RandomCardInList());
            cardSlots.Add(card.GetComponent<CardSlot>());
        }
        
        UpdateButton();
    }

    private ArtifactData RandomCardInList()
    {
        int randomNumber = Random.Range(0, cardList.Count - 1);
        ArtifactData cardData = cardList[randomNumber];
        currentCardRandom.Add(cardList[randomNumber]);
        cardList.Remove(cardList[randomNumber]);
        return cardData;
    }

    public void RandomAgain(bool withArtifact)
    {
        ClearSlots();
        if (withArtifact)
        {
            artifactUsed = true;
            StartRandomCard();
        }
        else
        {
            GetComponent<GameCurrency>().DecreaseEricCoin(currentCost);
            StartRandomCard();
        }
    }

    public void SelectedCard(int cardIndex)
    {
        player.AddNewArtifact(currentCardRandom[cardIndex]);
        currentCardRandom.Remove(currentCardRandom[cardIndex]);
        RandomEnd();
    }

    private void ClearSlots()
    {
        foreach (ArtifactData card in currentCardRandom.ToList())
        {
            cardList.Add(card);
            currentCardRandom.Remove(card);
        }
        foreach (CardSlot slot in cardSlots.ToList())
        {
            cardSlots.Remove(slot);
            Destroy(slot.gameObject);
        }
    }

    private void UpdateButton()
    {
        costText.text = "COST: " + currentCost;
        randomAgainButton.interactable = GetComponent<GameCurrency>().EricCoin >= currentCost;

        if (haveArtifact)
        {
            if (!artifactUsed)
            {
                randomAgainWithArtifactButton.gameObject.SetActive(true);
                randomAgainWithArtifactButton.interactable = true;
            }
            else
            {
                randomAgainWithArtifactButton.gameObject.SetActive(false);
                randomAgainWithArtifactButton.interactable = false;
            }
        }
        else
        {
            randomAgainWithArtifactButton.gameObject.SetActive(false);
        }
    }

    private void RandomEnd()
    {
        currentCost = randomCost / 2;
        if (artifactUsed)
        {
            artifactUsed = false;
        }
        ClearSlots();
        cardRandomCanvas.SetActive(false);

        if (cardList.Count <= 0)
        {
            cardOutOfStock = true;
        }
        player.GetComponent<PlayerMovementGrid>().currentState = MovementState.Idle;
    }
}
