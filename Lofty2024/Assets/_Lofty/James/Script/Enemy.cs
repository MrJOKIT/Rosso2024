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

public abstract class Enemy : MonoBehaviour,ITakeDamage,IUnit
{
    #region -Declared Variables-
    
    [Tab("Enemy Host")]
    [Header("Player")]
    [SerializeField] protected Transform targetTransform;
    [SerializeField] protected Transform focusTransform;
    
    public Transform TargetTransform { get => targetTransform; set => targetTransform = value; }
    public Transform FocusTransform => focusTransform;

    #region -Enemy Data-
    
    [Header("Data")]
    [SerializeField] private TurnData turnData;
    [SerializeField] protected EnemyData enemyData;
    
    public TurnData TurnData { get => turnData; set => turnData = value; }
    public EnemyData EnemyData => enemyData;

    [Space(10)] 
    [Header("Stats")] 
    [SerializeField] protected GameObject focusArrow;
    [SerializeField] protected Animator animator;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected float speed;
    [SerializeField] protected bool isDead;
    [SerializeField] protected bool onImmortalObject;

    public GameObject FocusArrow => focusArrow;
    public Animator Animator => animator;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float Speed => speed;
    public bool IsDead => isDead;
    public bool OnImmortalObject => onImmortalObject;

    [Space(10)] 
    [Header("Curse Status")] 
    [SerializeField] protected List<CurseData> curseHave;
    [SerializeField] protected Transform curseCanvas;
    [SerializeField] protected GameObject curseUIPrefab;
    [SerializeField] protected Transform bombPrefab;


    [Space(10)] 
    [Header("Turn")] 
    [SerializeField] protected bool skipTurn;
    [SerializeField] protected bool autoSkip;
    [SerializeField] protected bool onTurn;

    public bool OnTurn { get => onTurn; set => onTurn = value; }

    [Space(10)] 
    [Header("Reward")]
    [SerializeField] protected Image rewardImage;
    [SerializeField] protected AbilityType abilityDrop;
    [SerializeField] protected GameObject abilityOrbPrefab;

    #endregion

    [Space(10)] 
    [Foldout("Skill Image")] 
    [SerializeField] protected Sprite pawnSkillImage;
    [SerializeField] protected Sprite rookSkillImage;
    [SerializeField] protected Sprite knightSkillImage;
    [SerializeField] protected Sprite bishopSkillImage;
    [SerializeField] protected Sprite queenSkillImage;
    [SerializeField] protected Sprite kingSkillImage;

    #endregion

    private void Awake()
    {
        SetEnemyData();
    }

    private void SetEnemyData()
    {
        maxHealth = enemyData.enemyMaxHealth;
        currentHealth = maxHealth;
        speed = enemyData.enemySpeed;
    }

    private void SetTurnDirection()
    {
        if (targetTransform.position.x < transform.position.x 
            || targetTransform.position.z > transform.position.z)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (targetTransform.position.x > transform.position.x
                 || targetTransform.position.z < transform.position.z)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void ActiveUnit()
    {
        TurnManager.Instance.AddUnit(false, transform, speed);
        RewardUIHandle();
    }

    public void StartTurn()
    {
        onTurn = true;
        SetTurnDirection();
    }

    public void EndTurn()
    {
        EndTurnModify();
        GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().UpdateEmptyGrid();
        onTurn = false;
        TurnManager.Instance.TurnSuccess();
        if (curseHave.Count <= 0) return;
        
        foreach (var _curse in curseHave.ToList())
        {
            _curse.CurseTurn -= 1;
            if (_curse.CurseTurn <= 0)
            {
                Destroy(_curse.CurseUI.gameObject);
                curseHave.Remove(_curse);
            }
            else
            {
                CurseUIUpdate();
                _curse.CurseActivated = false;
            }
        }
    }

    #region #Cursed Function

    public void AddCurseStatus(CurseType _curseType, int _turnTime)
    {
        foreach (var _curseHave in curseHave.Where(_curse => _curse.CurseType == _curseType))
        {
            _curseHave.CurseTurn = _turnTime;
            CurseUIUpdate();
            return;
        }
        
        var _curseGUI = Instantiate(curseUIPrefab, curseCanvas);
        var _curseUI = _curseGUI.GetComponent<CurseUI>();
        curseHave.Add(new CurseData(_curseType, _turnTime, _curseUI));
        CurseUIUpdate();
    }

    public void CurseHandle()
    {
        if (curseHave.Count <= 0) return;
        
        foreach (var _curse in curseHave.ToList())
        {
            if (_curse.CurseActivated)
                continue;
            
            switch (_curse.CurseType)
            {
                case CurseType.Stun:
                    GridSpawnManager.Instance.ClearMover();
                    skipTurn = true;
                    GetComponent<EnemyMovementGrid>().currentState = MovementState.Idle;
                    break;
                case CurseType.Blood:
                    //ลดเลือดไม่ติดเกราะ
                    currentHealth -= 1;
                    break;
                case CurseType.Burn:
                    //ลดเลือดติดเกราะ
                    currentHealth -= 1;
                    break;
                case CurseType.Provoke:
                    //เปลื่ยนเป้าหมายไปติดสิ่งยัวยุแทน
                    break;
            }

            _curse.CurseActivated = true;
        }
        
    }

    #endregion

    #region #Enemy UI Handle

    public void CurseUIUpdate()
    {
        foreach (var _curse in curseHave.ToList())
        {
            _curse.CurseUI.curseType = _curse.CurseType;
            _curse.CurseUI.turnCount.text = _curse.CurseTurn.ToString();
        }
    }

    private void RewardUIHandle()
    {
        rewardImage.gameObject.SetActive(abilityDrop != AbilityType.Empty);

        rewardImage.sprite = abilityDrop switch
        {
            AbilityType.Pawn => pawnSkillImage,
            AbilityType.Rook => rookSkillImage,
            AbilityType.Knight => knightSkillImage,
            AbilityType.Bishop => bishopSkillImage,
            AbilityType.Queen => queenSkillImage,
            AbilityType.King => kingSkillImage,
            _ => rewardImage.sprite
        };
    }

    #endregion

    protected void EnemyDie()
    {
        VisualEffectManager.Instance.CallEffect(EffectName.EnemyDead,transform,1f);
        MouseSelectorManager.Instance.ClearSelector(this);
        TurnManager.Instance.RemoveUnit(turnData);
        isDead = true;
        
        if (onTurn)
        {
            EndTurn();
        }

        if (abilityDrop != AbilityType.Empty)
        {
            TutorialManager.Instance.ActiveTutorial(TutorialName.HowToGetAbility);
            var _abilityOrbItem = Instantiate(abilityOrbPrefab, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            _abilityOrbItem.GetComponent<AbilityOrb>().SetOrbAbility(abilityDrop);
        }
        
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (onImmortalObject) return;
        
        animator.SetTrigger("TakeDamage");
        CameraManager.Instance.TriggerShake();
        
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            EnemyDie();
        }
    }

    public void BombEnemy()
    {
        GameObject bomb = Instantiate(bombPrefab.gameObject, transform);
        bomb.GetComponent<SkillAction>().ActiveSkill();
        print("bomb enemy activated");
    }

    protected virtual void EndTurnModify()
    {
        
    }
}
