using System;
using System.Collections;
using System.Collections.Generic;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.Events;

public class PortalToNextRoom : InterfacePopUp<PortalToNextRoom>
{
    [Header("Portal Setting")]
    public RoomType roomTypeConnect;
    public Transform roomCenter;
    public Vector3 warpPoint;
    public Transform playerTrans;
    public bool portalActive;
    public bool isConnect;
    public bool pressActive;
    [Space(10)] 
    [Header("Portal Object")] 
    public GameObject combatPortal;
    public GameObject bonusPortal;
    public GameObject bossPortal;
    public GameObject clearPortal;
    private void Update()
    {
        if (!onPlayer || !portalActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && pressActive == false)
        {
            TransitionAnimator animator = TransitionAnimator.Start(TransitionType.Fade,0.75f,autoDestroy:true);
            animator.onTransitionEnd.AddListener(WarpToPoint);
            pressActive = true;
        }
    }

    public void SetPortal(RoomType roomType,Vector3 warpPos,Transform roomCenter,Transform playerTransform)
    {
        this.roomTypeConnect = roomType;
        this.roomCenter = roomCenter;
        playerTrans = playerTransform;
        
        
        switch (roomType)
        {
            case RoomType.Combat:
                SetDeActivePortal();
                combatPortal.SetActive(true);
                break;
            case RoomType.Bonus:
                SetDeActivePortal();
                bonusPortal.SetActive(true);
                break;
            case RoomType.Boss:
                SetDeActivePortal();
                bossPortal.SetActive(true);
                break;
            case RoomType.Clear:
                SetDeActivePortal();
                clearPortal.SetActive(true);
                break;
        }

        warpPoint = warpPos;
        ActivePortal();
        
        isConnect = true;
    }

    private void SetDeActivePortal()
    {
        combatPortal.SetActive(false);
        bonusPortal.SetActive(false);
        bossPortal.SetActive(false);
        clearPortal.SetActive(false);
    }

    public void UpdatePortal()
    {
        switch (roomTypeConnect)
        {
            case RoomType.Combat:
                SetDeActivePortal();
                combatPortal.SetActive(true);
                break;
            case RoomType.Bonus:
                SetDeActivePortal();
                bonusPortal.SetActive(true);
                break;
            case RoomType.Boss:
                SetDeActivePortal();
                bossPortal.SetActive(true);
                break;
            case RoomType.Clear:
                SetDeActivePortal();
                clearPortal.SetActive(true);
                break;
        }
    }
    private void WarpToPoint()
    {
        playerTrans.position = new Vector3(warpPoint.x,playerTrans.position.y,warpPoint.z);
        playerTrans.GetComponent<PlayerMovementGrid>().ResetPlayerTarget();
        CameraManager.Instance.SetCameraTarget(roomCenter.position);
        GameManager.Instance.GetComponent<PortalManager>().ShowStageNumber();
        PortalManager.Instance.progressList[PortalManager.Instance.secondStageNumber - 2].SetBarType(roomTypeConnect);
        PortalManager.Instance.progressState = ProgressState.OnProgress;
        //TransitionAnimator animatorTwo = TransitionAnimator.Start(TransitionType.Fade,duration: 2f,invert:true,autoDestroy:true,playDelay:2f);
        //animatorTwo.onTransitionEnd.AddListener(GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().StartRoom);
    }
    
    public void ActivePortal()
    {
        pressActive = false;
        portalActive = true;
    }

    public void DeActivePortal()
    {
        portalActive = false;
    }
}
