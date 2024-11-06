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
            originalFontSize = buttonText.fontSize; // Store the original font size
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManage.Instance.PlaySound(Sound.Effect1); 
        Debug.Log("Mouse entered!");
        imageToShow.SetActive(true);
        
        if (buttonText != null)
        {
            buttonText.color = Color.yellow; 
            buttonText.fontSize = originalFontSize * 1.1f; // Increase font size by 10%
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited!");
        imageToShow.SetActive(false);
        
        if (buttonText != null)
        {
            buttonText.color = Color.white; 
            buttonText.fontSize = originalFontSize; // Restore original font size
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked!");
        SoundManage.Instance.PlaySound(Sound.Effect1); 
    }
}