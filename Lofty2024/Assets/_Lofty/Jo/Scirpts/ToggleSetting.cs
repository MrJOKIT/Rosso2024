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
    private float deltaTime = 0.0f;

    private void Start()
    {
        // Load and apply saved settings
        SettingsManager.Instance.LoadSettings();
        previewResolution = SettingsManager.Instance.resolution;
        isVSyncEnabled = SettingsManager.Instance.isVSyncEnabled;
        isFPSLimited = SettingsManager.Instance.isFPSLimited;
        limitedFPS = SettingsManager.Instance.limitedFPS;

        // Initialize game objects, resolution, FPS settings
        InitializeGameObjects();
        InitializeResolutionGameObjects();
        InitializeFPSLimitGameObjects();

        // Ensure the FPS slider has a valid range and value
        fpsSlider.minValue = 30;
        fpsSlider.maxValue = 240;

        // Ensure the FPS slider is visible and has a valid value
        if (isFPSLimited)
        {
            fpsSlider.value = limitedFPS >= 30 && limitedFPS <= 240 ? limitedFPS : 60; // Default to 60 if invalid value
            fpsValueText.text = limitedFPS.ToString();
            fpsSlider.gameObject.SetActive(true); // Show slider if FPS is limited
        }
        else
        {
            fpsSlider.value = 60; // Default value when FPS is uncapped
            fpsValueText.text = "Uncapped";
            fpsSlider.gameObject.SetActive(false); // Hide slider if FPS is not limited
        }

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

    // Update V-Sync GameObjects visibility based on the current setting
    private void UpdateVSyncGameObjects()
    {
        if (vsyncGameObjects.Count > 0)
        {
            vsyncGameObjects[0].SetActive(!isVSyncEnabled); // If V-Sync is off, show vsyncGameObjects[0]
            if (vsyncGameObjects.Count > 1)
            {
                vsyncGameObjects[1].SetActive(isVSyncEnabled); // If V-Sync is on, show vsyncGameObjects[1]
            }
        }
    }

    public void ToggleVSync()
    {
        isVSyncEnabled = !isVSyncEnabled;

        // Update visibility of V-Sync game objects
        UpdateVSyncGameObjects();

        Debug.Log($"V-Sync state changed to: {(isVSyncEnabled ? "enabled" : "disabled")}");

        // Save the new V-Sync setting
        SettingsManager.Instance.isVSyncEnabled = isVSyncEnabled;
        SettingsManager.Instance.SaveSettings();
    }

    public void ToggleScreenMode()
    {
        if (gameObjects.Count == 0) return;

        gameObjects[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % gameObjects.Count;

        gameObjects[currentIndex].SetActive(true);
        SettingsManager.Instance.SaveSettings(); // Save settings when toggled
        Debug.Log("Toggled GameObject: " + gameObjects[currentIndex].name);
    }

    public void ToggleResolutionGameObject()
    {
        if (resolutionGameObjects.Count == 0) return;

        resolutionGameObjects[resolutionIndex].SetActive(false);
        resolutionIndex = (resolutionIndex + 1) % resolutions.Count;
        resolutionGameObjects[resolutionIndex].SetActive(true);

        previewResolution = resolutions[resolutionIndex];
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

        if (isFPSLimited)
        {
            fpsValueText.text = ((int)fpsSlider.value).ToString();
        }
        else
        {
            fpsValueText.text = "Uncapped";
        }

        SettingsManager.Instance.SaveSettings(); 
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
        SettingsManager.Instance.ApplySettings(); // Apply the settings from the SettingsManager

        Debug.Log($"Settings applied. Resolution: {previewResolution.x}x{previewResolution.y}, FPS Limit: {(isFPSLimited ? limitedFPS.ToString() : "Uncapped")}");
        fpsValueText.text = isFPSLimited ? limitedFPS.ToString() : "Uncapped";
    }

    public void OnSliderValueChanged()
    {
        if (isFPSLimited)
        {
            fpsValueText.text = ((int)fpsSlider.value).ToString();
            limitedFPS = (int)fpsSlider.value;
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
