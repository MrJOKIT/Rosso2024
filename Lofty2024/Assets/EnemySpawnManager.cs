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
    [Tooltip("If maxSpawn equal 0 is show ti no limit")] public int maxSpawn;
    public int currentSpawn;
}


public class EnemySpawnManager : Singeleton<EnemySpawnManager>
{
    [Tab("Enemy")]
    public RandomRateProfile enemyRandomRate;
    public List<EnemySpawnList> enemySpawnList;
    [Tab("Boss")]
    public List<EnemySpawnList> bossSpawnList;
    
    
    public GameObject GetEnemy()
    {
        int enemyNumber = 0;
        float randomNumber = Random.Range(0, 100f);
        Debug.Log("Enemy = " + randomNumber);
        if (randomNumber <= enemyRandomRate.labigonRate)
        {
            enemyNumber = 0;
        }
        else if (randomNumber <= enemyRandomRate.ironRate)
        {
            enemyNumber = 1;
        }
        else if (randomNumber <= enemyRandomRate.wizardRate)
        {
            enemyNumber = 2;
        }

        if (enemySpawnList[enemyNumber].currentSpawn >= enemySpawnList[enemyNumber].maxSpawn)
        {
            enemyNumber = 0;
            
        }
        enemySpawnList[enemyNumber].currentSpawn += 1;
        return enemySpawnList[enemyNumber].enemyPrefab;
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
        foreach (EnemySpawnList enemy in enemySpawnList)
        {
            enemy.currentSpawn = 0;
        }
    }
}
