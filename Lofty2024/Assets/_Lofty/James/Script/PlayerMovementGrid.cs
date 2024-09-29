using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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

[RequireComponent(typeof(PlayerInputHandle))]
public class PlayerMovementGrid : MonoBehaviour, IUnit
{
    [Header("Player Input")]
    public MoveType moveType;

    [Space(10)]
    [Header("Turn Setting")] 
    public bool autoSkip;
    public bool onTurn;
    public float turnSpeed = 20f;
    public int damage;
    public List<Button> playerInteractButton;

    [Space(10)] 
    [Header("Combat Setting")] 
    public AttackType attackType = AttackType.NormalAttack;
    public CurseType effectiveType = CurseType.Empty;
    public int effectiveTurnTime = 1;

    [Space(10)] 
    [Header("Move Setting")]
    public bool moveSuccess;
    public bool autoEndTurnAfterMove;
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
    [ReadOnly] public Transform currentPattern;

    [Space(10)] 
    [Header("Keyboard Check")] 
    public bool canForward;
    public Transform forwardChecker;
    [Space(5)]
    public bool canBackward;
    public Transform backwardChecker;
    [Space(5)]
    public bool canLeft;
    public Transform leftChecker;
    [Space(5)]
    public bool canRight;
    public Transform rightChecker;
    [Space(5)] 
    [Header("Move Checker")] 
    public LayerMask moveBlockLayer;
    public bool moveForwardBlock;
    public bool moveBackwardBlock;
    public bool moveLeftBlock;
    public bool moveRightBlock;
    
    [Space(10)]
    [Header("Hide Condition")] 
    private AbilityType oldPattern;
    private Vector3 targetPosition;

    private void Awake()
    {
        if (moveRandom)
        {
            moveType = MoveType.Keyboard;
        }
        oldPattern = movePattern;
    }

    private void Start()
    {
        targetPosition = transform.position;
        TurnManager.Instance.AddUnit(true,transform,turnSpeed);
        
    }

    private void Update()
    {
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

        canForward = Physics.Raycast(forwardChecker.position, Vector3.down, 10f,gridLayerMask);
        canBackward = Physics.Raycast(backwardChecker.position, Vector3.down, 10f, gridLayerMask);
        canLeft = Physics.Raycast(leftChecker.position, Vector3.down, 10f, gridLayerMask);
        canRight = Physics.Raycast(rightChecker.position, Vector3.down, 10, gridLayerMask);
        MoveChecker();
        
        MoveStateHandle();
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
                if (moveType == MoveType.Mouse || moveType == MoveType.Both )
                {
                    HandleClickToMove();
                }
                MoveToTarget();
                break;
            case MovementState.Freeze:
                break;
        }
    }

    

    private void MoveChecker()
    {
        moveForwardBlock = Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.forward, 1, moveBlockLayer);
        moveBackwardBlock = Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.back, 1, moveBlockLayer);
        moveLeftBlock = Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.left, 1, moveBlockLayer);
        moveRightBlock = Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.right, 1, moveBlockLayer);
    }
    

    private void HandleClickToMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayerMask))
            {
                if (hit.collider.GetComponent<GridMover>() == null)
                {
                    return;
                }
                switch (hit.collider.GetComponent<GridMover>().gridState)
                {
                    case GridState.OnMove:
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
                                enemy.GetComponent<EnemyMovementGrid>().KnockBack(transform,1);
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
                            if (autoEndTurnAfterMove)
                            {
                                EndTurn();
                            }
                        }
                        break;
                    case GridState.Empty:
                        if (GetComponent<PlayerGridBattle>().GetPlayerMode == PlayerMode.Combat)
                        {
                            return;
                        }
                        SetTargetPosition(hit.point);
                        break;
                }
                
            }
        }
    }

    public void SetTargetPosition(Vector3 direction)
    {
        Vector3 nextPosition;

        if (currentState == MovementState.Idle || direction != targetPosition - transform.position)
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

            targetPosition = nextPosition;
            currentState = MovementState.Moving;
        }
    }

    private void MoveToTarget()
    {
        if (movePattern == Knight)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        

        if (transform.position == targetPosition)
        {
            if (!moveRandom)
            {
                GridSpawnManager.Instance.ClearMover();
            }
            currentState = MovementState.Idle;
            moveSuccess = true;
            GetComponent<PlayerAbility>().CheckAbilityUse();
            if (autoEndTurnAfterMove)
            {
                EndTurn();
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

    [Button("Set Mover")]
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
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GetComponent<PlayerInputHandle>().MoveRandomKeyboard();
        }
    }

    public void StartTurn()
    {
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
            SetMover();
        }
        currentState = MovementState.Combat;
    }

    [Button("End Turn")]
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
        TurnManager.Instance.TurnSucces();
        onTurn = false;
        moveSuccess = false;
    }
}
