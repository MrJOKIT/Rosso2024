using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class PortalManager : Singeleton<PortalManager>
{


    [Header("Random Room Setting")] 
    [SerializeField] private int gameLoopCount;
    [Space(10)]
    [SerializeField] private int roomCount;
    [Space(10)]
    public List<GameObject> battleRoomModel;
    public List<GameObject> bonusRoomModel;
    public List<GameObject> clearRoomModel;
    public Vector3 spawnPoint;
    
    [Space(20)]
    [Header("Room One")]
    public PortalToNextRoom portalLeft;
    public RoomManager leftRoom;
    public RoomType leftRoomType;
    public Vector3 leftRoomWarpPoint;
    
    [Space(10)]
    [Header("Room Two")]
    public PortalToNextRoom portalRight;
    public RoomManager rightRoom;
    public RoomType rightRoomType;
    public Vector3 rightRoomWarpPoint;

    [Space(20)] 
    [Header("Clear setting")] 
    public RoomManager currentRoom;
    public Transform playerTransform;
    public Transform portalLeftPos;
    public Transform portalRightPos;
    private bool leftOrRight;
    public bool isBossRoom;

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
    

    private void Update()
    {
        checkForwardRoom = Physics.Raycast(checkForwardRoomPos.position,Vector3.down, Mathf.Infinity, roomLayer);
        checkLeftRoom = Physics.Raycast(checkLeftRoomPos.position,Vector3.down, Mathf.Infinity, roomLayer);
        checkMiddleRoom = Physics.Raycast(checkMiddleRoomPos.position, Vector3.down, Mathf.Infinity, roomLayer);
        checkRightRoom = Physics.Raycast(checkRightRoomPos.position, Vector3.down, Mathf.Infinity, roomLayer);
    }

    private Vector3 GetSpawnPoint()
    {
        

        if (!checkForwardRoom)
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
    
    
    public void SetUpNextRoom(Transform portalLeftPos,Transform portalRightPos,Transform playerTransform)
    {
        if (roomCount <= 0)
        {
            portalLeft.DeActivePortal();;
            portalRight.DeActivePortal();
        }
        
        
        roomCount -= 1;

        this.playerTransform = playerTransform;
        this.portalLeftPos = portalLeftPos;
        this.portalRightPos = portalRightPos;


        if (isBossRoom)
        {
            Invoke("SpawnRoom",0.2f);
        }
        else
        {
            Invoke("SpawnRoom",0.2f);
            Invoke("SpawnRoom",0.3f);
        }
        
    }

    private void SpawnRoom()
    {
        if (isBossRoom)
        {
            //show one portal
        }
        else
        {
            if (leftOrRight)
            {
                leftRoomType = RandomRoom();
                GameObject leftRoomObject = Instantiate(GetRoom(leftRoomType),GetSpawnPoint(),Quaternion.identity);
                leftRoom = leftRoomObject.GetComponent<RoomManager>();
                leftRoomWarpPoint = leftRoom.GetComponent<RoomManager>().startPoint.transform.position;
                portalLeft.SetPortal(leftRoomType,leftRoomWarpPoint,leftRoom.transform,playerTransform);
                portalLeft.transform.position = portalLeftPos.position;
                portalLeft.GetComponent<PortalToNextRoom>().ActivePortal();
                if (roomCount == 0)
                {
                    leftRoom.isLastRoom = true;
                }

                leftOrRight = false;
            }
            else
            {
                rightRoomType = RandomRoom();
                GameObject rightRoomObject = Instantiate(GetRoom(rightRoomType), GetSpawnPoint(), Quaternion.identity);
                rightRoom = rightRoomObject.GetComponent<RoomManager>();
                rightRoomWarpPoint = rightRoom.GetComponent<RoomManager>().startPoint.transform.position;
                portalRight.SetPortal(rightRoomType,rightRoomWarpPoint,rightRoom.transform,playerTransform);
                portalRight.transform.position = portalRightPos.position;
                portalRight.GetComponent<PortalToNextRoom>().ActivePortal();
                if (roomCount == 0)
                {
                    rightRoom.isLastRoom = true;
                }

                leftOrRight = true;
            }
        }
        
    }
    

    public void StartClearRoom()
    {
        Invoke("ClearRoom",0.1f);
    }
    private void ClearRoom()
    {
        if (leftRoom != null)
        {
            if (leftRoom.playerTrans != null)
            {
                if (currentRoom == null)
                {
                    currentRoom = leftRoom;
                    leftRoom = null;
                }
                else
                {
                    currentRoom.DestroyRoom();
                    currentRoom = leftRoom;
                    leftRoom = null;
                }
                
            }
            else
            {
                leftRoom.DestroyRoom();
                leftRoom = null;
            }
        }
         
        if (rightRoom != null)
        {
            if (rightRoom.playerTrans != null)
            {
                if (currentRoom == null)
                {
                    currentRoom = rightRoom;
                    rightRoom = null;
                }
                else
                {
                    currentRoom.DestroyRoom();
                    currentRoom = rightRoom;
                    rightRoom = null;
                }
                
            }
            else
            {
                rightRoom.DestroyRoom();
                rightRoom = null;
            }
        }
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

        if (roomCount == 0)
        {
            if (isBossRoom)
            {
                //fix type is boss
            }
            else
            {
                roomType = RoomType.Combat;
            }
        }

        return roomType;
    }
     
    private GameObject GetRoom(RoomType roomType)
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

        if (roomCount == 0)
        {
            if (isBossRoom)
            {
                //random boss room
            }
            else
            {
                room = battleRoomModel[Random.Range(0, battleRoomModel.Count - 1)];
            }
        }

        return room;
    }
}
