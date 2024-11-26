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


public class EnemySpawnManager : Singeleton<EnemySpawnManager>
{
    [Tab("Enemy")] 
    public int defaultDifficultyCost = 10;
    public int difficultyCost;
    public List<EnemySpawnList> enemySpawnList;
    public List<EnemySpawnList> enemyList;
    [Tab("Boss")]
    public List<EnemySpawnList> bossSpawnList;

    private void Start()
    {
        ResetSpawnList();
    }

    private void SetList()
    {
        enemyList.Clear();
        foreach (EnemySpawnList enemy in enemySpawnList)
        {
            if (enemy.cost <= difficultyCost)
            {
                enemyList.Add(enemy);
            }
        }
    }
    public GameObject GetEnemy()
    {
        int randomNumber = Random.Range(0, enemyList.Count - 1);
        GameObject newEnemy = enemyList[randomNumber].enemyPrefab;
        difficultyCost -= enemyList[randomNumber].cost;
        SetList();
        return newEnemy;
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
        SetList();
    }
}
