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
    public Transform currentRoomPos;
    public bool stageClear;

    [Tab("Random Room Setting")] 
    public List<GameObject> battleRoomModel;
    public List<GameObject> bonusRoomModel;
    public List<GameObject> clearRoomModel;
    
    public void UpdateCurrentRoom(Transform newCurrentRoom)
    {
        currentRoomPos = newCurrentRoom;
    }
    
    public GameObject SpawnRoom(RoomType roomType)
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

