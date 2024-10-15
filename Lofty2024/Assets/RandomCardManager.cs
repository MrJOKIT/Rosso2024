using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class RandomCardManager : MonoBehaviour
{
    public PlayerArtifact player;
    public GameObject cardRandomCanvas;
    [Header("Random Card")]
    public GameObject cardSlotPrefab;
    public Transform cardSlotParent;
    [Space(10)] 
    public List<CardSlot> cardSlots;
    [Space(10)]
    public List<ArtifactData> cardList;
    [Space(10)] 
    public List<ArtifactData> currentCardRandom;
    [Space(10)] 
    [Range(1,4)]public int randomCount;
    [Space(10)]
    public int randomCost;

    [Space(20)] 
    [Header("UI")] 
    public bool artifactUsed;
    public bool haveArtifact;
    public Button randomAgainButton;
    public Button randomAgainWithArtifactButton;
    
    [Button("RandomCard")]
    public void StartRandomCard()
    {
        cardRandomCanvas.SetActive(true);
        for (int a = 0; a < randomCount; a++)
        {
            GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
            card.GetComponent<CardSlot>().SetCard(a,RandomCardInList());
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
            GetComponent<GameCurrency>().DecreaseEricCoin(randomCost);
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
        randomAgainButton.interactable = GetComponent<GameCurrency>().EricCoin >= randomCost;

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
        if (artifactUsed)
        {
            artifactUsed = false;
        }
        ClearSlots();
        cardRandomCanvas.SetActive(false);
    }
}
