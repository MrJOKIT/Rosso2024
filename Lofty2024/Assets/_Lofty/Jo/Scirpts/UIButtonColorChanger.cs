using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButtonColorChanger : MonoBehaviour
{
    public Button myButton; 
    public TextMeshProUGUI text1; 
    public TextMeshProUGUI text3; 

    private bool isPressed;

    private void Start()
    {
        if (text3 != null)
        {
            text3.gameObject.SetActive(true); 
        }
        
        if (text1 != null)
        {
            text1.gameObject.SetActive(false); 
        }
    }

    public void OnButtonClick()
    {
        isPressed = !isPressed;

        if (text1 != null)
        {
            text1.gameObject.SetActive(isPressed);
        }

        if (text3 != null)
        {
            text3.gameObject.SetActive(!isPressed);
        }
    }
}