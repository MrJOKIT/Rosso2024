using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSpawner : MonoBehaviour
{
    public RoomType spawnType;
    private void Start()
    {
        if (spawnType == RoomType.Clear)
        {
            Invoke("SpawnRoom",0.5f); ;
        }
        else
        {
            Invoke("SpawnRoom",0.1f); ;
        }
        
    }

    private void SpawnRoom()
    {
        if (RandomStageManager.Instance.RoomSpawnCount <= 0)
        {
            return;
        } 
        RandomStageManager.Instance.SpawnRoom(transform,spawnType);
    }
    
}
