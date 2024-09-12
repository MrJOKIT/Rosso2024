using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy",fileName = "EnemyData",order = 0)]
public class EnemyData : ScriptableObject
{
    public int enemyHealth;
    [Range(0,100)]public float enemySpeed;
}
