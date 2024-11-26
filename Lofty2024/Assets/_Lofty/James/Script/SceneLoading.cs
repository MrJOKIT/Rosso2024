using System;
using System.Collections;
using System.Collections.Generic;
using TransitionsPlus;
using UnityEngine;

public class SceneLoading : MonoBehaviour
{
    public GameObject loadingScene;
    public float loadTime;
    public bool loadSucces;
    public bool startLoad;
    public TransitionProfile loadSuccesProfile;
    
    private void Update()
    {
        if (loadSucces || startLoad)
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
            animator.onTransitionEnd.AddListener(LoadSucces);
        }
        else
        {
            loadingScene.SetActive(true);
        }
    }

    private void LoadSucces()
    {
        loadTime = 0;
        loadSucces = true;
        GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().StartRoom();
        GetComponent<GameManager>().StartTimer();
        
    }
    
}
