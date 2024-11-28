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
    public bool isWindowedMode; 

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

        LoadSettings(); 
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionWidth", resolution.x);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.y);
        PlayerPrefs.SetInt("FPSLimit", isFPSLimited ? limitedFPS : -1);
        PlayerPrefs.SetInt("VSyncEnabled", isVSyncEnabled ? 1 : 0);
        PlayerPrefs.SetInt("WindowedMode", isWindowedMode ? 1 : 0);

       
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
            resolution = new Vector2Int(1920, 1080); 
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
       
        if (isWindowedMode)
        {
            
            Screen.SetResolution(resolution.x, resolution.y, false); 
            Screen.fullScreen = false; 
        }
        else
        {
          
            Screen.SetResolution(resolution.x, resolution.y, true);
            Screen.fullScreen = true; 
        }

       
        if (!isWindowedMode && currentResolutionIndex == 1) 
        {
            SetBorderlessWindow();
        }

        
        Application.targetFrameRate = isFPSLimited ? limitedFPS : -1;
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
    }

   
    private void SetBorderlessWindow()
    {
       
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
        Screen.fullScreen = false; 

        
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            
        }
        Debug.Log("Borderless Window Mode Enabled");
    }
}
