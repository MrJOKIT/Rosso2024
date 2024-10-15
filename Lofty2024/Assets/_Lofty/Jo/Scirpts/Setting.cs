using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public GameObject uiPanel; 
    public Button clickButton;
    public Button arrowButton; 
    public Text displayText; // Text ที่จะเปลี่ยนแปลง

    private string[] textOptions = { "Option 1", "Option 2", "Option 3" }; // ตัวเลือกข้อความ
    private int currentIndex = 0; // ตัวแปรติดตามตำแหน่งข้อความ

    void Start()
    {
        // ตรวจสอบว่ามีการตั้งค่าปุ่มและ UI Panel หรือไม่
        if (clickButton != null)
        {
            clickButton.onClick.AddListener(OnButtonClick);
        }

        if (arrowButton != null)
        {
            arrowButton.onClick.AddListener(OnArrowButtonClick);
        }

        // ซ่อน UI Panel เริ่มต้น
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        // แสดงข้อความเริ่มต้น
        UpdateDisplayText();
    }

    void OnButtonClick()
    {
        Debug.Log("Button clicked!");
        SoundManage.Instance.PlaySound(Sound.Effect2); 

        // แสดงหรือซ่อน UI Panel
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf); // สลับการแสดงผล
        }
    }

    void OnArrowButtonClick()
    {
        // เปลี่ยนข้อความเมื่อกดปุ่มลูกศร
        currentIndex = (currentIndex + 1) % textOptions.Length; // เปลี่ยนไปยังข้อความถัดไป
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        // อัพเดตข้อความใน Text component
        if (displayText != null)
        {
            displayText.text = textOptions[currentIndex];
        }
    }
}