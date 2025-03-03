using System;
using System.Collections;
using System.Collections.Generic;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneLoading : MonoBehaviour
{
    public GameObject loadingScene;
    public GameObject tutorialConfirmCanvas;
    public float loadTime;
    [FormerlySerializedAs("loadSucces")] public bool loadSuccess;
    public bool startLoad;
    public TransitionProfile loadSuccesProfile;
    
    private void Update()
    {
        if (loadSuccess || startLoad)
        {
            return;
        }

        loadTime -= Time.deltaTime;
        if (loadTime <= 0)
        {
            startLoad = true;
            loadingScene.SetActive(false);
            SoundManager.instace.Play(SoundManager.SoundName.BonusBGM);
            TransitionAnimator animator = TransitionAnimator.Start(loadSuccesProfile);
            animator.onTransitionEnd.AddListener(LoadSuccess);
        }
        else
        {
            loadingScene.SetActive(true);
        }
    }

    private void LoadSuccess()
    {
        print("Load Scene Success");
        loadTime = 0;
        loadSuccess = true;
        // GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().StartRoom();
        
        TurnManager.Instance.CurrentRoomClear();
        GetComponent<GameManager>().StartTimer();
        GetComponent<GameManager>().SetCursorVisible(true);
        bool tutorialEnable = ES3.Load("TutorialPopUp", true);
        if (tutorialEnable)
        {
            tutorialConfirmCanvas.SetActive(true);
            TutorialManager.Instance.tutorialState = TutorialState.OnProgress;
        }
    }
    
}
