using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoading : MonoBehaviour
{
    public GameObject loadingScene;
    public float loadTime;
    public bool loadSucces;
    
    private void Update()
    {
        if (loadSucces)
        {
            return;
        }

        loadTime -= Time.deltaTime;
        if (loadTime <= 0)
        {
            loadingScene.SetActive(false);
            loadTime = 0;
            loadSucces = true;
        }
        else
        {
            loadingScene.SetActive(true);
        }
    }
}
