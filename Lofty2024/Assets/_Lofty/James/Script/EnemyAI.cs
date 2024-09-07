using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King,
}
public class EnemyAI : MonoBehaviour
{
    public EnemyType enemyType;
    public int enemyHealth = 1;
    private void Start()
    {
        
    }

    private void Update()
    {
        if (enemyHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void TestDamage()
    {
        enemyHealth -= 1;
        Debug.Log("Test Enemy Damage");
    }
}
