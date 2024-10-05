using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using GD.MinMaxSlider;
using UnityEditor;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;


public class RandomStageManager : Singeleton<RandomStageManager>
{  
    [Tab("Game Room Setting")]
    public List<RoomManager> roomInStage;
    public Transform currentRoomPos;
    public bool stageClear;

    [Tab("Random Room Setting")] 
    [Range(1, 10)] public int spawnCount;
    [Range(0,10)]public int battleRoomCount;
    [Range(0,10)]public int bonusRoomCount;
    private int roomSpawnCount;
    public int RoomSpawnCount => roomSpawnCount;
    public List<GameObject> battleRoomModel;
    public List<GameObject> bonusRoomModel;
    public List<GameObject> clearRoomModel;
    public override void Awake()
    {
        roomSpawnCount = spawnCount;
        Debug.Log(battleRoomModel.Count - 1);
        base.Awake();
    }
    
    private void LateUpdate()
    {
        if (stageClear || roomInStage.Count == 0)
        {
            return;
        }
        StageProgressCheck();
    }

    public void AddRoom(RoomManager roomManager) 
    {
        roomInStage.Add(roomManager);
    }

    private void StageProgressCheck()
    {
        bool stageComplete = true;
        foreach (RoomManager room in roomInStage)
        {
            if (room.roomClear == false)
            {
                stageComplete = false;
                break;
            }
        }

        if (stageComplete)
        {
            stageClear = true;
            GetComponent<GameManager>().StageClear();
        }
    }

    public void UpdateCurrentRoom(Transform newCurrentRoom)
    {
        currentRoomPos = newCurrentRoom;
    }
    
    public void SpawnRoom(Transform spawnPoint,RoomType roomType)
    {
        roomSpawnCount -= 1;
        GameObject room;
        switch (roomType)
        {
            case RoomType.Clear:
                room = clearRoomModel[Random.Range(0, clearRoomModel.Count - 1)];
                GameObject clearRoom = Instantiate(room,new Vector3(spawnPoint.position.x,0,spawnPoint.position.z),Quaternion.identity);
                if (roomSpawnCount == 0)
                {
                    clearRoom.GetComponent<RoomManager>().isLastRoom = true;
                }
                roomInStage.Add(clearRoom.GetComponent<RoomManager>());
                //bonusRoomModel.Remove(room);
                break;
            case RoomType.Bonus:
                room = bonusRoomModel[Random.Range(0, bonusRoomModel.Count - 1)];
                GameObject bonusRoom = Instantiate(room,new Vector3(spawnPoint.position.x,0,spawnPoint.position.z),Quaternion.identity);
                if (roomSpawnCount == 0)
                {
                    bonusRoom.GetComponent<RoomManager>().isLastRoom = true;
                }
                roomInStage.Add(bonusRoom.GetComponent<RoomManager>());
                //bonusRoomModel.Remove(room);
                break;
            case RoomType.Combat:
                room = battleRoomModel[Random.Range(0, battleRoomModel.Count - 1)];
                GameObject battleRoom = Instantiate(room,new Vector3(spawnPoint.position.x,0,spawnPoint.position.z),Quaternion.identity);
                if (roomSpawnCount == 0)
                {
                    battleRoom.GetComponent<RoomManager>().isLastRoom = true;
                }
                roomInStage.Add(battleRoom.GetComponent<RoomManager>());
                //battleRoomModel.Remove(room);
                break;
            case RoomType.Boss:
                break;
        }
        
        
        
    }
    
}

