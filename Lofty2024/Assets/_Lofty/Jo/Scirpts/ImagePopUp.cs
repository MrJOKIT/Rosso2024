using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 

public class ImagePopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject imageToShow; 
    public TMP_Text buttonText; 
//
    void Start()
    {
      
        buttonText = GetComponentInChildren<TMP_Text>();
        
      
        if (buttonText != null)
        {
            buttonText.color = Color.black;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManage.Instance.PlaySound(Sound.Effect1); 
        Debug.Log("Mouse entered!");
        imageToShow.SetActive(true);
        
      
        
        if (buttonText != null)
        {
            buttonText.color = Color.white; 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited!");
        imageToShow.SetActive(false);
        
        if (buttonText != null)
        {
            buttonText.color = Color.black; 
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked!");
      
        SoundManage.Instance.PlaySound(Sound.Effect1); 
    }
}