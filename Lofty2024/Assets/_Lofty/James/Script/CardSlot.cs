using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int cardIndex;
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDetail;
    public GameObject detailBack;
    
    [Header("Card Icons")]
    public GameObject swordIcon, bladeIcon, shootIcon;

    [Header("Highlight Effect")]
    public Material highLightMat;

    private Dictionary<CardClass, GameObject> classIcons;

    private void Awake()
    {
        classIcons = new Dictionary<CardClass, GameObject>
        {
            { CardClass.SwordKnight, swordIcon },
            { CardClass.BladeMaster, bladeIcon },
            { CardClass.ShootingCaster, shootIcon }
        };
    }

    public void SetCard(int index, ArtifactData artifactData)
    {
        cardIndex = index;
        cardImage.sprite = artifactData.artifactImage;
        cardName.text = artifactData.artifactName;
        cardDetail.text = artifactData.artifactDetail;
        
        DisableAllIcons();
        if (classIcons.TryGetValue(artifactData.artifactClass, out GameObject icon))
        {
            icon.SetActive(true);
        }
    }

    private void DisableAllIcons()
    {
        swordIcon.SetActive(false);
        bladeIcon.SetActive(false);
        shootIcon.SetActive(false);
    }

    public void SelectedCard()
    {
        GameManager.Instance.GetComponent<RandomCardManager>().SelectedCard(cardIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (detailBack) detailBack.SetActive(true);
        if (cardImage) cardImage.material = highLightMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (detailBack) detailBack.SetActive(false);
        if (cardImage) cardImage.material = null;
    }
}