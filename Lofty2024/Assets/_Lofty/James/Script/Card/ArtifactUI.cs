using System;
using System.Collections;
using System.Collections.Generic;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public TextMeshProUGUI artifactName;
    public Image artifactImage;
    public Sprite defaultImage;
    public GameObject hoverObject;
    public TextMeshProUGUI cardDetail;

    public void SetArtifactUI(string artifactName,Sprite artifactSprite,string cardDetail)
    {
        this.artifactName.text = artifactName;
        artifactImage.sprite = artifactSprite;
        this.cardDetail.text = cardDetail;
    }

    public void ClearArtifactSlot()
    {
        artifactName.text = "Empty";
        artifactImage.sprite = defaultImage;
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverObject.SetActive(false);
    }
}
