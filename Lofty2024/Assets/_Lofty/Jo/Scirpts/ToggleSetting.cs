using System.Collections.Generic;
using UnityEngine;

public class ToggleSetting : MonoBehaviour
{
    public List<GameObject> gameObjects; 
    private int currentIndex = 0;
    private bool settingsApplied = false;

    private void Start()
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

    private void Update()
    {
       
        if (!settingsApplied && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleGameObject();
        }
       
        else if (settingsApplied && Input.GetKeyDown(KeyCode.Return))
        {
            ApplySettings();
        }
    }

    public void ToggleGameObject()
    {
       
        gameObjects[currentIndex].SetActive(false);

       
        currentIndex = (currentIndex + 1) % gameObjects.Count;

        
        gameObjects[currentIndex].SetActive(true);

       
        settingsApplied = true;
    }

    public void ApplySettings()
    {
        ToggleFullScreen();
        
        Debug.Log("Settings applied. Full Screen: " + Screen.fullScreen + ", Current GameObject: " + gameObjects[currentIndex].name);
        
        settingsApplied = false;
    }

    private void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
