using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 

public class ImagePopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject imageToShow; 
    public TMP_Text buttonText; 
    private float originalFontSize;

    void Start()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        
        if (buttonText != null)
        {
            buttonText.color = Color.white;
            originalFontSize = buttonText.fontSize;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered!");
        imageToShow.SetActive(true);
        
        if (buttonText != null)
        {
            buttonText.color = Color.yellow; 
            buttonText.fontSize = originalFontSize * 1.1f; 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited!");
        imageToShow.SetActive(false);
        
        if (buttonText != null)
        {
            buttonText.color = Color.white; 
            buttonText.fontSize = originalFontSize; 
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked!");
        EffectSoundManage.instance.PlaySFX("adriantnt_u_click");
    }

    public void ChangeMusic()
    {
        MusicVolumeSetting.instance.ChangeMusic("swift-valkyrie-remastered-229741");
    }
    public void ChangeBack()
    {
        MusicVolumeSetting.instance.ChangeMusic("Voicy_Yu-Gi-Oh! - Millennium Battle Theme");
    }
}