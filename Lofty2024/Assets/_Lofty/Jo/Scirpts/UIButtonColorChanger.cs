using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButtonColorChanger : MonoBehaviour
{
    public Button myButton; 
    private Image buttonImage; 

    public TextMeshProUGUI text1; 
  
    public TextMeshProUGUI text3; 
    

    public Color normalColor = Color.white; // Default color
    public Color pressedColor = Color.red;

    private bool isPressed;

    private void Start()
    {
        buttonImage = myButton.GetComponent<Image>();
        buttonImage.color = normalColor;

        text3.gameObject.SetActive(true);
        if (text1 != null)
        {
            text1.gameObject.SetActive(false);
        }
       

       
    }

    public void OnButtonClick()
    {
        isPressed = !isPressed;
        buttonImage.color = isPressed ? pressedColor : normalColor;

        if (text1 != null)
        {
            text1.gameObject.SetActive(isPressed);
        }
        

       
        text3.gameObject.SetActive(!isPressed); 
        
       
    }
}
