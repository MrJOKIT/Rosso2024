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

    public void SetCard(int cardIndex,ArtifactData artifactData)
    {
        this.cardIndex = cardIndex;
        this.cardImage.sprite = artifactData.artifactImage;
        this.cardName.text = artifactData.artifactName;
        this.cardDetail.text = artifactData.artifactDetail;
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
