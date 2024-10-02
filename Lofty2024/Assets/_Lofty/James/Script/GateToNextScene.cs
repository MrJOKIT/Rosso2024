using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateToNextScene : InterfacePopUp<GateToNextScene>
{
    [SerializeField] private string sceneName;

    private void Update()
    {
        if (!onPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            LoadNextScene();
        }
    }

    public void SetNextScene(string sceneName)
    {
        this.sceneName = sceneName;
    }
    private void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
