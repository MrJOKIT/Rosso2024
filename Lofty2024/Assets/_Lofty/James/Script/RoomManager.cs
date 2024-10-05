using System;
using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;
using Random = UnityEngine.Random;

public enum RoomType
{
    Combat,
    Bonus,
    Boss,
    Clear,
}
public class RoomManager : MonoBehaviour
{
    [Tab("Room Setting")] 
    [Space(5)] 
    public bool isLastRoom;
    public bool roomActive;
    public RoomType roomType;
    [Space(10)]
    public LayerMask roomLayer;
    public Transform playerTrans;

    [Foldout("Room Direction")] 
    public Transform startPoint;
    public Transform leftPoint;
    public Transform rightPoint;
    
    /*[Header("Forward Portal")] 
    public PortalToNextRoom portalForward;
    public Vector3 forwardConnectPoint;
    public bool forwardPortal;
    RaycastHit hitForward;
    [Space(5)] 
    [Header("Backward Portal")] 
    public PortalToNextRoom portalBackward;
    public Vector3 backwardConnectPoint;
    public bool backwardPortal;
    RaycastHit hitBackward;*/
    [Space(5)] 
    [Header("Left Portal")] 
    public PortalToNextRoom portalLeft;
    public Vector3 leftConnectPoint;
    public bool leftPortal;
    RaycastHit hitLeft;
    [Space(5)] 
    [Header("Right Portal")] 
    public PortalToNextRoom portalRight;
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
    
    [Tab("Spawn Room Setting")]

    private void Start()
    {
        
        foreach (GridMover grid in currentGrid)
        {
            GridSpawnManager.Instance.AddGridList(grid);
        }

        if (roomType == RoomType.Clear)
        {
            return;
        }
        RandomStageManager.Instance.AddRoom(this);
    }

    private void StartRoom()
    {
        
        if (roomType == RoomType.Clear || roomType == RoomType.Bonus)
        {
            RoomClear();
            return;
        }
        spawnObstacleMax = Random.Range(spawnObstacleCount.x, spawnObstacleCount.y);
        spawnEnemyMax = Random.Range(spawnEnemyCount.x,spawnEnemyCount.y);
        Invoke("SpawnObstacle",0.1f);
        Invoke("SpawnEnemy",0.2f);
        
        TurnManager.Instance.TurnStart();
    }

    private void SpawnObstacle()
    {
        for (int i = 0; i < spawnObstacleMax; i++)
        {
            Instantiate(obstaclePrefab[Random.Range(0, obstaclePrefab.Count - 1)], CheckSpawnPoint(), Quaternion.identity, obstacleParent);
            spawnedObstacleCount += 1;
            if (spawnedObstacleCount == spawnObstacleMax)
            {
                obstacleSpawnComplete = true;
            }
        }
    }
    private void SpawnEnemy()
    {
        for (int i = 0; i < spawnEnemyMax; i++)
        {
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
            if (currentGrid[randomNumber].gridState == GridState.Empty && currentGrid[randomNumber].isPortal == false)
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
        if (roomType == RoomType.Clear  && obstacleSpawnComplete)
        {
            SpawnPortal();
        }
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
        roomType = RoomType.Clear;
        TurnManager.Instance.currentRoomClear = true;
        SpawnPortal();
        UpdatePortal();
        if (isLastRoom)
        {
            RandomStageManager.Instance.stageClear = true;
            GameManager.Instance.StageClear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GridMover grid in currentGrid)
            {
                grid.gridActive = true;
            }
            if (roomClear)
            {
                return;
            }
            playerTrans = other.GetComponent<Transform>();
            StartRoom();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GridMover grid in currentGrid)
            {
                grid.gridActive = false;
            }
        }
    }

    private void PortalCheck()
    {
        /*forwardPortal = Physics.Raycast(centerPoint.position, Vector3.forward,out hitForward, Mathf.Infinity, roomLayer);
        
        backwardPortal = Physics.Raycast(centerPoint.position, Vector3.back,out hitBackward,Mathf.Infinity, roomLayer);*/
        
        leftPortal = Physics.Raycast(leftPoint.position, Vector3.right,out hitLeft,30f, roomLayer);
        
        rightPortal = Physics.Raycast(rightPoint.position, Vector3.right,out hitRight,30f, roomLayer);
    }

    private void SpawnPortal()
    {
        if (playerTrans == null)
        {
            return;
        }
        /*if (forwardPortal)
        {
            if (portalForward.isConnect)
            {
                return;
            }
            try
            {
                RoomManager roomManager = hitForward.collider.GetComponent<RoomManager>();
                portalForward.SetPortal(roomManager.roomType,roomManager.portalBackward.transform.position,roomManager.transform,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Forward portal not found {a}");
            }
        }
        
        if (backwardPortal)
        {
            if (portalBackward.isConnect)
            {
                return;
            }
            try
            {
                RoomManager roomManager = hitBackward.collider.GetComponent<RoomManager>();
                portalBackward.SetPortal(roomManager.roomType,roomManager.portalForward.transform.position,roomManager.transform,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Backward portal not found {a}");
            }
        }*/
        
        if (leftPortal)
        {
            if (portalLeft.isConnect)
            {
                return;
            }
            try
            {
                RoomManager roomManager = hitLeft.collider.GetComponent<RoomManager>();
                portalLeft.SetPortal(roomManager.roomType,roomManager.startPoint.position,roomManager.transform,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Left portal not found {a}");
            }
        }
        
        if (rightPortal)
        {
            if (portalRight.isConnect)
            {
                return;
            }
            try
            {
                RoomManager roomManager = hitRight.collider.GetComponent<RoomManager>();
                portalRight.SetPortal(roomManager.roomType,roomManager.startPoint.position,roomManager.transform,playerTrans);
            }
            catch (Exception a)
            {
                Debug.Log($"Right portal not found {a}");
            }
        }
    }

    private void UpdatePortal()
    {
        /*if (forwardPortal)
        {
            try
            {
                RoomManager roomManager = hitForward.collider.GetComponent<RoomManager>();
                roomManager.portalBackward.UpdatePortal(roomManager.roomType);
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
                roomManager.portalForward.UpdatePortal(roomManager.roomType);
            }
            catch (Exception a)
            {
                Debug.Log($"Backward portal not found {a}");
            }
        }*/
        
        if (leftPortal)
        {
            try
            {
                RoomManager roomManager = hitLeft.collider.GetComponent<RoomManager>();
                roomManager.portalRight.UpdatePortal(roomManager.roomType);
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
                roomManager.portalLeft.UpdatePortal(roomManager.roomType);
            }
            catch (Exception a)
            {
                Debug.Log($"Right portal not found {a}");
            }
        }
    }
    
}
