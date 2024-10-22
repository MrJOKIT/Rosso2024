using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButtonColorChanger : MonoBehaviour
{
    public Button myButton; 
    private Image buttonImage; 

    public TextMeshProUGUI text1; // เพิ่มการอ้างอิงสำหรับ TextMeshPro ตัวที่ 1
    public TextMeshProUGUI text2; // เพิ่มการอ้างอิงสำหรับ TextMeshPro ตัวที่ 2

    public Color normalColor = Color.white; // Default color
    public Color pressedColor = Color.red;

    private bool isPressed;

    private void Start()
    {
        buttonImage = myButton.GetComponent<Image>();
        buttonImage.color = normalColor; 

        // เริ่มต้นให้ TextMeshPro ไม่แสดง
        if (text1 != null)
        {
            text1.gameObject.SetActive(false);
        }
        if (text2 != null)
        {
            text2.gameObject.SetActive(false);
        }
    }

    public void OnButtonClick()
    {
        isPressed = !isPressed;

        buttonImage.color = isPressed ? pressedColor : normalColor;

        // เปิดหรือปิด TextMeshPro ตามสถานะ
        if (text1 != null)
        {
            text1.gameObject.SetActive(isPressed);
        }
        if (text2 != null)
        {
            text2.gameObject.SetActive(isPressed);
        }
    }
}