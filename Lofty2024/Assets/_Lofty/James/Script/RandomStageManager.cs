using System;
using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
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
    [MinMaxSlider(1, 10)] public Vector2Int spawnCount;
    private int roomSpawnCount;
    public int RoomSpawnCount => roomSpawnCount;
    public List<GameObject> roomModel;

    public override void Awake()
    {
        roomSpawnCount = Random.Range(spawnCount.x, spawnCount.y);
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

    public void SpawnRoom(Transform spawnPoint)
    {
        GameObject room = roomModel[Random.Range(0, roomModel.Count - 1)];
        GameObject battleRoom = Instantiate(room,new Vector3(spawnPoint.position.x,0,spawnPoint.position.z),Quaternion.identity);
        roomInStage.Add(battleRoom.GetComponent<RoomManager>());
        roomModel.Remove(room);
        roomSpawnCount -= 1;
    }
}
