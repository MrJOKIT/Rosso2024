using System;
using System.Collections;
using System.Collections.Generic;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateToNextScene : InterfacePopUp<GateToNextScene>
{
    [SerializeField] private string sceneName;
    public bool onWarp;
    public bool formatWhenWarp;

    private void Update()
    {
        if (!onPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && !onWarp)
        {
            onWarp = true;
            if (formatWhenWarp)
            {
                FormatAllData();
            }
            TransitionAnimator animator = TransitionAnimator.Start(TransitionType.Smear,2f,sceneNameToLoad:sceneName);
            animator.onTransitionEnd.AddListener(LoadNextScene);
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

    private void FormatAllData()
    {
        ES3.DeleteKey("FirstStage");
        ES3.DeleteKey("SecondStage");
        ES3.DeleteKey("ClearCount");
        
        ES3.DeleteKey("CardStock");
        
        ES3.DeleteKey("PlayerDefaultHealth");
        ES3.DeleteKey("PlayerDefaultHealthTemp");
        ES3.DeleteKey("PlayerCurrentHealth");
        ES3.DeleteKey("PlayerCurrentHealthTemp");
        
        ES3.DeleteKey("PlayerDefaultMovePoint");
        ES3.DeleteKey("PlayerDefaultDamage");
        ES3.DeleteKey("PlayerDefaultKnockBackRange");
        
        ES3.DeleteKey("ArtifactHave");
        
        ES3.DeleteKey("FirstClassUnlock");
        ES3.DeleteKey("SecondClassUnlock");
        ES3.DeleteKey("SwordPassiveOne");
        ES3.DeleteKey("SwordPassiveTwo");
        ES3.DeleteKey("BladePassiveOne");
        ES3.DeleteKey("BladePassiveTwo");
        ES3.DeleteKey("ShootPassiveOne");
        ES3.DeleteKey("ShootPassiveTwo");
        
        ES3.DeleteKey("EricCoin");
        ES3.DeleteKey("FlameSoul");
        
        ES3.DeleteKey("ArtifactHave");
        ES3.DeleteKey("TimeCount");
    }
}
