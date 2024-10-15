using System.Collections.Generic;
using UnityEngine;

public class ToggleSetting : MonoBehaviour
{
    public List<GameObject> gameObjects; // รายการ GameObject ที่จะแสดง
    private int currentIndex = 0; // ตัวแปรสำหรับติดตามตำแหน่งปัจจุบัน
    private bool settingsApplied = false; // ตัวแปรสำหรับตรวจสอบว่าการตั้งค่าได้ถูกนำไปใช้หรือยัง

    private void Start()
    {
        // ปิด GameObject ทั้งหมดก่อน
        foreach (var obj in gameObjects)
        {
            obj.SetActive(false);
        }

        // เปิด GameObject ตัวแรก (เช่น Full Screen)
        if (gameObjects.Count > 0)
        {
            gameObjects[currentIndex].SetActive(true);
        }
    }

    private void Update()
    {
        // เช็คการกดปุ่มลูกศร (Space) เพื่อเลือกการตั้งค่า
        if (!settingsApplied && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleGameObject();
        }
        // เช็คการกดปุ่ม Apply (Enter)
        else if (settingsApplied && Input.GetKeyDown(KeyCode.Return))
        {
            ApplySettings();
        }
    }

    public void ToggleGameObject()
    {
        // ปิด GameObject ปัจจุบันก่อน
        gameObjects[currentIndex].SetActive(false);

        // อัปเดตดัชนีไปยัง GameObject ถัดไป
        currentIndex = (currentIndex + 1) % gameObjects.Count;

        // เปิด GameObject ใหม่ (เช่น Full Screen หรือ Window)
        gameObjects[currentIndex].SetActive(true);

        // ตั้งค่าให้ settingsApplied เป็น true เพื่อแสดงว่ามีการเลือกแล้ว
        settingsApplied = true;
    }

    public void ApplySettings()
    {
        // สลับระหว่าง Full Screen และ Window
        ToggleFullScreen();

        // บันทึกการตั้งค่าหรือทำการนำไปใช้ที่นี่
        Debug.Log("Settings applied. Full Screen: " + Screen.fullScreen + ", Current GameObject: " + gameObjects[currentIndex].name);

        // รีเซ็ตค่า settingsApplied เพื่อให้สามารถเลือกใหม่ได้อีกครั้ง
        settingsApplied = false;
    }

    private void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
