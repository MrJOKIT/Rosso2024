using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToggleSetting : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjects; 
    [SerializeField] private List<GameObject> resolutionGameObjects; 
    [SerializeField] private List<GameObject> fpsLimitGameObjects; 
    [SerializeField] private TextMeshProUGUI fpsText; 
    [SerializeField] private TextMeshProUGUI fpsValueText; 
    [SerializeField] private Slider fpsSlider;

    private int currentIndex = 0;
    private int resolutionIndex = 0;

    private bool isBorderless = false;

    [SerializeField] private List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(800, 600),
        new Vector2Int(1024, 768),
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    private Vector2Int previewResolution;
    private bool isFPSLimited = false; 
    private bool isVSyncEnabled = false; 
    private int limitedFPS = 60; 
    private float deltaTime = 0.0f;

    private void Start()
    {
        InitializeGameObjects();
        InitializeResolutionGameObjects();
        InitializeFPSLimitGameObjects();
        previewResolution = resolutions[resolutionIndex];

        fpsSlider.minValue = 30; 
        fpsSlider.maxValue = 240; 
        fpsSlider.value = limitedFPS; 
        fpsValueText.text = limitedFPS.ToString(); 
        fpsSlider.gameObject.SetActive(false); 
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"{fps:0.}";
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

    private void InitializeFPSLimitGameObjects()
    {
        foreach (var obj in fpsLimitGameObjects)
        {
            obj.SetActive(false);
        }
        UpdateFPSLimitGameObject();
    }

    public void ToggleScreenMode()
    {
        if (gameObjects.Count == 0) return;

        gameObjects[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % gameObjects.Count; // เปลี่ยนค่า index ของ gameObjects

        gameObjects[currentIndex].SetActive(true);
        Debug.Log("Toggled GameObject: " + gameObjects[currentIndex].name);
    }

    public void ToggleResolutionGameObject()
    {
        if (resolutionGameObjects.Count == 0) return;

        resolutionGameObjects[resolutionIndex].SetActive(false);
        resolutionIndex = (resolutionIndex + 1) % resolutions.Count;
        resolutionGameObjects[resolutionIndex].SetActive(true);

        previewResolution = resolutions[resolutionIndex];
        Debug.Log("Toggled Resolution GameObject: " + resolutionGameObjects[resolutionIndex].name);
    }

    public void ToggleFPS()
    {
        isFPSLimited = !isFPSLimited; 
        fpsSlider.gameObject.SetActive(isFPSLimited);

        if (isFPSLimited)
        {
            fpsValueText.text = ((int)fpsSlider.value).ToString();
        }
        else
        {
            fpsValueText.text = "Uncapped"; 
        }

        UpdateFPSLimitGameObject(); 
        Debug.Log("FPS Limit toggled to: " + (isFPSLimited ? limitedFPS.ToString() : "Uncapped"));
    }

    private void UpdateFPSLimitGameObject()
    {
        for (int i = 0; i < fpsLimitGameObjects.Count; i++)
        {
            fpsLimitGameObjects[i].SetActive(i == (isFPSLimited ? 1 : 0));
        }
    }

    public void ApplySettings()
    {
        // ตั้งค่าหน้าจอที่นี่ตาม currentIndex
        switch (currentIndex)
        {
            case 0: // Fullscreen
                Screen.SetResolution(previewResolution.x, previewResolution.y, true);
                Debug.Log("Applied Fullscreen mode.");
                break;
            case 1: // Windowed
                Screen.SetResolution(previewResolution.x, previewResolution.y, false);
                Debug.Log("Applied Windowed mode.");
                break;
            case 2: // Borderless
                Screen.SetResolution(previewResolution.x, previewResolution.y, true); // ใช้ true สำหรับ Borderless
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // ตั้งค่าให้เป็น Borderless
                Debug.Log("Applied Borderless mode.");
                break;
        }

        // กำหนด FPS และ V-Sync
        if (isVSyncEnabled)
        {
            QualitySettings.vSyncCount = 1; 
            Application.targetFrameRate = -1; 
            Debug.Log("V-Sync is enabled, FPS limit is disabled.");
        }
        else if (isFPSLimited)
        {
            limitedFPS = (int)fpsSlider.value; 
            Application.targetFrameRate = limitedFPS; 
            Debug.Log($"FPS limit set to: {limitedFPS}");
        }
        else
        {
            Application.targetFrameRate = -1; 
        }

        Debug.Log($"Settings applied. Current GameObject: {gameObjects[currentIndex].name}, Resolution: {previewResolution.x}x{previewResolution.y}, FPS Limit: {(isFPSLimited ? limitedFPS.ToString() : "Uncapped")}");
        fpsValueText.text = isFPSLimited ? limitedFPS.ToString() : "Uncapped"; 
    }

    public void ChangeResolutionPreview()
    {
        resolutionIndex = (resolutionIndex + 1) % resolutions.Count;
        previewResolution = resolutions[resolutionIndex];
        Debug.Log($"Preview resolution changed to: {previewResolution.x}x{previewResolution.y}");
    }

    public void OnSliderValueChanged()
    {
        if (isFPSLimited)
        {
            fpsValueText.text = ((int)fpsSlider.value).ToString();
            limitedFPS = (int)fpsSlider.value; 
        }
    }

    public void ExitGame()
    {
        Application.Quit(); 
        Debug.Log("Game is exiting...");
    }

    public void ToggleVSync()
    {
        isVSyncEnabled = !isVSyncEnabled;
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;

        Debug.Log($"V-Sync is now {(isVSyncEnabled ? "enabled" : "disabled")}");
    }
}
