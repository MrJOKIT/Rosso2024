using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;
using static AbilityType;
using Random = UnityEngine.Random;

public enum MovementState
{
    Idle,
    Combat,
    Moving,
    Freeze,
}

public enum MoveType
{
    Keyboard,
    Mouse,
    Both,
}

public enum AttackType
{
    NormalAttack,
    SpecialAttack, 
    KnockBackAttack,
    EffectiveAttack,
}

public enum PlayerMoveDirection
{
    Forward,
    ForwardLeft,
    ForwardRight,
    Backward,
    BackwardLeft,
    BackwardRight,
    Left,
    Right,
}

public class PlayerMovementGrid : MonoBehaviour, IUnit
{
    

    [Tab("Combat")]
    [Space(10)]
    [Header("Turn Setting")] 
    public bool autoSkip;
    public bool onTurn;
    public float turnSpeed = 20f;
    [SerializeField] private int defaultDamage;
    [SerializeField] private int damage;
    public List<Button> playerInteractButton;

    [Space(10)] 
    [Header("Combat Setting")] 
    public AttackType attackType = AttackType.NormalAttack;
    public CurseType effectiveType = CurseType.Empty;
    public int effectiveTurnTime = 1;
    [SerializeField] private int knockBackRange = 1;
    [SerializeField] private int defaultKnockBackRange = 1;

    [Tab("Movement")] 
    [Header("Player Input")]
    public MoveType moveType;

    [Space(10)] 
    [Header("Movement Point")] 
    [SerializeField] private int defaultMovePoint = 2;
    [SerializeField] private int movePoint;
    [SerializeField] private int maxMovePoint;
    public bool inBattle;
    [Space(5)] 
    public TextMeshProUGUI movePointText;
    public TextMeshProUGUI maxMovePointText;
    [Space(10)]
    [Header("Move Setting")]
    public bool moveSuccess;
    public bool moveRandom;
    public AbilityType movePattern;
    public MovementState currentState = MovementState.Idle;
    public float moveSpeed = 5f;
    public Vector3 gridSize = new Vector3(1f, 1f, 1f);
    public LayerMask gridLayerMask;
    
    [Space(10)] 
    [Header("Move Pattern")] 
    public List<PatternData> patternDatas;
    public Transform parentPattern;
    public Transform currentPattern;


    [Header("Move Checker")]
    public PlayerNavigation playerNavigation;
    public GameObject movePathPrefab;
    public List<GameObject> movePathList;
    public LayerMask moveBlockLayer;
    public bool forwardMoveBlock;
    public bool forwardLeftMoveBlock;
    public bool forwardRightMoveBlock;
    public bool backwardMoveBlock;
    public bool backwardLeftMoveBlock;
    public bool backwardRightMoveBlock;
    public bool leftMoveBlock;
    public bool rightMoveBlock;
    
    [Space(10)]
    [Header("Hide Condition")] 
    private AbilityType oldPattern;
    private Vector3 targetTransform;
    private Vector3 supTargetTransform;
    private Vector3 lastPlayerTransform;

    private void Awake()
    {
        if (moveRandom)
        {
            moveType = MoveType.Keyboard;
        }
        oldPattern = movePattern;
        
        SetStats();
    }

    private void Start()
    {
        targetTransform = transform.position;
        supTargetTransform = transform.position;
        TurnManager.Instance.AddUnit(true,transform,turnSpeed);
        
    }

    private void Update()
    {
        MoveChecker();
        if (GetComponent<PlayerGridBattle>().GetPlayerMode == PlayerMode.Combat)
        {
            if (!onTurn)
            {
                return;
            }
        }
        
        if (autoSkip) 
        {
            TurnManager.Instance.TurnSucces();
            onTurn = false;
            return;
        }
        
        MoveStateHandle();
    }

    #region MovementGrid

