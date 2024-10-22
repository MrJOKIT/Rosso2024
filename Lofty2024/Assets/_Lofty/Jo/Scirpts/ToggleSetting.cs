using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToggleSetting : MonoBehaviour
{
    public List<GameObject> gameObjects; 
    public List<GameObject> resolutionGameObjects; 
    public List<GameObject> fpsLimitGameObjects; 
    public TextMeshProUGUI fpsText; 
    public TextMeshProUGUI fpsValueText; 
    public Slider fpsSlider;

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
    private bool isFPSLimited = false; 
    private int limitedFPS = 60; 
    private float deltaTime = 0.0f;

    private void Start()
    {
        InitializeGameObjects();
        InitializeResolutionGameObjects();
        InitializeFPSLimitGameObjects();
        previewResolution = resolutions[resolutionIndex];

        // ตั้งค่าเริ่มต้นสำหรับ Slider และ FPS
        fpsSlider.minValue = 30; 
        fpsSlider.maxValue = 240; 
        fpsSlider.value = limitedFPS; 
        fpsValueText.text = limitedFPS.ToString(); 

       
        isFPSLimited = false; 
        fpsSlider.gameObject.SetActive(false); // ปิด Slider
        ApplySettings();
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

    public void ToggleFPS()
    {
        isFPSLimited = !isFPSLimited; 
        UpdateFPSLimitGameObject();

      
        fpsSlider.gameObject.SetActive(isFPSLimited); 
        fpsValueText.text = ((int)fpsSlider.value).ToString();

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
        Screen.SetResolution(previewResolution.x, previewResolution.y, Screen.fullScreen);
        ToggleFullScreen(); 

        
        if (isFPSLimited)
        {
            limitedFPS = (int)fpsSlider.value; 
            Application.targetFrameRate = limitedFPS; 
        }
        else
        {
            Application.targetFrameRate = -1; 
        }

        Debug.Log($"Settings applied. Full Screen: {Screen.fullScreen}, Current GameObject: {gameObjects[currentIndex].name}, Resolution: {previewResolution.x}x{previewResolution.y}, FPS Limit: {(isFPSLimited ? limitedFPS.ToString() : "Uncapped")}");

        fpsValueText.text = ((int)fpsSlider.value).ToString();
    }

    private void ToggleFullScreen()
    {
        if (currentIndex == 0)
        {
            Screen.fullScreen = true;  
        }
        else if (currentIndex == 1)
        {
            Screen.fullScreen = false; 
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

    public void OnSliderValueChanged()
    {
        fpsValueText.text = ((int)fpsSlider.value).ToString();
    }

    public void ExitGame()
    {
        Application.Quit(); 

    }
}
