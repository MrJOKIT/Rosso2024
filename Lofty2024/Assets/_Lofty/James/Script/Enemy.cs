using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("Reward")] public Image rewardImage;
    public AbilityType abilityDrop;
    
    [Space(10)] 
    [Header("Skill Image")] 
    public Sprite emptySkillImage;
    public Sprite pawnSkillImage;
    public Sprite rookSkillImage;
    public Sprite knightSkillImage;
    public Sprite bishopSkillImage;
    public Sprite queenSkillImage;
    public Sprite kingSkillImage;

    private void Awake()
    {
        SetEnemyData();
    }

    private void Start()
    {
        TurnManager.Instance.AddUnit(false,transform,enemySpeed);
        RewardUiHandle();
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
    }

    public void StartTurn()
    {
        onTurn = true;
        if (autoSkip)
        {
            EndTurn();
        }
        else
        {
            CurseHandle();
        }
    }

    public void EndTurn()
    {
        onTurn = false;
        TurnManager.Instance.TurnSucces();
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

    private void RewardUiHandle()
    {
        switch (abilityDrop)
        {
            case AbilityType.Empty:
                rewardImage.gameObject.SetActive(false);
                break;
            case AbilityType.Pawn:
                rewardImage.gameObject.SetActive(true);
                rewardImage.sprite = pawnSkillImage;
                break;
            case AbilityType.Rook:
                rewardImage.gameObject.SetActive(true);
                rewardImage.sprite = rookSkillImage;
                break;
            case AbilityType.Knight:
                rewardImage.gameObject.SetActive(true);
                rewardImage.sprite = knightSkillImage;
                break;
            case AbilityType.Bishop:
                rewardImage.gameObject.SetActive(true);
                rewardImage.sprite = bishopSkillImage;
                break;
            case AbilityType.Queen:
                rewardImage.gameObject.SetActive(true);
                rewardImage.sprite = queenSkillImage;
                break;
            case AbilityType.King:
                rewardImage.gameObject.SetActive(true);
                rewardImage.sprite = kingSkillImage;
                break;
        }
    }
    public void TakeDamage(int damage)
    {
        if (enemyHealth <= 0)
        {
            return;
        }
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
