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
            
        }
    }
    
    

    public void DestroyRoom()
    {
        foreach (GridMover grid in currentGrid)
        {
            GridSpawnManager.Instance.RemoveGrid(grid);
        }
        Destroy(gameObject,0.1f);
    }
}
