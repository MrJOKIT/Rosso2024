using System;
using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public enum RoomType
{
    Combat,
    Bonus,
    Boss,
}
public class RoomManager : MonoBehaviour
{
    [Tab("Room Setting")] 
    public bool roomActive;
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

    [Tab("Enemy Generator")] 
    public bool enemySpawnComplete;
    [MinMaxSlider(1,10)] public Vector2Int spawnEnemyCount;
    private int spawnedEnemyCount;
    private int spawnEnemyMax;
    
    public Transform enemyParent;
    public List<GameObject> enemyPrefab;
    public List<Enemy> enemyInRoom;
    public bool roomClear;

    [Tab("Room Generator")] 
    public bool obstacleSpawnComplete;
    [MinMaxSlider(5,20)] public Vector2Int spawnObstacleCount;
    private int spawnedObstacleCount;
    private int spawnObstacleMax;
    
    public Transform obstacleParent;
    public List<GameObject> obstaclePrefab;
    public List<GridMover> currentGrid;

    private void Start()
    {
        RandomStageManager.Instance.AddRoom(this);
        foreach (GridMover grid in currentGrid)
        {
            GridSpawnManager.Instance.AddGridList(grid);
        }
    }

    private void StartRoom()
    {
        TurnManager.Instance.TurnStart();

        spawnObstacleMax = Random.Range(spawnObstacleCount.x, spawnObstacleCount.y);
        spawnEnemyMax = Random.Range(spawnEnemyCount.x,spawnEnemyCount.y);
        for (int i = 0; i < spawnObstacleMax; i++)
        {
            StartCoroutine(SpawnObstacle());
        }
        for (int i = 0; i < spawnEnemyMax; i++)
        {
            StartCoroutine(SpawnEnemy());
        }
        
        
    }

    IEnumerator SpawnObstacle()
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(obstaclePrefab[Random.Range(0, obstaclePrefab.Count - 1)], CheckSpawnPoint(), Quaternion.identity, obstacleParent);
        spawnedObstacleCount += 1;
        if (spawnedObstacleCount == spawnObstacleMax)
        {
            obstacleSpawnComplete = true;
        }
    }
    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject enemy = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Count - 1)], CheckSpawnPoint(), Quaternion.identity,enemyParent);
        enemyInRoom.Add(enemy.GetComponent<Enemy>());
        enemy.GetComponent<Enemy>().targetTransform = playerTrans;
        enemy.GetComponent<Enemy>().ActiveUnit();
        spawnedEnemyCount += 1;
        if (spawnedEnemyCount == spawnEnemyMax)
        {
            enemySpawnComplete = true;
        }
    }

    private void Update()
    {
        if (!roomActive)
        {
            if (enemySpawnComplete && obstacleSpawnComplete) 
            {
                roomActive = true;
            }
            return;
        }

        if (roomClear)
        {
            return;
        }
        RoomProgressCheck();
    }

    public Vector3 CheckSpawnPoint()
    {
        Vector3 spawnPoint ;
        do
        {
            int randomNumber = Random.Range(0, currentGrid.Count - 1);
            if (currentGrid[randomNumber].gridState == GridState.Empty)
            {
                spawnPoint = new Vector3(currentGrid[randomNumber].transform.position.x,0.5f,currentGrid[randomNumber].transform.position.z);
                break;
            }
            
        } while (true);

        return spawnPoint;
    }
    private void FixedUpdate()
    {
        PortalCheck();
    }

    private void RoomProgressCheck()
    {
        if (roomClear || !roomActive)
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
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointBackward.position,roomManager.transform,playerTrans);
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
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointForward.position,roomManager.transform,playerTrans);
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
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointRight.position,roomManager.transform,playerTrans);
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
                portal.GetComponent<PortalToNextRoom>().SetPortal(roomManager.roomType,roomManager.spawnPointLeft.position,roomManager.transform,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Right portal not found {a}");
            }
        }
    }
}