    [VInspector.Button("Reset Target")]
    public void ResetPlayerTarget()
    {
        targetTransform = transform.position;
        lastPlayerTransform = transform.position;
        supTargetTransform = transform.position;
    }
    private void MoveStateHandle()
    {
        switch (currentState)
        {
            case MovementState.Idle:
                if (TurnManager.Instance.onPlayerTurn)
                {
                    StartTurn();
                }
                break;
            case MovementState.Combat:
                switch (moveType)
                {
                    case MoveType.Keyboard:
                        GetComponent<PlayerInputHandle>().HandleInput();
                        break; 
                    case MoveType.Mouse:
                        HandleClickToMove();
                        break;
                    case MoveType.Both:
                        HandleClickToMove();
                        if (GetComponent<PlayerGridBattle>().GetPlayerMode == PlayerMode.Combat)
                        {
                            return;
                        }
                        GetComponent<PlayerInputHandle>().HandleInput();
                        break;
                }
                break;
            case MovementState.Moving:
                /*if (moveType == MoveType.Mouse || moveType == MoveType.Both )
                {
                    HandleClickToMove();
                }*/
                if (playerNavigation.navigationSuccess == false)
                {
                    return;
                }
                MoveToTarget();
                break;
            case MovementState.Freeze:
                break;
        }
    }

    

    private void MoveChecker() 
    {
        //Forward Check
        forwardMoveBlock = Physics.Raycast(transform.position, Vector3.forward, 1,moveBlockLayer);
        forwardLeftMoveBlock = Physics.Raycast(transform.position, new Vector3(-1,0,1), 1,moveBlockLayer);
        forwardRightMoveBlock = Physics.Raycast(transform.position, new Vector3(1, 0, 1), 1,moveBlockLayer);
       
        //Backward Check
        backwardMoveBlock = Physics.Raycast(transform.position, Vector3.back, 1, moveBlockLayer);
        backwardLeftMoveBlock = Physics.Raycast(transform.position, new Vector3(-1, 0, -1), 1, moveBlockLayer);
        backwardRightMoveBlock = Physics.Raycast(transform.position, new Vector3(1, 0, -1), 1, moveBlockLayer);
       
        //Left & Right
        leftMoveBlock = Physics.Raycast(transform.position, Vector3.left, 1, moveBlockLayer);
        rightMoveBlock = Physics.Raycast(transform.position, Vector3.right, 1, moveBlockLayer);
    }
    

