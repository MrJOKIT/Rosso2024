using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStageManager : Singeleton<RandomStageManager>
{
    public List<RoomManager> roomInStage;
    public Vector3 currentRoomPos;
    public bool stageClear;

    private void LateUpdate()
    {
        if (stageClear)
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

    public void UpdateCurrentRoom(Vector3 newCurrentRoom)
    {
        currentRoomPos = newCurrentRoom;
    }
}
