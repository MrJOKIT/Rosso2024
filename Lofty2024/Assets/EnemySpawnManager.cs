using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

[Serializable]
public class EnemySpawnList
{
    public GameObject enemyPrefab;
    public int cost;
}

[Serializable]
public class ObstacleSpawnList
{
    public GameObject obstacle;
    public int cost;
}
public class EnemySpawnManager : Singeleton<EnemySpawnManager>
{
    [Tab("Enemy")] 
    [Header("Normal")]
    public int defaultDifficultyCost = 10;
    public int difficultyCost;
    public List<EnemySpawnList> enemySpawnList;
    public List<EnemySpawnList> enemyList;
    [Header("Boss")]
    public List<EnemySpawnList> bossSpawnList;

    [Tab("Obstacle")] 
    public int defaultObstacleCost = 10;
    public int obstacleCost;
    public List<ObstacleSpawnList> obstacleSpawnList;
    public List<ObstacleSpawnList> obstacleList;
    private void Start()
    {
        ResetSpawnList();
    }

    private void SetEnemyList()
    {
        
        enemyList.Clear();
        foreach (EnemySpawnList enemy in enemySpawnList)
        {
            if (enemy.cost <= difficultyCost)
            {
                enemyList.Add(enemy);
            }
        }
        enemyList = enemyList.OrderBy(x => Random.value).ToList();
    }

    private void SetObstacleList()
    {
        obstacleList.Clear();
        foreach (ObstacleSpawnList obstacle in obstacleSpawnList)
        {
            if (obstacle.cost <= obstacleCost)
            {
                obstacleList.Add(obstacle);
            }
        }
        obstacleList = obstacleList.OrderBy(x => Random.value).ToList();
    }
    public GameObject GetEnemy()
    {
        int randomNumber = Random.Range(0, enemyList.Count - 1);
        GameObject newEnemy = enemyList[randomNumber].enemyPrefab;
        difficultyCost -= enemyList[randomNumber].cost;
        SetEnemyList();
        return newEnemy;
    }

    public GameObject GetEnemyNoCost()
    {
        int randomNumber = Random.Range(0, enemySpawnList.Count - 1);
        GameObject newEnemy = enemySpawnList[randomNumber].enemyPrefab;
        return newEnemy;
    }

    public GameObject GetObstacle()
    {
        int randomNumber = Random.Range(0, obstacleList.Count - 1);
        GameObject newObstacle = obstacleList[randomNumber].obstacle;
        obstacleCost -= obstacleList[randomNumber].cost;
        SetObstacleList();
        return newObstacle;
    }
    
    public GameObject GetBossEnemy()
    {
        int enemyNumber = 0;
        float randomNumber = Random.Range(0, 100f);
        Debug.Log("Enemy = " + randomNumber);
        if (randomNumber <= 33f)
        {
            enemyNumber = 0;
        }
        else if (randomNumber <= 66f)
        {
            enemyNumber = 1;
        }
        else if (randomNumber <= 100f)
        {
            enemyNumber = 2;
        }

        return bossSpawnList[enemyNumber].enemyPrefab;
    }

    public void ResetSpawnList()
    {
        difficultyCost = defaultDifficultyCost;
        obstacleCost = defaultObstacleCost;
        SetEnemyList();
        SetObstacleList();
    }
}
