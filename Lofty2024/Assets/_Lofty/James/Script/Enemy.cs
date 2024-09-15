using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
public enum CurseType
{
    Empty,
    Stun,
    Blood,
    Burn,
    Provoke,
}

[Serializable]
public class CurseData
{
    [ReadOnly] public int curseIndex;
    [ReadOnly] public CurseType curseType;
    [ReadOnly] public int curseTurn;
    public bool curseActivated;
    public CurseData(CurseType curseType,int curseTurn)
    {
        this.curseType = curseType;
        this.curseTurn = curseTurn;
    }
}
public abstract class Enemy : MonoBehaviour,ITakeDamage,IUnit
{
    [Header("Data")]
    [ReadOnly] public TurnData enemyTurnData;
    public EnemyData enemyData;
    
    [Space(10)]
    [Header("Stats")]
    public int enemyHealth;
    public float enemySpeed;

    [Space(10)] 
    [Header("Curse Status")] 
    public List<CurseData> curseHave;


    [Space(10)] 
    [Header("Turn")] 
    public bool autoSkip;
    [ReadOnly] public bool onTurn;

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
            return;
        }

        if (!onTurn)
        {
            return;
        }

        if (autoSkip)
        {
            EndTurn();
        }
        CurseHandle();
    }

    public void StartTurn()
    {
        onTurn = true;
        if (curseHave != null)
        {
            foreach (CurseData curse in curseHave)
            {
                //deal damage with curse
            }
        }
    }

    public void EndTurn()
    {
        onTurn = false;
        if (curseHave != null)
        {
            foreach (CurseData curse in curseHave.ToList())
            {
                curse.curseTurn -= 1;
                if (curse.curseTurn <= 0 )
                {
                    curseHave.Remove(curse);
                }
                else
                {
                    curse.curseActivated = false;
                }
            }
        }
        
        TurnManager.Instance.TurnSucces(false);
    }
    public void CurseHandle()
    {
        if (curseHave == null)
        {
            return;
        }
        foreach (CurseData curse in curseHave.ToList())
        {
            if (curse.curseActivated)
            {
                continue;
            }
            switch (curse.curseType)
            {
                case CurseType.Stun:
                    EndTurn();
                    break;
                case CurseType.Blood:
                    //ลดช่องเดิน 1 ตามั้ง?
                    enemyHealth -= 1;
                    break;
                case CurseType.Burn:
                    enemyHealth -= 1;
                    break;
                case CurseType.Provoke:
                    //คิดก่อน มีทำไม
                    break;
            }

            curse.curseActivated = true;
        }
        
    }
    private void EnemyDie()
    {
        gameObject.SetActive(false);
        TurnManager.Instance.RemoveUnit(enemyTurnData);
    }
    
    private void SetEnemyData()
    { 
        enemyHealth = enemyData.enemyHealth;
        enemySpeed = enemyData.enemySpeed;
    }
    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;
    }

    public void AddCurseStatus(CurseType curseType,int turnTime)
    {
        curseHave.Add(new CurseData(curseType,turnTime));
    }
    
}
