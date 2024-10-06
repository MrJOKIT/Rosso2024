using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class PortalManager : Singeleton<PortalManager>
{


    [Header("Random Room Setting")] 
    [SerializeField] private int roomCount;
    [Space(10)]
    public List<GameObject> battleRoomModel;
    public List<GameObject> bonusRoomModel;
    public List<GameObject> clearRoomModel;
    
    [Space(20)]
    [Header("Room One")]
    public PortalToNextRoom portalLeft;
    public GameObject leftRoom;
    public RoomType leftRoomType;
    public Vector3 leftRoomWarpPoint;
    
    [Space(10)]
    [Header("Room Two")]
    public PortalToNextRoom portalRight;
    public GameObject rightRoom;
    public RoomType rightRoomType;
    public Vector3 rightRoomWarpPoint;

    [Space(20)] 
    [Header("Clear setting")] 
    public GameObject currentRoom;

    [Space(20)] 
    [Header("Room Checker")] 
    public LayerMask roomLayer;
    [Space(7)] 
    public Transform checkForwardRoomPos;
    public bool checkForwardRoom;
    [Space(7)]
    public Transform checkLeftRoomPos;
    public bool checkLeftRoom;
    [Space(7)]
    public Transform checkMiddleRoomPos;
    public bool checkMiddleRoom;
    [Space(7)]
    public Transform checkRightRoomPos;
    public bool checkRightRoom;
    
    private Vector3 GetSpawnPoint()
    {
        Vector3 spawnPoint = new Vector3(100,0,0);

        checkForwardRoom = Physics.Raycast(checkForwardRoomPos.position,Vector3.down, Mathf.Infinity, roomLayer);
        checkLeftRoom = Physics.Raycast(checkLeftRoomPos.position,Vector3.down, Mathf.Infinity, roomLayer);
        checkMiddleRoom = Physics.Raycast(checkMiddleRoomPos.position, Vector3.down, Mathf.Infinity, roomLayer);
        checkRightRoom = Physics.Raycast(checkRightRoomPos.position, Vector3.down, Mathf.Infinity, roomLayer);

        if (!checkForwardRoomPos)
        {
            spawnPoint = new Vector3(checkForwardRoomPos.position.x, 0, checkForwardRoomPos.position.z);
        }
        else if (!checkLeftRoom)
        {
            spawnPoint = new Vector3(checkLeftRoomPos.position.x, 0, checkLeftRoomPos.position.z);
        }
        else if (!checkMiddleRoom)
        {
            spawnPoint = new Vector3(checkMiddleRoomPos.position.x, 0, checkMiddleRoomPos.position.z);
        }
        else if (!checkRightRoom)
        {
            spawnPoint = new Vector3(checkRightRoomPos.position.x, 0, checkRightRoomPos.position.z);
        }

        return spawnPoint;
    }
    
    public void PrepareRoom()
    {
        roomCount -= 1;
        if (leftRoom != null)
        {
            if (leftRoom.GetComponent<RoomManager>().playerTrans != null)
            {
                if (currentRoom == null)
                {
                    currentRoom = leftRoom;
                    leftRoom = null;
                }
                else
                {
                    currentRoom.GetComponent<RoomManager>().DestroyRoom();
                    currentRoom = leftRoom;
                    leftRoom = null;
                }
                
            }
            else
            {
                leftRoom.GetComponent<RoomManager>().DestroyRoom();
                leftRoom = null;
            }
        }

        if (rightRoom != null)
        {
            if (rightRoom.GetComponent<RoomManager>().playerTrans != null)
            {
                if (currentRoom == null)
                {
                    currentRoom = rightRoom;
                    rightRoom = null;
                }
                else
                {
                    currentRoom.GetComponent<RoomManager>().DestroyRoom();
                    checkLeftRoom = rightRoom;
                    rightRoom = null;
                }
                
            }
            else
            {
                rightRoom.GetComponent<RoomManager>().DestroyRoom();
                rightRoom = null;
            }
        }
        
        leftRoomType = RandomRoom();
        rightRoomType = RandomRoom();
        
        leftRoom = Instantiate(SpawnRoom(leftRoomType),GetSpawnPoint(),Quaternion.identity);
        rightRoom = Instantiate(SpawnRoom(rightRoomType), GetSpawnPoint(), Quaternion.identity);

        leftRoomWarpPoint = leftRoom.GetComponent<RoomManager>().startPoint.transform.position;
        rightRoomWarpPoint = rightRoom.GetComponent<RoomManager>().startPoint.transform.position;

        if (roomCount == 0)
        {
            leftRoom.GetComponent<RoomManager>().isLastRoom = true;
            rightRoom.GetComponent<RoomManager>().isLastRoom = true;
        }
    }
    
    public void SetUpNextRoom(Transform portalOnePos, Transform portalTwoPos,Transform playerTransform)
    {
        PrepareRoom();
        
        portalLeft.SetPortal(leftRoomType,leftRoomWarpPoint,leftRoom.transform,playerTransform);
        portalRight.SetPortal(rightRoomType,rightRoomWarpPoint,rightRoom.transform,playerTransform);
        
        portalLeft.transform.position = portalOnePos.position;
        portalRight.transform.position = portalTwoPos.position;
    }
    
    
    public RoomType RandomRoom()
    {
        RoomType roomType = RoomType.Combat;
        int randomNumber = Random.Range(0, 2);
        switch (randomNumber)
        {
            case 0:
                roomType = RoomType.Combat;
                break;
            case 1:
                roomType = RoomType.Bonus;
                break;
            case 2:
                roomType = RoomType.Clear;
                break;
        }

        return roomType;
    }
    
    private GameObject SpawnRoom(RoomType roomType)
    {
        GameObject room = null;
        switch (roomType)
        {
            case RoomType.Clear:
                room = clearRoomModel[Random.Range(0, clearRoomModel.Count - 1)];
                //clearRoomModel.Remove(room);
                break;
            case RoomType.Bonus:
                room = bonusRoomModel[Random.Range(0, bonusRoomModel.Count - 1)];
                //bonusRoomModel.Remove(room);
                break;
            case RoomType.Combat:
                room = battleRoomModel[Random.Range(0, battleRoomModel.Count - 1)];
                //battleRoomModel.Remove(room);
                break;
            case RoomType.Boss:
                break;
        }

        return room;
    }
}
