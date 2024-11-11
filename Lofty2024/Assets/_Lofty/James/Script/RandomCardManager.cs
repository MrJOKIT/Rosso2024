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
    [Tab("Random Setting")] 
    public bool isRandom;
    public PlayerArtifact player;
    public GameObject cardRandomCanvas;
    [Header("Random Card")] 
    public bool cardOutOfStock;
    public bool commonOutOfStock;
    public bool rareOutOfStock;
    public bool epicOutOfStock;
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

    [Tab("Drop Rate")] 
    [Header("Drop Rate Setting")] 
    [Range(0,1f)][SerializeField] private float commonRate;
    [Range(0,1f)][SerializeField] private float rareRate;
    [Space(10)]
    public List<ArtifactData> cardCommon;
    public List<ArtifactData> cardRare;
    public List<ArtifactData> cardEpic;
    [Space(10)] 
    public List<ArtifactData> cardKnight;
    public bool cardKnightOutOfStock;
    public List<ArtifactData> cardBlade;
    public bool cardBladeOutOfStock;
    public List<ArtifactData> cardShoot;
    public bool cardShootOutOfStock;

    
    private void Awake()
    {
        currentCost = randomCost / 2;
        SortingCardGrade();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartRandomCardFixClass(CardClass.SwordKnight,4);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartRandomCardFixClass(CardClass.BladeMaster,4);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartRandomCardFixClass(CardClass.ShootingCaster,4);
        }
    }

    [Button("RandomCard")]
    public void StartRandomCard()
    {
        if (cardOutOfStock)
        {
            GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
            return;
        }
        isRandom = true;
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
                card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.All,CardClass.Normal));
                cardSlots.Add(card.GetComponent<CardSlot>());
            }
        }
        else
        {
            GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
            card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.All,CardClass.Normal));
            cardSlots.Add(card.GetComponent<CardSlot>());
        }
        
        UpdateButton();
    }

    
    public void StartRandomCardFixGrade(ArtifactGrade cardGrade,int count)
    {
        switch (cardGrade)
        {
            case ArtifactGrade.Common:
                if (commonOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
            case ArtifactGrade.Rare:
                if (rareOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
            case ArtifactGrade.Epic:
                if (epicOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
            default:
                if (cardOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
        }
        isRandom = true;
        player.GetComponent<PlayerMovementGrid>().currentState = MovementState.Freeze;
        cardRandomCanvas.SetActive(true);
        switch (cardGrade)
        {
            case ArtifactGrade.Common:
                if (cardCommon.Count > 1)
                {
                    for (int a = 0; a < count; a++)
                    {
                        if (a > cardCommon.Count + 1)
                        { 
                            continue;
                        }
                        GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                        card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.Common,CardClass.Normal));
                        cardSlots.Add(card.GetComponent<CardSlot>());
                    }
                }
                else
                {
                    GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                    card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.Common,CardClass.Normal));
                    cardSlots.Add(card.GetComponent<CardSlot>());
                }
                break;
            case ArtifactGrade.Rare:
                if (cardRare.Count > 1)
                {
                    for (int a = 0; a < count; a++)
                    {
                        if (a > cardRare.Count + 1)
                        { 
                            continue;
                        }
                        GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                        card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.Rare,CardClass.Normal));
                        cardSlots.Add(card.GetComponent<CardSlot>());
                    }
                }
                else
                {
                    GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                    card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.Rare,CardClass.Normal));
                    cardSlots.Add(card.GetComponent<CardSlot>());
                }
                break;
            case ArtifactGrade.Epic:
                if (cardEpic.Count > 1)
                {
                    for (int a = 0; a < count; a++)
                    {
                        if (a > cardEpic.Count + 1)
                        { 
                            continue;
                        }
                        GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                        card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.Epic,CardClass.Normal));
                        cardSlots.Add(card.GetComponent<CardSlot>());
                    }
                }
                else
                {
                    GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                    card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.Epic,CardClass.Normal));
                    cardSlots.Add(card.GetComponent<CardSlot>());
                }
                break;
            case ArtifactGrade.All:
                if (cardList.Count > 1)
                {
                    for (int a = 0; a < randomCount; a++)
                    {
                        if (a > cardList.Count + 1)
                        { 
                            continue;
                        }
                        GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                        card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.All,CardClass.Normal));
                        cardSlots.Add(card.GetComponent<CardSlot>());
                    }
                }
                else 
                {
                    GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                    card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.All,CardClass.Normal));
                    cardSlots.Add(card.GetComponent<CardSlot>());
                }
                break;
        }
        
        CloseButton();
    }
    public void StartRandomCardFixClass(CardClass cardCLass,int count)
    {
        
        switch (cardCLass)
        {
            case CardClass.SwordKnight:
                if (cardKnightOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
            case CardClass.BladeMaster:
                if (cardBladeOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
            case CardClass.ShootingCaster:
                if (cardShootOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
            default:
                if (cardOutOfStock)
                {
                    GetComponent<AnnouncementManager>().ShowTextTimer("Out of cards",1.5f);
                    return;
                }
                break;
        }
        isRandom = true;
        player.GetComponent<PlayerMovementGrid>().currentState = MovementState.Freeze;
        cardRandomCanvas.SetActive(true);
        switch (cardCLass)
        {
            case CardClass.SwordKnight:
                if (cardKnight.Count > 1)
                {
                    for (int a = 0; a < count; a++)
                    {
                        if (a > cardKnight.Count + 1)
                        { 
                            continue;
                        }
                        GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                        card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.All,CardClass.SwordKnight));
                        cardSlots.Add(card.GetComponent<CardSlot>());
                    }
                }
                else
                {
                    GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                    card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.All,CardClass.SwordKnight));
                    cardSlots.Add(card.GetComponent<CardSlot>());
                }
                break;
            case CardClass.BladeMaster:
                if (cardBlade.Count > 1)
                {
                    for (int a = 0; a < count; a++)
                    {
                        if (a > cardBlade.Count + 1)
                        { 
                            continue;
                        }
                        GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                        card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.All,CardClass.BladeMaster));
                        cardSlots.Add(card.GetComponent<CardSlot>());
                    }
                }
                else
                {
                    GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                    card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.All,CardClass.BladeMaster));
                    cardSlots.Add(card.GetComponent<CardSlot>());
                }
                break;
            case CardClass.ShootingCaster:
                if (cardShoot.Count > 1)
                {
                    for (int a = 0; a < count; a++)
                    {
                        if (a > cardShoot.Count + 1)
                        { 
                            continue;
                        }
                        GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                        card.GetComponent<CardSlot>().SetCard(a,RandomCardInList(ArtifactGrade.All,CardClass.ShootingCaster));
                        cardSlots.Add(card.GetComponent<CardSlot>());
                    }
                }
                else
                {
                    GameObject card = Instantiate(cardSlotPrefab, cardSlotParent);
                    card.GetComponent<CardSlot>().SetCard(0,RandomCardInList(ArtifactGrade.All,CardClass.ShootingCaster));
                    cardSlots.Add(card.GetComponent<CardSlot>());
                }
                break;
        }
        
        CloseButton();
    }

    private void SortingCardGrade()
    {
        foreach (ArtifactData card in cardList)
        {
            switch (card.artifactGrade)
            {
                case ArtifactGrade.Common:
                    cardCommon.Add(card);
                    break;
                case ArtifactGrade.Rare:
                    cardRare.Add(card);
                    break;
                case ArtifactGrade.Epic:
                    cardEpic.Add(card);
                    break;
            }

            switch (card.artifactClass)
            {
                case CardClass.SwordKnight:
                    cardKnight.Add(card);
                    break;
                case CardClass.BladeMaster:
                    cardBlade.Add(card);
                    break;
                case CardClass.ShootingCaster:
                    cardShoot.Add(card);
                    break;
            }
        }
    }

    private ArtifactData RandomCardInList(ArtifactGrade gradeToRandom,CardClass classToRandom)
    {
        ArtifactData cardData = null;
        if (classToRandom == CardClass.Normal)
        {
            float randomGradeNumber = Random.Range(0f, 1f);
            switch (gradeToRandom)
            {
                case ArtifactGrade.Common:
                    int randomNumberCommon = Random.Range(0, cardCommon.Count - 1);
                    cardData = cardCommon[randomNumberCommon];
                    currentCardRandom.Add(cardCommon[randomNumberCommon]);
                    cardList.Remove(cardCommon[randomNumberCommon]);
                    cardCommon.Remove(cardCommon[randomNumberCommon]);
                    switch (cardData.artifactClass)
                    {
                        case CardClass.SwordKnight: 
                            cardKnight.Remove(cardData);
                            break;
                        case CardClass.BladeMaster:
                            cardBlade.Remove(cardData);
                            break;
                        case CardClass.ShootingCaster:
                            cardShoot.Remove(cardData);
                            break;
                    }
                    break;
                case ArtifactGrade.Rare:
                    int randomNumberRare = Random.Range(0, cardRare.Count - 1);
                    cardData = cardRare[randomNumberRare];
                    currentCardRandom.Add(cardRare[randomNumberRare]);
                    cardList.Remove(cardRare[randomNumberRare]);
                    cardRare.Remove(cardRare[randomNumberRare]);
                    switch (cardData.artifactClass)
                    {
                        case CardClass.SwordKnight:
                            cardKnight.Remove(cardData);
                            break;
                        case CardClass.BladeMaster:
                            cardBlade.Remove(cardData);
                            break;
                        case CardClass.ShootingCaster:
                            cardShoot.Remove(cardData);
                            break;
                    }
                    break;
                case ArtifactGrade.Epic:
                    int randomNumberEpic = Random.Range(0, cardEpic.Count - 1);
                    cardData = cardEpic[randomNumberEpic];
                    currentCardRandom.Add(cardEpic[randomNumberEpic]);
                    cardList.Remove(cardEpic[randomNumberEpic]);
                    cardEpic.Remove(cardEpic[randomNumberEpic]);
                    switch (cardData.artifactClass)
                    {
                        case CardClass.SwordKnight:
                            cardKnight.Remove(cardData);
                            break;
                        case CardClass.BladeMaster:
                            cardBlade.Remove(cardData);
                            break;
                        case CardClass.ShootingCaster:
                            cardShoot.Remove(cardData);
                            break;
                    }
                    break;
                default:
                    if (randomGradeNumber < commonRate)
                    {
                        if (cardCommon.Count > 0)
                        {
                            int randomNumber = Random.Range(0, cardCommon.Count - 1);
                            cardData = cardCommon[randomNumber];
                            currentCardRandom.Add(cardCommon[randomNumber]);
                            cardList.Remove(cardCommon[randomNumber]);
                            cardCommon.Remove(cardCommon[randomNumber]);
                            switch (cardData.artifactClass)
                            {
                                case CardClass.SwordKnight:
                                    cardKnight.Remove(cardData);
                                    break;
                                case CardClass.BladeMaster:
                                    cardBlade.Remove(cardData);
                                    break;
                                case CardClass.ShootingCaster:
                                    cardShoot.Remove(cardData);
                                    break;
                            }
                        }
                        else
                        {
                            if (cardRare.Count > 0)
                            {
                                int randomNumber = Random.Range(0, cardRare.Count - 1);
                                cardData = cardRare[randomNumber];
                                currentCardRandom.Add(cardRare[randomNumber]);
                                cardList.Remove(cardRare[randomNumber]);
                                cardRare.Remove(cardRare[randomNumber]);
                                switch (cardData.artifactClass)
                                {
                                    case CardClass.SwordKnight:
                                        cardKnight.Remove(cardData);
                                        break;
                                    case CardClass.BladeMaster:
                                        cardBlade.Remove(cardData);
                                        break;
                                    case CardClass.ShootingCaster:
                                        cardShoot.Remove(cardData);
                                        break;
                                }
                            }
                            else
                            {
                                int randomNumber = Random.Range(0, cardEpic.Count - 1);
                                cardData = cardEpic[randomNumber];
                                currentCardRandom.Add(cardEpic[randomNumber]);
                                cardList.Remove(cardEpic[randomNumber]);
                                cardEpic.Remove(cardEpic[randomNumber]);
                                switch (cardData.artifactClass)
                                {
                                    case CardClass.SwordKnight:
                                        cardKnight.Remove(cardData);
                                        break;
                                    case CardClass.BladeMaster:
                                        cardBlade.Remove(cardData);
                                        break;
                                    case CardClass.ShootingCaster:
                                        cardShoot.Remove(cardData);
                                        break;
                                }
                            }
                        }
                    
                    }
                    else if (randomGradeNumber < rareRate)
                    {
                        if (cardRare.Count > 0)
                        {
                            int randomNumber = Random.Range(0, cardRare.Count - 1);
                            cardData = cardRare[randomNumber];
                            currentCardRandom.Add(cardRare[randomNumber]);
                            cardList.Remove(cardRare[randomNumber]);
                            cardRare.Remove(cardRare[randomNumber]);
                            switch (cardData.artifactClass)
                            {
                                case CardClass.SwordKnight:
                                    cardKnight.Remove(cardData);
                                    break;
                                case CardClass.BladeMaster:
                                    cardBlade.Remove(cardData);
                                    break;
                                case CardClass.ShootingCaster:
                                    cardShoot.Remove(cardData);
                                    break;
                            }
                        }
                        else
                        {
                            if (cardEpic.Count > 0)
                            {
                                int randomNumber = Random.Range(0, cardEpic.Count - 1);
                                cardData = cardEpic[randomNumber];
                                currentCardRandom.Add(cardEpic[randomNumber]);
                                cardList.Remove(cardEpic[randomNumber]);
                                cardEpic.Remove(cardEpic[randomNumber]);
                                switch (cardData.artifactClass)
                                {
                                    case CardClass.SwordKnight:
                                        cardKnight.Remove(cardData);
                                        break;
                                    case CardClass.BladeMaster:
                                        cardBlade.Remove(cardData);
                                        break;
                                    case CardClass.ShootingCaster:
                                        cardShoot.Remove(cardData);
                                        break;
                                }
                            }
                            else
                            {
                                int randomNumber = Random.Range(0, cardCommon.Count - 1);
                                cardData = cardCommon[randomNumber];
                                currentCardRandom.Add(cardCommon[randomNumber]);
                                cardList.Remove(cardCommon[randomNumber]);
                                cardCommon.Remove(cardCommon[randomNumber]);
                                switch (cardData.artifactClass)
                                {
                                    case CardClass.SwordKnight:
                                        cardKnight.Remove(cardData);
                                        break;
                                    case CardClass.BladeMaster:
                                        cardBlade.Remove(cardData);
                                        break;
                                    case CardClass.ShootingCaster:
                                        cardShoot.Remove(cardData);
                                        break;
                                }
                            }
                        }
                    
                    }
                    else
                    {
                        if (cardEpic.Count > 0)
                        {
                            int randomNumber = Random.Range(0, cardEpic.Count - 1);
                            cardData = cardEpic[randomNumber];
                            currentCardRandom.Add(cardEpic[randomNumber]);
                            cardList.Remove(cardEpic[randomNumber]);
                            cardEpic.Remove(cardEpic[randomNumber]);
                            switch (cardData.artifactClass)
                            {
                                case CardClass.SwordKnight:
                                    cardKnight.Remove(cardData);
                                    break;
                                case CardClass.BladeMaster:
                                    cardBlade.Remove(cardData);
                                    break;
                                case CardClass.ShootingCaster:
                                    cardShoot.Remove(cardData);
                                    break;
                            }
                        } 
                        else
                        {
                            if (cardCommon.Count > 0)
                            {
                                int randomNumber = Random.Range(0, cardCommon.Count - 1);
                                cardData = cardCommon[randomNumber];
                                currentCardRandom.Add(cardCommon[randomNumber]);
                                cardList.Remove(cardCommon[randomNumber]);
                                cardCommon.Remove(cardCommon[randomNumber]);
                                switch (cardData.artifactClass)
                                {
                                    case CardClass.SwordKnight:
                                        cardKnight.Remove(cardData);
                                        break;
                                    case CardClass.BladeMaster:
                                        cardBlade.Remove(cardData);
                                        break;
                                    case CardClass.ShootingCaster:
                                        cardShoot.Remove(cardData);
                                        break;
                                }
                            }
                            else
                            {
                                int randomNumber = Random.Range(0, cardRare.Count - 1);
                                cardData = cardRare[randomNumber];
                                currentCardRandom.Add(cardRare[randomNumber]);
                                cardList.Remove(cardRare[randomNumber]);
                                cardRare.Remove(cardRare[randomNumber]);
                                switch (cardData.artifactClass)
                                {
                                    case CardClass.SwordKnight:
                                        cardKnight.Remove(cardData);
                                        break;
                                    case CardClass.BladeMaster:
                                        cardBlade.Remove(cardData);
                                        break;
                                    case CardClass.ShootingCaster:
                                        cardShoot.Remove(cardData);
                                        break;
                                }
                            }
                        }
                    
                    }
                    break;
        }
        }
        else
        {
            switch (classToRandom)
            {
                case CardClass.SwordKnight:
                    int randomNumberKnight = Random.Range(0, cardKnight.Count - 1);
                    cardData = cardKnight[randomNumberKnight];
                    currentCardRandom.Add(cardKnight[randomNumberKnight]);
                    cardList.Remove(cardKnight[randomNumberKnight]);
                    cardKnight.Remove(cardKnight[randomNumberKnight]);
                    switch (cardData.artifactGrade)
                    {
                        case ArtifactGrade.Common:
                            cardCommon.Remove(cardData);
                            break;
                        case ArtifactGrade.Rare:
                            cardRare.Remove(cardData);
                            break;
                        case ArtifactGrade.Epic:
                            cardEpic.Remove(cardData);
                            break;
                    }
                    break;
                case CardClass.BladeMaster:
                    int randomNumberBlade = Random.Range(0, cardBlade.Count - 1);
                    cardData = cardBlade[randomNumberBlade];
                    currentCardRandom.Add(cardBlade[randomNumberBlade]);
                    cardList.Remove(cardBlade[randomNumberBlade]);
                    cardBlade.Remove(cardBlade[randomNumberBlade]);
                    switch (cardData.artifactGrade)
                    {
                        case ArtifactGrade.Common:
                            cardCommon.Remove(cardData);
                            break;
                        case ArtifactGrade.Rare:
                            cardRare.Remove(cardData);
                            break;
                        case ArtifactGrade.Epic:
                            cardEpic.Remove(cardData);
                            break;
                    }
                    break;
                case CardClass.ShootingCaster:
                    int randomNumberShoot = Random.Range(0, cardShoot.Count - 1);
                    cardData = cardShoot[randomNumberShoot];
                    currentCardRandom.Add(cardShoot[randomNumberShoot]);
                    cardList.Remove(cardShoot[randomNumberShoot]);
                    cardShoot.Remove(cardShoot[randomNumberShoot]);
                    switch (cardData.artifactGrade)
                    {
                        case ArtifactGrade.Common:
                            cardCommon.Remove(cardData);
                            break;
                        case ArtifactGrade.Rare: 
                            cardRare.Remove(cardData);
                            break;
                        case ArtifactGrade.Epic:
                            cardEpic.Remove(cardData);
                            break;
                    }
                    break;
            }
        }
        
        
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
            switch (card.artifactGrade)
            {
                case ArtifactGrade.Common:
                    cardCommon.Add(card);
                    break;
                case ArtifactGrade.Rare:
                    cardRare.Add(card);
                    break;
                case ArtifactGrade.Epic: 
                    cardEpic.Add(card);
                    break;
            }

            switch (card.artifactClass)
            {
                case CardClass.SwordKnight:
                    cardKnight.Add(card);
                    break;
                case CardClass.BladeMaster:
                    cardBlade.Add(card);
                    break;
                case CardClass.ShootingCaster: 
                    cardShoot.Add(card);
                    break;
            }
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

    private void CloseButton()
    {
        randomAgainButton.interactable = false;
        randomAgainWithArtifactButton.interactable = false;
        
        randomAgainButton.gameObject.SetActive(false);
        randomAgainWithArtifactButton.gameObject.SetActive(false);
    }

    public void RandomEnd()
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

        if (cardCommon.Count <= 0)
        {
            commonOutOfStock = true;
        }
        
        if (cardRare.Count <= 0)
        {
            rareOutOfStock = true;
        }

        if (cardEpic.Count <= 0)
        {
            epicOutOfStock = true;
        }

        if (cardKnight.Count <= 0)
        {
            cardKnightOutOfStock = true;
        }

        if (cardBlade.Count <= 0)
        {
            cardBladeOutOfStock = true;
        }

        if (cardShoot.Count <= 0)
        {
            cardShootOutOfStock = true;
        }
        player.GetComponent<PlayerMovementGrid>().currentState = MovementState.Idle;
        player.GetComponent<PlayerArtifact>().ResultArtifact();
        isRandom = false;
    }
}
