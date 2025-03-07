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
    public Transform focusTransform;
    
    [Header("Data")]
    public TurnData enemyTurnData;
    public EnemyData enemyData;

    [Space(10)] 
    [Header("Stats")] 
    public GameObject focusArrow;
    public Animator enemyAnimator;
    public int enemyMaxHealth;
    public int enemyHealth;
    public float enemySpeed;
    public bool isDead;
    public bool onImmortalObject;

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

    protected EnemyMovementGrid enemyMovementGrid;

    private void Awake()
    {
        enemyMovementGrid = GetComponent<EnemyMovementGrid>();
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
        SetTurnDirection();
    }

    private void SetTurnDirection()
    {
        if (targetTransform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (targetTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (targetTransform.position.z > transform.position.z)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (targetTransform.position.z < transform.position.z)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void EndTurn()
    {
        EndTurnModify();
        GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().UpdateEmptyGrid();
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
    protected void EnemyDie()
    {
        VisualEffectManager.Instance.CallEffect(EffectName.EnemyDead,transform,1f);
        MouseSelectorManager.Instance.ClearSelector(this);
        TurnManager.Instance.RemoveUnit(enemyTurnData);
        isDead = true;
        if (onTurn)
        {
            EndTurn();
        }

        if (abilityDrop != AbilityType.Empty)
        {
            TutorialManager.Instance.ActiveTutorial(TutorialName.HowToGetAbility);
            GameObject abilityOrbItem =Instantiate(abilityOrbPrefab, new Vector3(transform.position.x,0,transform.position.z), Quaternion.identity);
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
        if (onImmortalObject)
        {
            return;
        }
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
