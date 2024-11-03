using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

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
    public int curseIndex;
    public CurseType curseType;
    public int curseTurn;
    public CurseUI curseUI;
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
    [Tab("Enemy Host")]
    [Header("Player")]
    public Transform targetTransform;
    
    [Header("Data")]
    public TurnData enemyTurnData;
    public EnemyData enemyData;

    [Space(10)] 
    [Header("Stats")] 
    public Animator enemyAnimator;
    public int enemyMaxHealth;
    public int enemyHealth;
    public float enemySpeed;
    public bool isDead;

    [Space(10)] 
    [Header("Curse Status")] 
    public List<CurseData> curseHave;
    public Transform curseCanvas;
    public GameObject curseUiPrefab;
    public Transform bombPrefab;


    [Space(10)] 
    [Header("Turn")] 
    public bool skipTurn;
    public bool autoSkip;
    public bool onTurn;

    [Space(10)] 
    [Header("Reward")]
    public Image rewardImage;
    public AbilityType abilityDrop;
    public GameObject abilityOrbPrefab;
    
    [Space(10)] 
    [Foldout("Skill Image")] 
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

    public void ActiveUnit()
    {
        TurnManager.Instance.AddUnit(false,transform,enemySpeed);
        RewardUiHandle();
    }
    
    public void StartTurn()
    {
        onTurn = true;
        
    }

    public void EndTurn()
    {
        EndTurnModify();
        onTurn = false;
        TurnManager.Instance.TurnSucces();
        if (curseHave.Count != 0)
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
        if (curseHave.Count == 0)
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
                    GridSpawnManager.Instance.ClearMover();
                    skipTurn = true;
                    GetComponent<EnemyMovementGrid>().currentState = MovementState.Idle;
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
        MouseSelectorManager.Instance.ClearSelector(this);
        TurnManager.Instance.RemoveUnit(enemyTurnData);
        isDead = true;
        if (onTurn)
        {
            EndTurn();
        }

        if (abilityDrop != AbilityType.Empty)
        {
            GameObject abilityOrbItem =Instantiate(abilityOrbPrefab, transform.position, Quaternion.identity);
            abilityOrbItem.GetComponent<AbilityOrb>().SetOrbAbility(abilityDrop);
        }
        
        gameObject.SetActive(false);
    }
    
    private void SetEnemyData()
    {
        enemyMaxHealth = enemyData.enemyMaxHealth;
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
        enemyAnimator.SetTrigger("TakeDamage");
        CameraManager.Instance.TriggerShake();
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            EnemyDie();
        }
    }

    public void BombEnemy()
    {
        GameObject bomb = Instantiate(bombPrefab.gameObject, transform);
        bomb.GetComponent<SkillAction>().ActiveSkill();
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

    private void OnMouseOver()
    {
        Debug.Log("See Data");
    }

    protected virtual void EndTurnModify()
    {
        
    }
}
