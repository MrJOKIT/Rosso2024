using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public enum RoomType
{
    Combat,
    Bonus,
    Boss,
}
public class RoomManager : MonoBehaviour
{
    [Tab("Room Setting")] 
    public RoomType roomType;
    [Space(10)]
    public GameObject portalPrefab;
    public LayerMask roomLayer;
    public Transform centerTransform;
    public Transform playerTrans;

    [Header("Forward Portal")] 
    public Transform spawnPointForward;
    public Vector3 forwardConnectPoint;
    public bool forwardPortal;
    RaycastHit hitForward;
    [Space(5)] 
    [Header("Backward Portal")] 
    public Transform spawnPointBackward;
    public Vector3 backwardConnectPoint;
    public bool backwardPortal;
    RaycastHit hitBackward;
    [Space(5)] 
    [Header("Left Portal")] 
    public Transform spawnPointLeft;
    public Vector3 leftConnectPoint;
    public bool leftPortal;
    RaycastHit hitLeft;
    [Space(5)] 
    [Header("Right Portal")] 
    public Transform spawnPointRight;
    public Vector3 rightConnectPoint;
    public bool rightPortal;
    RaycastHit hitRight;
    
    [Tab("Enemy Setting")]
    public List<Enemy> enemyInRoom;
    public bool roomClear;

    private void Start()
    {
        RandomStageManager.Instance.AddRoom(this);
    }

    private void StartRoom()
    {
        TurnManager.Instance.TurnStart();
        foreach (Enemy enemy in enemyInRoom)
        {
            enemy.ActiveUnit();
        }
    }

    private void Update()
    {
        if (roomClear)
        {
            return;
        }
        RoomProgressCheck();
    }

    private void FixedUpdate()
    {
        PortalCheck();
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
            RoomClear();
        }
    }

    private void RoomClear()
    {
        roomClear = true;
        TurnManager.Instance.currentRoomClear = true;
        SpawnPortal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (roomClear)
            {
                return;
            }
            playerTrans = other.GetComponent<Transform>();
            StartRoom();
        }
    }

    private void PortalCheck()
    {
        forwardPortal = Physics.Raycast(centerTransform.position, Vector3.forward,out hitForward, Mathf.Infinity, roomLayer);
        
        backwardPortal = Physics.Raycast(centerTransform.position, Vector3.back,out hitBackward,Mathf.Infinity, roomLayer);
        
        leftPortal = Physics.Raycast(centerTransform.position, Vector3.left,out hitLeft,Mathf.Infinity, roomLayer);
        
        rightPortal = Physics.Raycast(centerTransform.position, Vector3.right,out hitRight,Mathf.Infinity, roomLayer);
    }

    private void SpawnPortal()
    {
        if (forwardPortal)
        {
            try
            {
                RoomManager roomManager = hitForward.collider.GetComponent<RoomManager>();
                GameObject portal = Instantiate(portalPrefab, spawnPointForward);
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointBackward.position,roomManager.transform.position,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Forward portal not found {a}");
            }
        }
        
        if (backwardPortal)
        {
            try
            {
                RoomManager roomManager = hitBackward.collider.GetComponent<RoomManager>();
                GameObject portal = Instantiate(portalPrefab, spawnPointBackward);
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointForward.position,roomManager.transform.position,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Backward portal not found {a}");
            }
        }
        
        if (leftPortal)
        {
            try
            {
                RoomManager roomManager = hitLeft.collider.GetComponent<RoomManager>();
                GameObject portal = Instantiate(portalPrefab, spawnPointLeft);
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointRight.position,roomManager.transform.position,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Left portal not found {a}");
            }
        }
        
        if (rightPortal)
        {
            try
            {
                RoomManager roomManager = hitRight.collider.GetComponent<RoomManager>();
                GameObject portal = Instantiate(portalPrefab, spawnPointRight);
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointLeft.position,roomManager.transform.position,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Right portal not found {a}");
            }
        }
    }
}
