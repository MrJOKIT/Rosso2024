using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyMoveDirection
{
    Forward, Backward, Left, Right,
    ForwardLeft, ForwardRight, BackwardLeft, BackwardRight
}

public class EnemyMovementGrid : MonoBehaviour
{
    private Enemy enemy;
    
    [Header("Move Settings")] 
    public Transform movePattern;
    public MovementState currentState;
    public float moveSpeed = 5f;
    public float knockBackSpeed = 100f;
    public Vector3 gridSize = Vector3.one;
    
    [Header("KnockBack Properties")]
    public bool isKnockBack;
    public EnemyMoveDirection enemyMoveDirection;
    public LayerMask obstacleLayer;
    
    [Header("Check Obstacle")]
    public bool forwardBlock;
    public bool forwardLeftBlock;
    public bool forwardRightBlock;
    public bool backwardBlock;
    public bool backwardLeftBlock;
    public bool backwardRightBlock;
    public bool leftBlock;
    public bool rightBlock;
    
    public Vector3 targetPosition;
    private static readonly Vector3[] moveDirections =
    {
        Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
        new Vector3(-1, 0, 1), new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1), new Vector3(1, 0, -1)
    };

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        CheckBlockHandle();
        MoveStateHandle();
    }

    public void KnockBack(Transform playerTrans, int gridDistance)
    {
        if (enemy.enemyData.isBoss) return;
        
        isKnockBack = true;
        enemyMoveDirection = GetBackDirection(playerTrans);
        
        if (!IsBlocked(enemyMoveDirection))
        {
            SetTargetPosition(transform.localPosition + moveDirections[(int)enemyMoveDirection] * gridDistance);
        }
    }
    
    private EnemyMoveDirection GetBackDirection(Transform playerTransform)
    {
        Vector3 direction = transform.position - playerTransform.position;
        return (EnemyMoveDirection)Array.IndexOf(moveDirections, direction.normalized);
    }
    
    private bool IsBlocked(EnemyMoveDirection direction)
    {
        return Physics.Raycast(transform.position, moveDirections[(int)direction], 1, obstacleLayer);
    }

    private void CheckBlockHandle()
    {
        forwardBlock = Physics.Raycast(transform.position, Vector3.forward, 1, obstacleLayer);
        forwardLeftBlock = Physics.Raycast(transform.position, new Vector3(-1, 0, 1), 1, obstacleLayer);
        forwardRightBlock = Physics.Raycast(transform.position, new Vector3(1, 0, 1), 1, obstacleLayer);
        backwardBlock = Physics.Raycast(transform.position, Vector3.back, 1, obstacleLayer);
        backwardLeftBlock = Physics.Raycast(transform.position, new Vector3(-1, 0, -1), 1, obstacleLayer);
        backwardRightBlock = Physics.Raycast(transform.position, new Vector3(1, 0, -1), 1, obstacleLayer);
        leftBlock = Physics.Raycast(transform.position, Vector3.left, 1, obstacleLayer);
        rightBlock = Physics.Raycast(transform.position, Vector3.right, 1, obstacleLayer);
    }
    
    private void MoveStateHandle()
    {
        if (currentState == MovementState.Moving)
        {
            MoveToTarget();
        }
    }

    private void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        currentState = MovementState.Moving;
    }
    
    private void MoveToTarget()
    {
        float speed = isKnockBack ? knockBackSpeed : moveSpeed;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.deltaTime);
        
        if (transform.localPosition == targetPosition)
        {
            currentState = MovementState.Idle;
            isKnockBack = false;
            enemy.enemyAnimator.SetBool("OnMove", false);
            enemy.EndTurn();
        }
    }
    
    public void MoveDirection(EnemyMoveDirection direction)
    {
        if (!IsBlocked(direction))
        {
            SetTargetPosition(transform.localPosition + Vector3.Scale(moveDirections[(int)direction], gridSize));
            enemy.enemyAnimator.SetBool("OnMove", true);
        }
    }
    
    private void OnDrawGizmos()
    {
        foreach (var dir in moveDirections)
        {
            Debug.DrawRay(transform.position, dir, Color.green);
        }
    }

    public bool CheckIsMoving()
    {
        List<bool> _blocks = new List<bool>
        {
            forwardBlock,
            forwardLeftBlock,
            forwardRightBlock,
            backwardBlock,
            backwardLeftBlock,
            backwardRightBlock,
            leftBlock,
            rightBlock
        };

        var isMoving = _blocks.All(value => value);
        return isMoving;
    }
}
