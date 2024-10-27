using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public int cardIndex;
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDetail;
    [Space(10)] 
    public Image cardClassIcon;
    public Sprite swordIcon;
    public Sprite bladeIcon;
    public Sprite shootIcon;

    public void SetCard(int cardIndex,ArtifactData artifactData)
    {
        this.cardIndex = cardIndex;
        this.cardImage.sprite = artifactData.artifactImage;
        this.cardName.text = artifactData.artifactName;
        this.cardDetail.text = artifactData.artifactDetail;
        switch (artifactData.artifactClass)
        {
            case CardClass.SwordKnight:
                cardClassIcon.sprite = swordIcon;
                break;
            case CardClass.BladeMaster:
                cardClassIcon.sprite = bladeIcon;
                break;
            case CardClass.ShootingCaster:
                cardClassIcon.sprite = shootIcon;
                break;
            default:
                cardClassIcon.gameObject.SetActive(false);
                break;
        }
    }

    public void Reveal()
    {
        cardDetail.GetComponent<TextRevealer>().RevealTime = 0.5f;
        cardDetail.GetComponent<TextRevealer>().Reveal();
    }

    public void SelectedCard()
    {
        GameManager.Instance.GetComponent<RandomCardManager>().SelectedCard(cardIndex);
    }
}
