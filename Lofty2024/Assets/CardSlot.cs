using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDetail;

    public void SetCard(Sprite cardImage,string cardName,string cardDetail)
    {
        this.cardImage.sprite = cardImage;
        this.cardName.text = cardName;
        this.cardDetail.text = cardDetail;
    }
}
