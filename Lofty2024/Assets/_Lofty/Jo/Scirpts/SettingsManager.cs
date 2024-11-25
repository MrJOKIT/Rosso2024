using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public int currentGameObjectIndex;
    public int currentResolutionIndex;

    public Vector2Int resolution;
    public bool isFPSLimited;
    public int limitedFPS;
    public bool isVSyncEnabled;
    public bool isWindowedMode; // Track windowed/fullscreen setting

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadSettings(); // Load settings from PlayerPrefs
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionWidth", resolution.x);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.y);
        PlayerPrefs.SetInt("FPSLimit", isFPSLimited ? limitedFPS : -1);
        PlayerPrefs.SetInt("VSyncEnabled", isVSyncEnabled ? 1 : 0);
        PlayerPrefs.SetInt("WindowedMode", isWindowedMode ? 1 : 0);

        // Save the indices for game objects and resolutions
        PlayerPrefs.SetInt("CurrentGameObjectIndex", currentGameObjectIndex);
        PlayerPrefs.SetInt("CurrentResolutionIndex", currentResolutionIndex);

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionWidth"))
        {
            resolution = new Vector2Int(PlayerPrefs.GetInt("ResolutionWidth"), PlayerPrefs.GetInt("ResolutionHeight"));
        }
        else
        {
            resolution = new Vector2Int(1920, 1080); // Default resolution if not found in PlayerPrefs
        }

        isFPSLimited = PlayerPrefs.GetInt("FPSLimit", -1) != -1;
        limitedFPS = PlayerPrefs.GetInt("FPSLimit", 60);
        limitedFPS = Mathf.Clamp(limitedFPS, 30, 240);

        isVSyncEnabled = PlayerPrefs.GetInt("VSyncEnabled", 0) == 1;
        isWindowedMode = PlayerPrefs.GetInt("WindowedMode", 1) == 1;

        // Load the indices for game objects and resolutions
        currentGameObjectIndex = PlayerPrefs.GetInt("CurrentGameObjectIndex", 0);
        currentResolutionIndex = PlayerPrefs.GetInt("CurrentResolutionIndex", 0);
    }

    public void ApplySettings()
    {
        // Apply the resolution and screen mode (Windowed, Fullscreen, Borderless) based on the settings
        if (isWindowedMode)
        {
            // If in windowed mode
            Screen.SetResolution(resolution.x, resolution.y, false); // Set windowed resolution
            Screen.fullScreen = false; // Ensure it's not fullscreen
        }
        else
        {
            // If in fullscreen mode
            Screen.SetResolution(resolution.x, resolution.y, true); // Set fullscreen resolution
            Screen.fullScreen = true; // Ensure it's fullscreen
        }

        // Apply borderless window mode if necessary
        if (!isWindowedMode && currentResolutionIndex == 1) // Assuming index 1 is for Borderless
        {
            SetBorderlessWindow();
        }

        // Apply FPS and V-Sync settings
        Application.targetFrameRate = isFPSLimited ? limitedFPS : -1;
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
    }

    // Set borderless window mode
    private void SetBorderlessWindow()
    {
        // For Borderless mode, use native resolution but ensure it's windowed (not fullscreen)
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
        Screen.fullScreen = false; // Ensure not in fullscreen mode

        // Additional platform-specific code can be added here if necessary (e.g., Windows plugins for full borderless control)
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            // Ideally, you need a native plugin or platform-specific code to make this truly borderless on Windows
            // Some Windows API or plugin could help set the window to borderless.
        }
        Debug.Log("Borderless Window Mode Enabled");
    }
}
