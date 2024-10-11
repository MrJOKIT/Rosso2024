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
    public Material clearMat;

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
        this.roomTypeConnect = roomType;
        this.roomCenter = roomCenter;
        playerTrans = playerTransform;
        
        
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
            case RoomType.Clear:
                portalRenderer.material = clearMat;
                break;
        }

        warpPoint = warpPos;
        ActivePortal();
        
        isConnect = true;
    }

    public void UpdatePortal()
    {
        switch (roomTypeConnect)
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
            case RoomType.Clear:
                portalRenderer.material = clearMat;
                break;
        }
    }

    private void WarpToPoint()
    {
        playerTrans.position = new Vector3(warpPoint.x,playerTrans.position.y,warpPoint.z);
        playerTrans.GetComponent<PlayerMovementGrid>().ResetPlayerTarget();
        CameraManager.Instance.SetCameraTarget(roomCenter.position);
    }

    public void ActivePortal()
    {
        portalActive = true;
        portalObject.SetActive(true);
    }

    public void DeActivePortal()
    {
        portalActive = false;
        portalObject.SetActive(false);
    }
}
