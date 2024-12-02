using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public int cardIndex;
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDetail;
    public GameObject detailBack;
    [Space(10)] 
    public GameObject swordIcon;
    public GameObject bladeIcon;
    public GameObject shootIcon;
    [Space(10)] 
    public Material highLightMat;

    public void SetCard(int cardIndex,ArtifactData artifactData)
    {
        this.cardIndex = cardIndex;
        this.cardImage.sprite = artifactData.artifactImage;
        this.cardName.text = artifactData.artifactName;
        this.cardDetail.text = artifactData.artifactDetail;
        switch (artifactData.artifactClass)
        {
            case CardClass.SwordKnight:
                swordIcon.SetActive(true);
                shootIcon.SetActive(false);
                bladeIcon.SetActive(false);
                break;
            case CardClass.BladeMaster:
                bladeIcon.SetActive(true);
                shootIcon.SetActive(false);
                swordIcon.SetActive(false);
                break;
            case CardClass.ShootingCaster:
                shootIcon.SetActive(true);
                swordIcon.SetActive(false);
                bladeIcon.SetActive(false);
                break;
            default:
                swordIcon.SetActive(false);
                bladeIcon.SetActive(false);
                shootIcon.SetActive(false);
                break;
        }
    }

    
    

    public void SelectedCard()
    {
        GameManager.Instance.GetComponent<RandomCardManager>().SelectedCard(cardIndex);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        detailBack.SetActive(true);
        cardImage.material = highLightMat;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        detailBack.SetActive(false);
        cardImage.material = null;
    }
}
