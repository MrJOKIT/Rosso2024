using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public enum MovementState
{
    Idle,
    Moving,
}

public enum MovePattern
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King,
}

public enum MoveType
{
    Keyboard,
    Mouse,
    Both,
}

[Serializable]
public class PlayerPattern
{
    public MovePattern patternName;
    public GameObject pattern;
    public List<MoveChecker> moveCheckers;
}

public class PlayerMovementGrid : MonoBehaviour
{
    [Header("Player Input")]
    public MoveType moveType;

    [Space(10)] 
    [Header("Turn Setting")] 
    public bool onTurn;
    public float turnSpeed = 20f;
    
    [Space(10)] 
    [Header("Move Setting")] 
    public MovePattern movePattern;
    public MovementState currentState = MovementState.Idle;
    public float moveSpeed = 5f;
    public Vector3 gridSize = new Vector3(1f, 1f, 1f);
    public LayerMask gridLayerMask;
    
    [Space(10)] 
    [Header("Move Pattern")] 
    public List<PlayerPattern> playerPattern;

    [Header("Hide Condition")] 
    private MovePattern oldPattern;
    private Vector3 targetPosition;

    private void Awake()
    {
        oldPattern = movePattern;
    }

    private void Start()
    {
        targetPosition = transform.position;
        TurnManager.Instance.AddUnit(true,transform,turnSpeed);
    }

    private void Update()
    {
        if (!onTurn)
        {
            return;
        }
        MoveStateHandle();
        PatternHandle();
    }

    private void MoveStateHandle()
    {
        switch (currentState)
        {
            case MovementState.Idle:
                if (TurnManager.Instance.onPlayerTurn)
                {
                    SetMover();
                }
                switch (moveType)
                {
                    case MoveType.Keyboard:
                        HandleInput();
                        break; 
                    case MoveType.Mouse:
                        HandleClickToMove();
                        break;
                    case MoveType.Both:
                        HandleInput();
                        HandleClickToMove();
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
        }
    }
    private void PatternHandle()
    {
        if (oldPattern == movePattern)
        {
            return;
        }

        ClearPattern();
        switch (movePattern)
        {
            case MovePattern.Pawn:
                ClearPattern();
                playerPattern[(int)movePattern].pattern.SetActive(true);
                break;
            case MovePattern.Rook:
                ClearPattern();
                playerPattern[(int)movePattern].pattern.SetActive(true);
                break;
            case MovePattern.Knight:
                ClearPattern();
                playerPattern[(int)movePattern].pattern.SetActive(true);
                break;
            case MovePattern.Bishop:
                ClearPattern();
                playerPattern[(int)movePattern].pattern.SetActive(true);
                break;
            case MovePattern.Queen:
                ClearPattern();
                playerPattern[(int)movePattern].pattern.SetActive(true);
                break;
            case MovePattern.King:
                ClearPattern();
                playerPattern[(int)movePattern].pattern.SetActive(true);
                break;
        }
        SetMover();
        oldPattern = movePattern;
    }

    private void ClearPattern()
    {
        foreach (PlayerPattern patternObject in playerPattern)
        {
            patternObject.pattern.SetActive(false);
        }
    }

    private void HandleInput()
    {
        if (GetComponent<PlayerGridBattle>().GetPlayerMode != PlayerMode.Normal)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            SetTargetPosition(Vector3.right);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SetTargetPosition(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            SetTargetPosition(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            SetTargetPosition(Vector3.back);
        }
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
                        CameraShake.Instance.TriggerShake();
                        hit.collider.GetComponent<GridMover>().enemyAI.TestDamage();
                        if (hit.collider.GetComponent<GridMover>().enemyAI.enemyHealth <= 0)
                        {
                            SetTargetPosition(hit.point);
                        }
                        else
                        {
                            TurnManager.Instance.TurnSucces();
                            onTurn = false;
                        }
                        break;
                }
                
            }
        }
    }

    private void SetTargetPosition(Vector3 direction)
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
        if (movePattern == MovePattern.Knight)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        

        if (transform.position == targetPosition)
        {
            GridSpawnManager.Instance.ClearMover();
            TurnManager.Instance.TurnSucces();
            currentState = MovementState.Idle;
            onTurn = false;
        }
    }

    [Button("Set Mover")]
    private void SetMover()
    {
        foreach (MoveChecker mc in playerPattern[(int)movePattern].moveCheckers)
        {
            mc.SetMover();
        }
        
    }
}
