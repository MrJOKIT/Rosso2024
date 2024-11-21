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
        LoadSettings(); 
        InitializeGameObjects();
        InitializeResolutionGameObjects();
        InitializeFPSLimitGameObjects();

        fpsSlider.minValue = 30;
        fpsSlider.maxValue = 240;
        fpsSlider.value = limitedFPS;
        fpsValueText.text = limitedFPS.ToString();
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
        currentIndex = (currentIndex + 1) % gameObjects.Count;

        gameObjects[currentIndex].SetActive(true);
        SaveSettings(); // Save settings when toggled
        Debug.Log("Toggled GameObject: " + gameObjects[currentIndex].name);
    }

    public void ToggleResolutionGameObject()
    {
        if (resolutionGameObjects.Count == 0) return;

        resolutionGameObjects[resolutionIndex].SetActive(false);
        resolutionIndex = (resolutionIndex + 1) % resolutions.Count;
        resolutionGameObjects[resolutionIndex].SetActive(true);

        previewResolution = resolutions[resolutionIndex];
        SaveSettings(); // Save settings when toggled
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
        SaveSettings(); // Save settings when toggled
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
    
    switch (currentIndex)
    {
        case 0: 
         
            if (IsResolutionSupported(previewResolution))
            {
               
                Screen.SetResolution(previewResolution.x, previewResolution.y, true); // Fullscreen
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; // Ensures exclusive fullscreen
                Debug.Log("Applied Fullscreen mode.");
            }
            else
            {
                Debug.LogWarning("Selected resolution is not supported.");
            }
            break;

        case 1: 
           
            if (IsResolutionSupported(previewResolution))
            {
               
                Screen.SetResolution(previewResolution.x, previewResolution.y, false); 
                Screen.fullScreenMode = FullScreenMode.Windowed; // Explicitly set to windowed
                Debug.Log("Applied Windowed mode.");
            }
            else
            {
                Debug.LogWarning("Selected resolution is not supported.");
            }
            break;

        case 2: // Borderless
           
            if (IsResolutionSupported(previewResolution))
            {
               
                Screen.SetResolution(previewResolution.x, previewResolution.y, true); 
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // Borderless
                Debug.Log("Applied Borderless mode.");
            }
            else
            {
                Debug.LogWarning("Selected resolution is not supported.");
            }
            break;
    }

    
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

private bool IsResolutionSupported(Vector2Int resolution)
{
    foreach (var res in Screen.resolutions)
    {
        if (res.width == resolution.x && res.height == resolution.y)
        {
            return true;
        }
    }
    return false;
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
            SaveSettings(); // Save the FPS limit when changed
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

    
    private void SaveSettings()
    {
        PlayerPrefs.SetInt("ScreenModeIndex", currentIndex);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.SetInt("FPSLimit", isFPSLimited ? limitedFPS : -1);
        PlayerPrefs.SetInt("VSync", isVSyncEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

   
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ScreenModeIndex"))
        {
            currentIndex = PlayerPrefs.GetInt("ScreenModeIndex");
            resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            limitedFPS = PlayerPrefs.GetInt("FPSLimit", 60);
            isFPSLimited = limitedFPS != -1;
            isVSyncEnabled = PlayerPrefs.GetInt("VSync") == 1;
        }
    }
}
