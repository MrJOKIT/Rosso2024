using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<Enemy> enemyInRoom;
    public bool roomClear;

    private void Start()
    {
        RandomStageManager.Instance.AddRoom(this);
        foreach (Enemy enemy in enemyInRoom)
        {
            enemy.ActiveUnit();
        }
    }

    private void StartRoom()
    {
        TurnManager.Instance.currentRoomClear = false;
    }

    private void Update()
    {
        if (roomClear)
        {
            return;
        }
        RoomProgressCheck();
    }

    private void RoomProgressCheck()
    {
        if (roomClear)
        {
            return;
        }
        bool roomComplete = true;
        foreach (Enemy enemy in enemyInRoom)
        {
            if (enemy.isDead == false)
            {
                roomComplete = false;
                break;
            }
        }

        if (roomComplete)
        {
            roomClear = true;
            TurnManager.Instance.currentRoomClear = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartRoom();
            foreach (Enemy enemy in enemyInRoom)
            {
                enemy.ActiveUnit();
            }
        }
    }
}
