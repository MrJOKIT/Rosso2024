using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public Vector2Int resolution;
    public bool isFPSLimited;
    public int limitedFPS;
    public bool isVSyncEnabled;

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
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionWidth"))
        {
            resolution = new Vector2Int(PlayerPrefs.GetInt("ResolutionWidth"), PlayerPrefs.GetInt("ResolutionHeight"));
        }
        isFPSLimited = PlayerPrefs.GetInt("FPSLimit", -1) != -1;
        limitedFPS = PlayerPrefs.GetInt("FPSLimit", 60);  
        isVSyncEnabled = PlayerPrefs.GetInt("VSyncEnabled", 0) == 1;
    }

    public void ApplySettings()
    {
        Screen.SetResolution(resolution.x, resolution.y, true);
        Application.targetFrameRate = isFPSLimited ? limitedFPS : -1;
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;
    }
}