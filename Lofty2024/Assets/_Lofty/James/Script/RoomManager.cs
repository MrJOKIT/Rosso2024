using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool isStandbyRoom;
    public bool isLastRoom;
    public bool roomActive;
    public RoomType roomType;
    [Space(10)]
    public LayerMask roomLayer;
    [Space(5)]
    public Transform playerTrans;
    [Space(5)]
    public Transform startPoint;

    [Header("Portal Position")] 
    public Transform portalLeft;
    public Transform portalRight;
    
    [Tab("Enemy Generator")] 
    public bool enemySpawnComplete;
    public Vector2Int spawnEnemyCount;
    private int spawnedEnemyCount;
    private int spawnEnemyMax;
    
    public Transform enemyParent;
    public List<GameObject> enemyPrefab;
    public List<Enemy> enemyInRoom;
    public bool roomClear;

    [Tab("Room Generator")] 
    public bool obstacleSpawnComplete;
    public Vector2Int spawnObstacleCount;
    private int spawnedObstacleCount;
    private int spawnObstacleMax;
    
    public Transform obstacleParent;
    public List<GameObject> obstaclePrefab;
    public List<GridMover> currentGrid;
    public List<GridMover> emptyGrid;
 
    [Tab("Item In Room")] 
    public List<GameObject> itemInRoom;

    private void Start()
    {
        
        foreach (GridMover grid in currentGrid)
        {
            GridSpawnManager.Instance.AddGridList(grid);
        }

        foreach (GridMover grid in currentGrid)
        {
            if (grid.gridState == GridState.Empty && grid.isPortal == false)
            {
                emptyGrid.Add(grid);
            }
        }
    }

    private void StartRoom()
    {
        if (roomType == RoomType.Clear || roomType == RoomType.Bonus)
        {
            RoomClear();
            return;
        }
        
        SpawnObstacle();
        
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
        int randomNumber = Random.Range(0, emptyGrid.Count - 1);
        GridMover grid = emptyGrid[randomNumber];
        Vector3 spawnPoint = new Vector3(grid.transform.position.x,0.5f,grid.transform.position.z);
        emptyGrid.Remove(grid);
        return spawnPoint;
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
        if (isStandbyRoom)
        {
            return;
        }
        
        if (isLastRoom)
        {
            GameManager.Instance.StageClear();
        }
        else
        {
            PortalManager.Instance.SetUpNextRoom(portalLeft,portalRight,playerTrans);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTrans = other.GetComponent<Transform>();
            GameManager.Instance.UpdateCurrentRoom(transform);
            PortalManager.Instance.StartClearRoom();
            foreach (GridMover grid in currentGrid)
            {
                grid.gridActive = true;
            }
            if (roomClear)
            {
                return;
            }
            StartRoom();
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            try
            {
                foreach (GridMover grid in currentGrid)
                {
                    grid.gridActive = false;
                }
            }
            catch (Exception a)
            {
                Debug.Log($"Gird is gone {a}");
            }
            
            playerTrans = null;
            DestroyRoom();
        }
    }


    public void AddItemInRoom(GameObject itemObject)
    {
        itemInRoom.Add(itemObject);
    }
    
    public void DestroyRoom()
    {
        foreach (GridMover grid in currentGrid)
        {
            GridSpawnManager.Instance.RemoveGrid(grid);
        }

        foreach (GameObject item in itemInRoom.ToList())
        {
            itemInRoom.Remove(item);
            Destroy(item);
        }
        Destroy(gameObject,0.1f);
    }
}
