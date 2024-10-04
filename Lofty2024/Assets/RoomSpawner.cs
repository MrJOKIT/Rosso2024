using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSpawner : MonoBehaviour
{
    private void Start()
    {
        if (RandomStageManager.Instance.RoomSpawnCount <= 0)
        {
            return;
        }
        Invoke("SpawnRoom",Random.Range(0.1f,0.5f));
    }

    private void SpawnRoom()
    {
        RandomStageManager.Instance.SpawnRoom(transform);
    }
}
