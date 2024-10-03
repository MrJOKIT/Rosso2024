using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalToNextRoom : InterfacePopUp<PortalToNextRoom>
{
    [Header("Portal Setting")]
    public RoomType roomTypeConnect;
    public Vector3 roomCenter;
    public Vector3 warpPoint;
    public Transform playerTrans;

    [Space(10)] 
    [Header("Material")] 
    public MeshRenderer portalRenderer;
    [Space(5)]
    public Material combatMaterial;
    public Material bonusMaterial;
    public Material bossMaterial;

    private void Update()
    {
        if (!onPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            WarpToPoint();
        }
    }

    public void SetPortal(RoomType roomType,Vector3 warpPos,Vector3 roomCenter,Transform playerTransform)
    {
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
        }

        warpPoint = warpPos;
    }

    private void WarpToPoint()
    {
        playerTrans.position = new Vector3(warpPoint.x,playerTrans.position.y,warpPoint.z);
        playerTrans.GetComponent<PlayerMovementGrid>().ResetPlayerTarget();
        CameraManager.Instance.SetCameraTarget(roomCenter);
        GameManager.Instance.GetComponent<RandomStageManager>().UpdateCurrentRoom(roomCenter);
    }
}
