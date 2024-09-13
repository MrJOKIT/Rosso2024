using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
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
    [ReadOnly] public TurnData enemyTurnData;
    public EnemyData enemyData;
    public EnemyType enemyType;
    public int enemyHealth = 1;
    public float enemySpeed;
    public bool onTurn;

    private void Awake()
    {
        SetEnemyData();
    }

    private void Start()
    {
        TurnManager.Instance.AddUnit(false,transform,enemySpeed);
    }

    private void Update()
    {
        if (enemyHealth <= 0)
        {
            EnemyDie();
        }
    }

    private void EnemyDie()
    {
        gameObject.SetActive(false);
        TurnManager.Instance.RemoveUnit(enemyTurnData);
    }

    public void TestDamage()
    {
        enemyHealth -= 1;
        Debug.Log("Test Enemy Damage");
    }

    private void SetEnemyData()
    { 
        enemyHealth = enemyData.enemyHealth;
        enemySpeed = enemyData.enemySpeed;
    }
}