    private void HandleClickToMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayerMask))
            {
                if (hit.collider.GetComponent<GridMover>() == null || moveSuccess)
                {
                    return;
                }
                switch (hit.collider.GetComponent<GridMover>().gridState)
                {
                    case GridState.OnMove:
                        moveSuccess = true;
                        SetTargetPosition(hit.point);
                        break;
                    case GridState.OnEnemy:
                        if (hit.collider.GetComponent<GridMover>().enemyActive == false)
                        {
                            return;
                        }

                        Enemy enemy = hit.collider.GetComponent<GridMover>().enemy;
                        switch (attackType)
                        {
                            case AttackType.NormalAttack:
                                enemy.TakeDamage(damage);
                                break;
                            case AttackType.SpecialAttack:
                                enemy.TakeDamage(damage * 2); 
                                break;
                            case AttackType.KnockBackAttack:
                                enemy.TakeDamage(damage);
                                enemy.GetComponent<EnemyMovementGrid>().KnockBack(transform,knockBackRange);
                                break;
                            case AttackType.EffectiveAttack:
                                enemy.TakeDamage(damage);
                                enemy.AddCurseStatus(effectiveType,effectiveTurnTime);
                                break;
                        }
                        
                        if (enemy.enemyHealth <= 0)
                        {
                            SetTargetPosition(hit.point);
                        }
                        else
                        {
                            GridSpawnManager.Instance.ClearMover();
                            currentState = MovementState.Idle;
                            moveSuccess = true;
                            EndTurn();
                        }
                        break;
                    case GridState.Empty:
                        if (GetComponent<PlayerGridBattle>().GetPlayerMode == PlayerMode.Combat)
                        {
                            return;
                        }
                        playerNavigation.StartNavigation(hit.point);
                        SetTargetPosition(hit.point);
                        break;
                }
                
            }
        }
    }

    private void AddMovePath(Vector3 spawnPosition,PlayerMoveDirection direction)
    {
        GameObject movePathManager = Instantiate(movePathPrefab,spawnPosition,Quaternion.identity);
        movePathManager.GetComponent<MovePathManager>().SetPath(direction);
        movePathList.Add(movePathManager);
    }
    private void ClearMovePath()
    {
        foreach (GameObject movePaths in movePathList.ToList())
        {
            Destroy(movePaths);
            movePathList.Remove(movePaths);
        }
    }

    public void SetTargetPosition(Vector3 direction)
    {
        if (!moveRandom && GetComponent<PlayerGridBattle>().GetPlayerMode == PlayerMode.Combat)
        {
            GridSpawnManager.Instance.ClearMover();
        }
        Vector3 nextPosition;

        if (currentState == MovementState.Idle || direction != targetTransform - transform.position)
        {
            if (direction == Vector3.forward || direction == Vector3.back || direction == Vector3.left || direction == Vector3.right)
            {
                nextPosition = transform.position + Vector3.Scale(direction, gridSize);
            }
            else
            {
                // Handling grid snapping for mouse click input
                float gridX = Mathf.Round(direction.x / gridSize.x) * gridSize.x;
                float gridZ = Mathf.Round(direction.z / gridSize.z) * gridSize.z;
                nextPosition = new Vector3(gridX, transform.position.y, gridZ);
            }

            targetTransform = nextPosition;
            currentState = MovementState.Moving;
        }
    }

    private void MoveToTarget()
    {
        if (movePattern == Knight)
        {
            transform.position = targetTransform;
        }
        else
        {
            if (transform.position != supTargetTransform )
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, supTargetTransform, moveSpeed * Time.deltaTime);
                if (forwardMoveBlock && forwardLeftMoveBlock && forwardRightMoveBlock && backwardMoveBlock && backwardLeftMoveBlock && backwardRightMoveBlock && leftMoveBlock && rightMoveBlock)
                {
                    ClearMovePath();
                }
            }
            else
            {
                MoveHandle();
            }
            //transform.position = Vector3.MoveTowards(transform.position,targetTransform,moveSpeed * Time.deltaTime);
        }
        
        if (transform.position == targetTransform)
        {
            ClearMovePath();
            currentState = MovementState.Idle;
            GetComponent<PlayerAbility>().CheckAbilityUse();
            EndTurn();
        }
    }

    public void SetPlayerMoveDirection(PlayerMoveDirection direction)
    {
        lastPlayerTransform = transform.position;
        switch (direction)
        {
            case PlayerMoveDirection.Forward:
                supTargetTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.ForwardLeft:
                supTargetTransform = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.ForwardRight:
                supTargetTransform = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.Backward:
                supTargetTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.BackwardLeft:
                supTargetTransform = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.BackwardRight:
                supTargetTransform = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.Left:
                supTargetTransform = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.Right:
                supTargetTransform = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
        }
        AddMovePath(lastPlayerTransform,direction);
    }
    private void MoveHandle()
    {
        if (transform.position.z < targetTransform.z && transform.position.x == targetTransform.x)
        {
            //Move Forward
            
            if (!forwardMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Forward);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                }
                else if (!forwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Right);
                    }
                    else
                    {
                        if (!backwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!backwardMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                            }
                        }
                    }
                }
            }
            
        }
        else if (transform.position.z < targetTransform.z && transform.position.x < targetTransform.x)
        {
            if (!forwardRightMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                }
                else if (!rightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Right);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!leftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Left);
                        }
                        else if (!backwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardLeftMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                            }
                        }
                    }
                }
            }
        }
        else if (transform.position.z < targetTransform.z && transform.position.x > targetTransform.x)
        {
            if (!forwardLeftMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                }
                else if (!leftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Left);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!rightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Right);
                        }
                        else if (!backwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardRightMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                            }
                        }
                    }
                }
            }
        }

        if (transform.position.z > targetTransform.z && transform.position.x == targetTransform.x)
        {
            //Move Backward
            if (!backwardMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Backward);
            }
            else
            {
                if (!backwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                }
                else if (!backwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Right);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                        }
                        else if (!forwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                        }
                        else
                        {
                            if (!forwardMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                            }
                        }
                    }
                }
            }
            
            
        }
        else if (transform.position.z > targetTransform.z && transform.position.x < targetTransform.x)
        {
            if (!backwardRightMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
            }
            else
            {
                if (!rightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Right);
                }
                else if (!backwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                        }
                        else if (!leftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Left);
                        }
                        else
                        {
                            if (!forwardLeftMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                            }
                        }
                    }
                }
            }
        }
        else if (transform.position.z > targetTransform.z && transform.position.x > targetTransform.x)
        {
            if (!backwardLeftMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
            }
            else
            {
                if (!leftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Left);
                }
                else if (!backwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                        }
                        else if (!rightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Right);
                        }
                        else
                        {
                                if (!forwardRightMoveBlock)
                                {
                                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                                }
                                else
                                {
                                    Debug.Log("Can't move");
                                }
                        }
                    }
                }
            }
        }
        
        if (transform.position.x > targetTransform.x && transform.position.z == targetTransform.z)
        {
            //Move left
            if (!leftMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Left);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                }
                else if (!backwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!rightMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Right);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                            }
                        }
                    }
                }
            }
        }
        
        if (transform.position.x < targetTransform.x && transform.position.z == targetTransform.z)
        {
            //Move right
            if (!rightMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Right);
            }
            else
            {
                if (!forwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                }
                else if (!backwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                        }
                        else if (!backwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                        }
                        else
                        {
                            if (!leftMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Left);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                            }
                        }
                    }
                }
            }
        }

        
    }
    
    private void ClearPattern()
    {
        if (currentPattern != null)
        {
            Destroy(currentPattern.gameObject);
            currentPattern = null;
        }

        currentState = MovementState.Idle;
    }

    [EditorAttributes.Button("Set Mover")]
    private void SetMover()
    {
        currentPattern = Instantiate(patternDatas[(int)movePattern - 1].patternPrefab, parentPattern);
        currentPattern.GetComponent<MoverCheckerHost>().CheckMove();
    }


    public void ChangePatternNow(AbilityType newPattern)
    {
        if (currentPattern != null)
        {
            Destroy(currentPattern.gameObject);
            currentPattern = null;
        }
        GridSpawnManager.Instance.ClearMover();
        movePattern = newPattern;
        SetMover();
        
    }

    #endregion

    #region TurnHandle

    public void StartTurn()
    {
        ResetPlayerTarget();
        onTurn = true;
        if (moveSuccess)
        {
            return;
        }

        foreach (Button button in playerInteractButton)
        {
            button.interactable = true;
        }

        if (GetComponent<PlayerGridBattle>().GetPlayerMode == PlayerMode.Combat)
        {
            if (inBattle == false)
            {
                movePoint = maxMovePoint;
                inBattle = true;
                MovementPointInterfaceUpdate();
            }
            SetMover();
        }
        else
        {
            if (movePoint <= 0)
            {
                movePoint = maxMovePoint;
                MovementPointInterfaceUpdate();
            }
        }
        currentState = MovementState.Combat;
    }

    
    public void EndTurn()
    {
        if (movePattern != King)
        {
            movePattern = King;
        }
        foreach (Button button in playerInteractButton)
        {
            button.interactable = false;
        }
        ClearPattern();
        onTurn = false;
        moveSuccess = false;
        if (GetComponent<PlayerGridBattle>().GetPlayerMode == PlayerMode.Combat)
        {
            movePoint -= 1;
            ChaosManager.Instance.IncreaseChaosPoint(1);
            MovementPointInterfaceUpdate();
            if (movePoint <= 0)
            {
                GetComponent<PlayerSkillHandle>().AddSkillPoint(1);
                TurnManager.Instance.TurnSucces();
                inBattle = false;
            }
            else
            {
                StartTurn();
            }
            
        }
        else
        {
            playerNavigation.ClearNavigation();
        }
        
    }

    [Button("End Turn")]
    public void EndTurnPermanent()
    {
        if (movePattern != King)
        {
            movePattern = King;
        }
        foreach (Button button in playerInteractButton)
        {
            button.interactable = false;
        }
        ClearPattern();
        onTurn = false;
        moveSuccess = false;
        TurnManager.Instance.TurnSucces();
        inBattle = false;
    }

    #endregion


    #region MovementPoint

    private void IncreaseMovementPoint()
    {
        movePoint += 1;
        MovementPointInterfaceUpdate();
    }

    private void DecreaseMovementPoint()
    {
        movePoint -= 1;
        MovementPointInterfaceUpdate();
    }

    private void MovementPointInterfaceUpdate()
    {
        movePointText.text = movePoint.ToString();
        maxMovePointText.text = maxMovePoint.ToString();
    }

    #endregion
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GetComponent<PlayerInputHandle>().MoveRandomKeyboard();
        }
    }

    public void SetStats()
    {
        damage = defaultDamage + GetComponent<PlayerArtifact>().Damage;
        maxMovePoint = defaultMovePoint + GetComponent<PlayerArtifact>().ActionPoint;
        knockBackRange = defaultKnockBackRange + GetComponent<PlayerArtifact>().KnockBackRange;
    }
   
}
