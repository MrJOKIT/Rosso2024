using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public GameObject uiPanel; 
    public Button clickButton;
    public Button arrowButton; 
    public Text displayText; 

    private string[] textOptions = { "Option 1", "Option 2", "Option 3" }; 
    private int currentIndex = 0; 

    void Start()
    {
        
        if (clickButton != null)
        {
            clickButton.onClick.AddListener(OnButtonClick);
        }

        if (arrowButton != null)
        {
            arrowButton.onClick.AddListener(OnArrowButtonClick);
        }

      
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        
        UpdateDisplayText();
    }

    void OnButtonClick()
    {
        Debug.Log("Button clicked!");
       

       
       EffectSoundManage.instance.PlaySFX("adriantnt_u_click");
            uiPanel.SetActive(true);
        
    }

    void OnArrowButtonClick()
    {  
        currentIndex = (currentIndex + 1) % textOptions.Length; 
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        
        if (displayText != null)
        {
            displayText.text = textOptions[currentIndex];
        }
    }
}