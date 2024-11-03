using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy",fileName = "EnemyData",order = 0)]
public class EnemyData : ScriptableObject
{
    [Header("Data")]
    public string enemyName;
    public Sprite enemySprite;
    [Header("Stats")] 
    public bool isBoss;
    public int enemyMaxHealth;
    public int enemyHealth;
    [Space(10)] 
    public int damage;
    [Range(0,100)]public float enemySpeed;
}
