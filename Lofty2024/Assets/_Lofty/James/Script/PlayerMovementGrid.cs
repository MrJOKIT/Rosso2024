using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
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
    public int damage;
    public List<Button> playerInteractButton;

    [Space(10)] 
    [Header("Combat Setting")] 
    public AttackType attackType = AttackType.NormalAttack;
    public CurseType effectiveType = CurseType.Empty;
    public int effectiveTurnTime = 1;

    [Tab("Movement")]
    [Header("Player Input")]
    public MoveType moveType;
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


    [Header("Move Checker")] 
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
        if (!moveRandom)
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
            if (autoEndTurnAfterMove)
            {
                EndTurn();
            }
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
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GetComponent<PlayerInputHandle>().MoveRandomKeyboard();
        }
    }

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
            SetMover();
        }
        currentState = MovementState.Combat;
    }

    [EditorAttributes.Button("End Turn")]
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
