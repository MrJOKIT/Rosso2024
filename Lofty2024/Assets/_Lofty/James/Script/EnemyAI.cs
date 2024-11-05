using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using VInspector;

public class EnemyAI : Enemy
{
    [Tab("Move Checker")]
    public LayerMask moveBlockLayer;
    [Space(10)]
    public bool forwardMoveBlock;
    public bool forwardLeftMoveBlock;
    public bool forwardRightMoveBlock;
    public bool backwardMoveBlock;
    public bool backwardLeftMoveBlock;
    public bool backwardRightMoveBlock;
    public bool leftMoveBlock;
    public bool rightMoveBlock;

    [Tab("Combat")]
    [Header("Combat Checker")]
    public bool playerInRange;
    public LayerMask gridLayer;
    public List<Transform> combatChecker;
    private void Update()
    {
        ChangeGridMoverUnder();
        CheckMoveHandle();
        EnemyCombatHandle();
        if (onTurn == false)
        {
            return;
        }
        switch (GetComponent<EnemyMovementGrid>().currentState)
        {
            case MovementState.Idle: 
                CurseHandle();
                 
                if (autoSkip || skipTurn)
                {
                    EndTurn();
                    skipTurn = false;
                }
                else
                {
                    if (playerInRange)
                    {
                        //Combat time
                        enemyAnimator.SetTrigger("Attack");
                        targetTransform.GetComponent<Player>().TakeDamage(enemyData.damage);
                        EndTurn();
                    }
                    else
                    {
                        EnemyMoveToPlayer();
                    }
                }
                
                break;
            case MovementState.Moving:
                if (forwardMoveBlock && backwardMoveBlock && leftMoveBlock && rightMoveBlock && forwardLeftMoveBlock && forwardRightMoveBlock && backwardLeftMoveBlock && backwardRightMoveBlock)
                {
                    EndTurnModify();
                }
                break;
                
        }
    }

    private void ChangeGridMoverUnder()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin,Vector3.down,out hit,10,gridLayer))
        {
            if (hit.collider.GetComponent<GridMover>() != null)
            {
                if (hit.collider.GetComponent<GridMover>().gridState != GridState.OnEnemy)
                {
                    hit.collider.GetComponent<GridMover>().enemy = this; 
                    hit.collider.GetComponent<GridMover>().gridState = GridState.OnEnemy;
                }
            }
        }
    }


    #region Enemy Move
    
    private void EnemyMoveToPlayer()
    {
        if (forwardMoveBlock && forwardLeftMoveBlock && forwardRightMoveBlock && backwardMoveBlock && backwardLeftMoveBlock && backwardRightMoveBlock && leftMoveBlock && rightMoveBlock)
        {
            EndTurnModify();
        }
        
        if (transform.position.z < targetTransform.position.z && transform.position.x == targetTransform.position.x)
        {
            //Move Forward
            
            if (!forwardMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!forwardRightMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!backwardLeftMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!backwardMoveBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
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
        else if (transform.position.z < targetTransform.position.z && transform.position.x < targetTransform.position.x)
        {
            if (!forwardRightMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!rightMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!leftMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                        }
                        else if (!backwardMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardLeftMoveBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
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
        else if (transform.position.z < targetTransform.position.z && transform.position.x > targetTransform.position.x)
        {
            if (!forwardLeftMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!leftMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!rightMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                        }
                        else if (!backwardMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardRightMoveBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
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

        if (transform.position.z > targetTransform.position.z && transform.position.x == targetTransform.position.x)
        {
            //Move Backward
            if (!backwardMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
            }
            else
            {
                if (!backwardLeftMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else if (!backwardRightMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!forwardRightMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else
                        {
                            if (!forwardMoveBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
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
        else if (transform.position.z > targetTransform.position.z && transform.position.x < targetTransform.position.x)
        {
            if (!backwardRightMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
            }
            else
            {
                if (!rightMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                }
                else if (!backwardMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!leftMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                        }
                        else
                        {
                            if (!forwardLeftMoveBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
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
        else if (transform.position.z > targetTransform.position.z && transform.position.x > targetTransform.position.x)
        {
            if (!backwardLeftMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
            }
            else
            {
                if (!leftMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                }
                else if (!backwardMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!rightMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                        }
                        else
                        {
                                if (!forwardRightMoveBlock)
                                {
                                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
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
        
        if (transform.position.x > targetTransform.position.x && transform.position.z == targetTransform.position.z)
        {
            //Move left
            if (!leftMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!backwardLeftMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardRightMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!rightMoveBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
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
        
        if (transform.position.x < targetTransform.position.x && transform.position.z == targetTransform.position.z)
        {
            //Move right
            if (!rightMoveBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
            }
            else
            {
                if (!forwardRightMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else if (!backwardRightMoveBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!backwardLeftMoveBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else
                        {
                            if (!leftMoveBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
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
    
    private void CheckMoveHandle()
    {
        //Forward Check
        forwardMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(0,0,1), 1,moveBlockLayer);
        forwardLeftMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(-1,0,1), 1,moveBlockLayer);
        forwardRightMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(1, 0, 1), 1,moveBlockLayer);
       
        //Backward Check
        backwardMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(0,0,-1), 1f, moveBlockLayer);
        backwardLeftMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(-1, 0, -1), 1, moveBlockLayer);
        backwardRightMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(1, 0, -1), 1, moveBlockLayer);
       
        //Left & Right
        leftMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(-1,0,0), 1.1f, moveBlockLayer);
        rightMoveBlock = Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z), new Vector3(1,0,0), 1.1f, moveBlockLayer);
    }
    
    #endregion

    #region Enemy Combat

    private void EnemyCombatHandle()
    {
        foreach (Transform combatCheck in combatChecker)
        {
            Ray ray = new Ray(combatCheck.position,Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin,Vector3.down,out hit,gridLayer))
            {
                if (hit.collider.GetComponent<GridMover>() != null)
                {
                    GridMover gridMover = hit.collider.GetComponent<GridMover>();
                    if (gridMover.gridState == GridState.OnPlayer)
                    {
                        gridMover.isAlert = true;
                        playerInRange = true;
                        break;
                    }
                    else
                    {
                        gridMover.isAlert = false;
                        playerInRange = false;
                    }
                }
            }
        }
        
    }
    
    #endregion

    protected override void EndTurnModify()
    {
        base.EndTurnModify();
        playerInRange = false;
    }
}
