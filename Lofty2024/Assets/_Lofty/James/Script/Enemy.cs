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
    [ReadOnly] public CurseUI curseUI;
    public bool curseActivated;
    public CurseData(CurseType curseType,int curseTurn,CurseUI curseUI)
    {
        this.curseType = curseType;
        this.curseTurn = curseTurn;
        this.curseUI = curseUI;
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
    public bool isDead;

    [Space(10)] 
    [Header("Curse Status")] 
    public List<CurseData> curseHave;
    public Transform curseCanvas;
    public GameObject curseUiPrefab;


    [Space(10)] 
    [Header("Turn")] 
    public bool autoSkip;
    [ReadOnly] public bool onTurn;

    [Space(10)] 
    [Header("Reward")]
    public AbilityType abilityDrop;

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
        
    }

    public void StartTurn()
    {
        onTurn = true;
        CurseHandle();
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
                    Destroy(curse.curseUI.gameObject);
                    curseHave.Remove(curse);
                }
                else
                {
                    CurseUiUpdate();
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
                    //ลดเลือดไม่ติดเกราะ
                    enemyHealth -= 1;
                    break;
                case CurseType.Burn:
                    //ลดเลือดติดเกราะ
                    enemyHealth -= 1;
                    break;
                case CurseType.Provoke:
                    //เปลื่ยนเป้าหมายไปติดสิ่งยัวยุแทน
                    break;
            }

            curse.curseActivated = true;
        }
        
    }

    public void CurseUiUpdate()
    {
        foreach (CurseData curse in curseHave.ToList())
        {
            curse.curseUI.curseType = curse.curseType;
            curse.curseUI.turnCount.text = curse.curseTurn.ToString();
        }
    }
    private void EnemyDie()
    {
        TurnManager.Instance.RemoveUnit(enemyTurnData);
        isDead = true;
        if (onTurn)
        {
            EndTurn();
        }
        gameObject.SetActive(false);
    }
    
    private void SetEnemyData()
    { 
        enemyHealth = enemyData.enemyHealth;
        enemySpeed = enemyData.enemySpeed;
    }
    public void TakeDamage(int damage)
    {
        CameraShake.Instance.TriggerShake();
        enemyHealth -= damage;
    }

    public void AddCurseStatus(CurseType curseType,int turnTime)
    {
        foreach (CurseData curse in curseHave)
        {
            if (curse.curseType == curseType)
            {
                curse.curseTurn = turnTime;
                CurseUiUpdate();
                return;
            }
        }
        GameObject curseGUI = Instantiate(curseUiPrefab, curseCanvas);
        curseHave.Add(new CurseData(curseType,turnTime,curseGUI.GetComponent<CurseUI>()));
        CurseUiUpdate();
    }
    
}
