using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalToNextRoom : InterfacePopUp<PortalToNextRoom>
{
    [Header("Portal Setting")]
    public RoomType roomTypeConnect;
    public Transform roomCenter;
    public Vector3 warpPoint;
    public Transform playerTrans;
    public GameObject portalObject;
    public bool portalActive;
    public bool isConnect;

    [Space(10)] 
    [Header("Material")] 
    public MeshRenderer portalRenderer;
    [Space(5)]
    public Material combatMaterial;
    public Material bonusMaterial;
    public Material bossMaterial;

    private void Update()
    {
        if (!onPlayer || !portalActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            WarpToPoint();
        }
    }

    public void SetPortal(RoomType roomType,Vector3 warpPos,Transform roomCenter,Transform playerTransform)
    {
        this.roomCenter = roomCenter;
        playerTrans = playerTransform;
        portalObject.SetActive(true);
        
        switch (roomType)
        {
            case RoomType.Combat:
                portalRenderer.material = combatMaterial;
                break;
            case RoomType.Bonus:
                portalRenderer.material = bonusMaterial;
                break;
            case RoomType.Boss:
                portalRenderer.material = bossMaterial;
                break;
        }

        warpPoint = warpPos;

        portalActive = true;
        isConnect = true;
    }

    private void WarpToPoint()
    {
        playerTrans.position = new Vector3(warpPoint.x,playerTrans.position.y,warpPoint.z);
        playerTrans.GetComponent<PlayerMovementGrid>().ResetPlayerTarget();
        CameraManager.Instance.SetCameraTarget(roomCenter.position);
        GameManager.Instance.GetComponent<RandomStageManager>().UpdateCurrentRoom(roomCenter);
    }
}
