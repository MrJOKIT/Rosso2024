using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToggleSetting : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjects;
    [SerializeField] private List<GameObject> resolutionGameObjects;
    [SerializeField] private List<GameObject> fpsLimitGameObjects;
    [SerializeField] private List<GameObject> vsyncGameObjects; // VSYNC game objects

    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI fpsValueText;
    [SerializeField] private Slider fpsSlider;

    private int currentIndex = 0;
    private int resolutionIndex = 0;

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
    private bool isWindowedMode = true;  // Track windowed vs fullscreen mode
    private float deltaTime = 0.0f;

    private void Start()
    {
        // Load and apply saved settings
        SettingsManager.Instance.LoadSettings();
        previewResolution = SettingsManager.Instance.resolution;
        isVSyncEnabled = SettingsManager.Instance.isVSyncEnabled;
        isFPSLimited = SettingsManager.Instance.isFPSLimited;
        limitedFPS = SettingsManager.Instance.limitedFPS;
        isWindowedMode = SettingsManager.Instance.isWindowedMode;

        // Use the saved indices for initialization
        currentIndex = SettingsManager.Instance.currentGameObjectIndex;
        resolutionIndex = SettingsManager.Instance.currentResolutionIndex;

        // Initialize game objects, resolution, FPS settings
        InitializeGameObjects();
        InitializeResolutionGameObjects();
        InitializeFPSLimitGameObjects();

        // Set up the FPS slider and display
        fpsSlider.minValue = 30;
        fpsSlider.maxValue = 240;
        UpdateFPSSlider();

        // Update visibility of V-Sync GameObjects based on the saved setting
        UpdateVSyncGameObjects();
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

    private void UpdateFPSSlider()
    {
        if (isFPSLimited)
        {
            fpsSlider.value = Mathf.Clamp(limitedFPS, 30, 240);
            fpsValueText.text = limitedFPS.ToString();
            fpsSlider.gameObject.SetActive(true); // Show slider if FPS is limited
        }
        else
        {
            fpsValueText.text = "Uncapped";
            fpsSlider.gameObject.SetActive(false); // Hide slider if FPS is uncapped
        }
    }

    // Update V-Sync GameObjects visibility based on the current setting
    private void UpdateVSyncGameObjects()
    {
        if (vsyncGameObjects.Count > 0)
        {
            vsyncGameObjects[0].SetActive(!isVSyncEnabled); // If V-Sync is off
            if (vsyncGameObjects.Count > 1)
            {
                vsyncGameObjects[1].SetActive(isVSyncEnabled); // If V-Sync is on
            }
        }
    }

    public void ToggleVSync()
    {
        isVSyncEnabled = !isVSyncEnabled;
        UpdateVSyncGameObjects();
        Debug.Log($"V-Sync state changed to: {(isVSyncEnabled ? "enabled" : "disabled")}");
        
        SettingsManager.Instance.isVSyncEnabled = isVSyncEnabled;
        SettingsManager.Instance.SaveSettings();
    }

    public void ToggleScreenMode()
    {
        // Cycle through: Windowed -> Borderless -> Fullscreen -> Windowed
        currentIndex = (currentIndex + 1) % gameObjects.Count;

        // Determine the current screen mode
        if (currentIndex == 0)
        {
            isWindowedMode = false; // Fullscreen
        }
        else if (currentIndex == 1)
        {
            isWindowedMode = true; // Windowed
        }
        else if (currentIndex == 2)
        {
            isWindowedMode = false; // Borderless
        }

        // Save the new screen mode setting
        SettingsManager.Instance.currentGameObjectIndex = currentIndex;
        SettingsManager.Instance.isWindowedMode = isWindowedMode;
        SettingsManager.Instance.SaveSettings();

        // Update the game object visibility
        InitializeGameObjects();

        Debug.Log("Saved Screen Mode: " + (isWindowedMode ? "Windowed" : (currentIndex == 1 ? "Borderless" : "Fullscreen")));
    }

private void SetWindowedMode()
{
    // Set the screen to windowed mode (you can set the desired window size here)
    Screen.SetResolution(1280, 720, false); // Example resolution (adjust as needed)
    // Ensure the window is not maximized or borderless
    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
    {
        // On Windows, we typically want to make sure the window is not borderless
        // This requires additional platform-specific methods (e.g., using `WindowManager` libraries for full control)
    }
    // For now, just ensure we are in Windowed mode
    Screen.fullScreen = false;
}

private void SetBorderlessWindow()
{
    // Use the same screen resolution as the monitor's resolution
    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
    // Set to windowed but without borders
    Screen.fullScreen = false;
    if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
    {
        // Example for Windows, may need a native plugin for full borderless control
        // WindowManager.SetWindowBorderless(); // This is platform-dependent, using a plugin for full borderless mode.
    }
    Debug.Log("Switching to Borderless Window Mode");
}

private void SetFullScreenMode()
{
    // Set to fullscreen
    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    Screen.fullScreen = true;
    Debug.Log("Switching to Fullscreen Mode");
}

    public void ToggleResolutionGameObject()
    {
        resolutionIndex = (resolutionIndex + 1) % resolutions.Count;
        previewResolution = resolutions[resolutionIndex];

        // Update the resolution display
        InitializeResolutionGameObjects();

        // Save the new resolution
        SettingsManager.Instance.currentResolutionIndex = resolutionIndex;
        SettingsManager.Instance.resolution = previewResolution;
        SettingsManager.Instance.SaveSettings();
        Debug.Log("Toggled Resolution GameObject: " + resolutionGameObjects[resolutionIndex].name);
    }

    public void ToggleFPS()
    {
        isFPSLimited = !isFPSLimited;
        SettingsManager.Instance.isFPSLimited = isFPSLimited;
        
        UpdateFPSLimitGameObject();
        fpsSlider.gameObject.SetActive(isFPSLimited);

        UpdateFPSSlider();
        SettingsManager.Instance.SaveSettings();
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
        // Apply the settings from the SettingsManager
        SettingsManager.Instance.ApplySettings(); 
        Debug.Log($"Settings applied. Resolution: {previewResolution.x}x{previewResolution.y}, FPS Limit: {(isFPSLimited ? limitedFPS.ToString() : "Uncapped")}");
        fpsValueText.text = isFPSLimited ? limitedFPS.ToString() : "Uncapped";
    }

    public void OnSliderValueChanged()
    {
        if (isFPSLimited)
        {
            limitedFPS = Mathf.Clamp((int)fpsSlider.value, 30, 240);
            fpsValueText.text = limitedFPS.ToString();

            SettingsManager.Instance.limitedFPS = limitedFPS;
            SettingsManager.Instance.SaveSettings();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting...");
    }
}
