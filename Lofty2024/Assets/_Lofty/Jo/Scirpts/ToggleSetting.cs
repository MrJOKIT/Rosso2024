using System.Collections.Generic;
using UnityEngine;

public class ToggleSetting : MonoBehaviour
{
    public List<GameObject> gameObjects; // Main GameObjects
    public List<GameObject> resolutionGameObjects; // Resolution-related GameObjects
    private int currentIndex = 0;
    private int resolutionIndex = 0;

    public List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720),
        new Vector2Int(800, 600)
    };

    private Vector2Int previewResolution;

    private void Start()
    {
        InitializeGameObjects();
        InitializeResolutionGameObjects();
        previewResolution = resolutions[resolutionIndex];
    }

    private void InitializeGameObjects()
    {
        foreach (var obj in gameObjects)
        {
            obj.SetActive(false);
        }
        if (gameObjects.Count > 0)
        {
            gameObjects[currentIndex].SetActive(true);
        }
    }

    private void InitializeResolutionGameObjects()
    {
        foreach (var obj in resolutionGameObjects)
        {
            obj.SetActive(false);
        }
        if (resolutionGameObjects.Count > 0)
        {
            resolutionGameObjects[resolutionIndex].SetActive(true);
        }
    }

    public void ToggleGameObject()
    {
        if (gameObjects.Count == 0) return;

        gameObjects[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % gameObjects.Count;
        gameObjects[currentIndex].SetActive(true);
        Debug.Log("Toggled GameObject: " + gameObjects[currentIndex].name);
    }

    public void ToggleResolutionGameObject()
    {
        if (resolutionGameObjects.Count == 0) return;

        resolutionGameObjects[resolutionIndex].SetActive(false);
        resolutionIndex = (resolutionIndex + 1) % resolutionGameObjects.Count;
        resolutionGameObjects[resolutionIndex].SetActive(true);
        
        previewResolution = resolutions[resolutionIndex];
        Debug.Log("Toggled Resolution GameObject: " + resolutionGameObjects[resolutionIndex].name);
    }

    public void ApplySettings()
    {
        // เปลี่ยนความละเอียดก่อนที่จะเปลี่ยนโหมดเต็มจอ
        Screen.SetResolution(previewResolution.x, previewResolution.y, Screen.fullScreen);
        
        // เรียก ToggleFullScreen เพื่อปรับโหมดตาม currentIndex
        ToggleFullScreen(); 

        Debug.Log($"Settings applied. Full Screen: {Screen.fullScreen}, Current GameObject: {gameObjects[currentIndex].name}, Resolution: {previewResolution.x}x{previewResolution.y}");
    }

    private void ToggleFullScreen()
    {
        // เช็คค่าของ currentIndex
        if (currentIndex == 0)
        {
            Screen.fullScreen = true;  // ถ้า currentIndex เป็น 0 ให้เป็นโหมดเต็มจอ
        }
        else if (currentIndex == 1)
        {
            Screen.fullScreen = false; // ถ้า currentIndex เป็น 1 ให้เป็นโหมดหน้าต่าง
        }

        Debug.Log("Full screen mode is now: " + Screen.fullScreen);
    }

    public void ChangeResolutionPreview()
    {
        resolutionIndex = (resolutionIndex + 1) % resolutions.Count;
        previewResolution = resolutions[resolutionIndex];
        Screen.SetResolution(previewResolution.x, previewResolution.y, Screen.fullScreen);
        Debug.Log($"Preview resolution changed to: {previewResolution.x}x{previewResolution.y}");
    }
}
