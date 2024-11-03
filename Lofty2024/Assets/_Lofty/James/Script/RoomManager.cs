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
    public RandomRateProfile randomRate;
    public bool isBossRoom;
    public Transform bossSpawnPoint;
    [Space(10)]
    public bool enemySpawnComplete;
    public bool stunAll;
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
            RoomClearWithNoReward(); 
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
        if (isBossRoom)
        {
            obstacleSpawnComplete = true;
            return;
        }
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
        if (isBossRoom)
        {
            GameObject enemy = Instantiate(enemyPrefab[RandomMonsterNumber()], new Vector3(bossSpawnPoint.position.x,0.5f,bossSpawnPoint.position.z), Quaternion.identity,enemyParent);
            enemyInRoom.Add(enemy.GetComponent<Enemy>());
            enemy.GetComponent<Enemy>().targetTransform = playerTrans;
            enemy.GetComponent<Enemy>().ActiveUnit();
            if (stunAll)
            {
                enemy.GetComponent<Enemy>().AddCurseStatus(CurseType.Stun,1); 
            }
            enemySpawnComplete = true;
            return;
        }
        for (int i = 0; i < spawnEnemyMax; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab[RandomMonsterNumber()], CheckSpawnPoint(), Quaternion.identity,enemyParent);
            enemyInRoom.Add(enemy.GetComponent<Enemy>());
            enemy.GetComponent<Enemy>().targetTransform = playerTrans;
            enemy.GetComponent<Enemy>().ActiveUnit();
            if (stunAll)
            {
                enemy.GetComponent<Enemy>().AddCurseStatus(CurseType.Stun,1); 
            }
            spawnedEnemyCount += 1;
            if (spawnedEnemyCount == spawnEnemyMax)
            {
                enemySpawnComplete = true;
            }
        }
    }

    private int RandomMonsterNumber()
    {
        int monsterNumber = 0;
        float randomNumber = Random.Range(0f, 1f);
        //Debug.Log(randomNumber);
        if (randomNumber < randomRate.pawnRate)
        {
            monsterNumber = 0;
        }
        else if (randomNumber < randomRate.pawnRate + randomRate.rookRate)
        {
            monsterNumber = 1;
        }
        else if (randomNumber < randomRate.pawnRate + randomRate.rookRate + randomRate.knightRate)
        {
            monsterNumber = 2;
        }
        else if (randomNumber < randomRate.pawnRate + randomRate.rookRate + randomRate.knightRate + randomRate.bishopRate)
        {
            monsterNumber = 3;
        }
        else if (randomNumber < randomRate.pawnRate + randomRate.rookRate + randomRate.knightRate + randomRate.bishopRate + randomRate.queenRate)
        {
            monsterNumber = 4;
        }
        else if (randomNumber < randomRate.pawnRate + randomRate.rookRate + randomRate.knightRate + randomRate.bishopRate + randomRate.queenRate + randomRate.kingRate)
        {
            monsterNumber = 5;
        }

        return monsterNumber;
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

    public void AddNewEnemyInRoom(GameObject newEnemyPrefab)
    {
        GameObject enemy = Instantiate(newEnemyPrefab, CheckSpawnPoint(), Quaternion.identity,enemyParent);
        enemyInRoom.Add(enemy.GetComponent<Enemy>());
        enemy.GetComponent<Enemy>().targetTransform = playerTrans;
        enemy.GetComponent<Enemy>().ActiveUnit();
        TurnManager.Instance.AddNewQueue(enemy.transform);
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
        TurnManager.Instance.CurrentRoomClear(); 
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
            GameManager.Instance.RoomClear();
            PortalManager.Instance.SetUpNextRoom(portalLeft,portalRight,playerTrans);
        }
    }
    private void RoomClearWithNoReward()
    {
        roomClear = true; 
        roomType = RoomType.Clear;
        TurnManager.Instance.CurrentRoomClear(); 
        if (isStandbyRoom)
        {
            return;
        }
        
        PortalManager.Instance.SetUpNextRoom(portalLeft,portalRight,playerTrans);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTrans = other.GetComponent<Transform>();
            stunAll = other.GetComponent<PlayerArtifact>().CreepingTerror;
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

    public void ClearSelectedGird()
    {
        foreach (GridMover grid in currentGrid)
        {
            grid.onHover = false;
        }
    }
}
