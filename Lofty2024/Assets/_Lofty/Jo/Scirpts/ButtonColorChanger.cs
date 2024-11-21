using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    [SerializeField] private Button button1;  
    [SerializeField] private Button button2;  
    [SerializeField] private Button button3;  

    [SerializeField] private Color whiteColor = Color.white;
    [SerializeField] private Color grayColor = Color.gray;

    void Start()
    {
        button1.GetComponent<Image>().color = whiteColor;
        button2.GetComponent<Image>().color = grayColor;
        button3.GetComponent<Image>().color = grayColor;

        button1.onClick.AddListener(() => ChangeButtonColor(button1));
        button2.onClick.AddListener(() => ChangeButtonColor(button2));
        button3.onClick.AddListener(() => ChangeButtonColor(button3));
    }

    void ChangeButtonColor(Button clickedButton)
    {
        button1.GetComponent<Image>().color = grayColor;
        button2.GetComponent<Image>().color = grayColor;
        button3.GetComponent<Image>().color = grayColor;

        clickedButton.GetComponent<Image>().color = whiteColor;
    }
}
