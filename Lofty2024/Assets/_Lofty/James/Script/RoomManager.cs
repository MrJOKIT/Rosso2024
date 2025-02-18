using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Combat, Bonus, Boss, Clear }

public class RoomManager : MonoBehaviour
{
    [Header("Room Settings")]
    public RuntimeAnimatorController iconAnimator;
    public bool isStandbyRoom, isLastRoom, roomActive, roomClear, isBossRoom;
    public RoomType roomType;
    public LayerMask roomLayer;
    public Transform playerTrans, startPoint, portalLeft, portalRight;
    
    [Header("Enemy Generator")]
    public Transform bossSpawnPoint, enemyParent;
    public bool enemySpawnComplete, stunAll;
    public List<Enemy> enemyInRoom;
    
    [Header("Room Generator")]
    public bool obstacleSpawnComplete;
    public Vector2Int spawnObstacleCount;
    public Transform obstacleParent;
    public List<GameObject> obstaclePrefab, trapObject;
    public List<GridMover> currentGrid, emptyGrid;
    
    [Header("Item In Room")]
    public List<GameObject> itemInRoom;
    
    private void Start()
    {
        currentGrid.ForEach(grid => GridSpawnManager.Instance.AddGridList(grid));
        UpdateEmptyGrid();
    }

    public void UpdateEmptyGrid()
    {
        emptyGrid = currentGrid.FindAll(grid => grid.gridState == GridState.Empty && !grid.isPortal);
    }

    public void StartRoom()
    {
        if (roomType == RoomType.Clear || roomType == RoomType.Bonus) { RoomClearWithNoReward(); return; }
        
        GameManager.Instance.StartTimer();
        Invoke(nameof(SpawnObstacle), 0.1f);
        Invoke(nameof(SpawnEnemy), 0.2f);
        Invoke(nameof(SpawnTrap), 0.3f);
        TurnManager.Instance.TurnStart();
    }

    private void SpawnObstacle()
    {
        if (isBossRoom) { obstacleSpawnComplete = true; return; }
        while (EnemySpawnManager.Instance.obstacleCost-- > 0)
        {
            Instantiate(EnemySpawnManager.Instance.GetObstacle(), CheckSpawnPoint(), Quaternion.identity, obstacleParent);
        }
        obstacleSpawnComplete = true;
    }

    private void SpawnEnemy()
    {
        if (isBossRoom)
        {
            GameObject enemy = Instantiate(EnemySpawnManager.Instance.GetBossEnemy(), bossSpawnPoint.position + Vector3.up * 0.5f, Quaternion.identity, enemyParent);
            SetupEnemy(enemy);
            enemySpawnComplete = true;
            return;
        }
        while (EnemySpawnManager.Instance.difficultyCost-- > 0)
        {
            GameObject enemy = Instantiate(EnemySpawnManager.Instance.GetEnemy(), CheckSpawnPoint(), Quaternion.identity, enemyParent);
            SetupEnemy(enemy);
        }
        enemySpawnComplete = true;
    }

    private void SetupEnemy(GameObject enemy)
    {
        var enemyComponent = enemy.GetComponent<Enemy>();
        enemyInRoom.Add(enemyComponent);
        enemyComponent.targetTransform = playerTrans;
        enemyComponent.ActiveUnit();
        if (stunAll) enemyComponent.AddCurseStatus(CurseType.Stun, 1);
    }

    private void SpawnTrap()
    {
        while (EnemySpawnManager.Instance.curseCost-- > 0)
        {
            trapObject.Add(Instantiate(EnemySpawnManager.Instance.GetTrap(), CheckSpawnPoint(), Quaternion.identity, transform));
        }
    }

    private void ClearTrap()
    {
        trapObject.ForEach(Destroy);
        trapObject.Clear();
    }

    private void Update()
    {
        if (!roomActive && enemySpawnComplete && obstacleSpawnComplete) roomActive = true;
        if (!roomClear) RoomProgressCheck();
    }

    public Vector3 CheckSpawnPoint()
    {
        int index = UnityEngine.Random.Range(0, emptyGrid.Count);
        GridMover grid = emptyGrid[index];
        emptyGrid.RemoveAt(index);
        return grid.transform.position + Vector3.up * 0.5f;
    }

    public void AddNewEnemyInRoom(GameObject newEnemyPrefab)
    {
        GameObject enemy = Instantiate(newEnemyPrefab, CheckSpawnPoint(), Quaternion.identity, enemyParent);
        SetupEnemy(enemy);
        TurnManager.Instance.AddNewQueue(enemy.transform);
    }

    private void RoomProgressCheck()
    {
        if (!roomActive || roomClear) return;
        if (enemyInRoom.TrueForAll(enemy => enemy.isDead)) RoomClear();
    }

    private void RoomClear()
    {
        roomClear = true;
        roomType = RoomType.Clear;
        SoundManager.instace.Play(SoundManager.SoundName.ClearBGM);
        ClearTrap();
        TurnManager.Instance.CurrentRoomClear();
        
        if (isLastRoom)
        {
            if (isBossRoom) GameManager.Instance.GameClearRevealer();
            else GameManager.Instance.StageClear();
        }
        else
        {
            GameManager.Instance.RoomClear();
            PortalManager.Instance.SetUpNextRoom(portalLeft, portalRight, playerTrans);
        }
        EnemySpawnManager.Instance.ResetSpawnList();
    }

    private void RoomClearWithNoReward()
    {
        roomClear = true;
        roomType = RoomType.Clear;
        TurnManager.Instance.CurrentRoomClear();
        PortalManager.Instance.SetUpNextRoom(portalLeft, portalRight, playerTrans);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerTrans = other.transform;
        stunAll = other.GetComponent<PlayerArtifact>().CreepingTerror;
        GameManager.Instance.UpdateCurrentRoom(transform);
        PortalManager.Instance.StartClearRoom();
        currentGrid.ForEach(grid => grid.gridActive = true);
        if (!roomClear) Invoke(nameof(StartRoom), 2f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        currentGrid.ForEach(grid => grid.gridActive = false);
        playerTrans = null;
        DestroyRoom();
    }
    public void ClearSelectedGird()
    {
        currentGrid.ForEach(grid => grid.onHover = false);
    }

    public void AddItemInRoom(GameObject itemObject)
    {
        itemInRoom.Add(itemObject);
    }

    public void DestroyRoom()
    {
        currentGrid.ForEach(grid => GridSpawnManager.Instance.RemoveGrid(grid));
        itemInRoom.ForEach(Destroy);
        itemInRoom.Clear();
        Destroy(gameObject, 0.1f);
    }
}
