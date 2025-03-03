using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Playables;
using VInspector;

public class EnemyIron : Enemy
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
    public LayerMask enemyLayer;
    public bool enemyForward;
    public bool enemyForwardLeft;
    public bool enemyForwardRight;
    public bool enemyBackward;
    public bool enemyBackwardLeft;
    public bool enemyBackwardRight;
    public bool enemyLeft;
    public bool enemyRight;
    private void Update()
    {
        ChangeGridMoverUnder();
        CheckMoveHandle();
        EnemyCombatHandle();
        if (onTurn == false)
        {
            return;
        }
        
        switch (enemyMovementGrid.currentState)
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
                        onTurn = false;
                        //enemyAnimator.SetTrigger("Attack");
                        GetComponent<PlayableDirector>().Play();
                        //EndTurn();
                    }
                    else
                    {
                        EnemyMoveToPlayer();
                    }
                }
                
                break;
            case MovementState.Moving:
                // if (forwardMoveBlock && forwardLeftMoveBlock && forwardRightMoveBlock && backwardMoveBlock && backwardLeftMoveBlock && backwardRightMoveBlock && leftMoveBlock && rightMoveBlock)
                // {
                //     print("Manogo");
                //     EndTurnModify();
                // }
                if (enemyMovementGrid.CheckIsMoving())
                {
                    print("end turn modify");
                
                    EndTurnModify();
                }
                // if (forwardMoveBlock && backwardMoveBlock && leftMoveBlock && rightMoveBlock && forwardLeftMoveBlock && forwardRightMoveBlock && backwardLeftMoveBlock && backwardRightMoveBlock)
                // {
                //     print("Manogo");
                //     EndTurnModify();
                // }
                break;
                
        }
    }

    private void ChangeGridMoverUnder()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit _hit;
        if (Physics.Raycast(ray.origin,Vector3.down,out _hit,10,gridLayer))
        {
            var _gridMover = _hit.collider.GetComponent<GridMover>();
            
            if (_gridMover == null) return;
            if (_gridMover.gridState == GridState.OnEnemy) return;
            _gridMover.enemy = this; 
            _gridMover.gridState = GridState.OnEnemy;
        }
    }


    #region Enemy Move
    
    private void EnemyMoveToPlayer()
    {
        // if (forwardMoveBlock && forwardLeftMoveBlock && forwardRightMoveBlock && backwardMoveBlock && backwardLeftMoveBlock && backwardRightMoveBlock && leftMoveBlock && rightMoveBlock)
        // {
        //     print("Manogo");
        //     EndTurnModify();
        // }
        // if (enemyMovementGrid.CheckIsMoving())
        // {
        //     EndTurnModify();
        // }
        
        if (enemyMovementGrid.CheckIsMoving())
        {
            print("end turn modify");
            
            EndTurnModify();
        }
        
        if (transform.position.z < targetTransform.position.z && transform.position.x == targetTransform.position.x)
        {
            //Move Forward
            
            if (!forwardMoveBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!forwardRightMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!backwardLeftMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!backwardMoveBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
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
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!rightMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!leftMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                        }
                        else if (!backwardMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardLeftMoveBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
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
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!leftMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!rightMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                        }
                        else if (!backwardMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardRightMoveBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
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
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
            }
            else
            {
                if (!backwardLeftMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else if (!backwardRightMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!forwardRightMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else
                        {
                            if (!forwardMoveBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
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
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
            }
            else
            {
                if (!rightMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                }
                else if (!backwardMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!leftMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                        }
                        else
                        {
                            if (!forwardLeftMoveBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
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
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
            }
            else
            {
                if (!leftMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                }
                else if (!backwardMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!rightMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                        }
                        else
                        {
                                if (!forwardRightMoveBlock)
                                {
                                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
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
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!backwardLeftMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardRightMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!rightMoveBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
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
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
            }
            else
            {
                if (!forwardRightMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else if (!backwardRightMoveBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!backwardLeftMoveBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else
                        {
                            if (!leftMoveBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
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
        //Forward Check
        enemyForward = Physics.Raycast(transform.position, Vector3.forward, 1,enemyLayer);
        enemyForwardLeft = Physics.Raycast(transform.position, new Vector3(-1,0,1), 1,enemyLayer);
        enemyForwardRight = Physics.Raycast(transform.position, new Vector3(1, 0, 1), 1,enemyLayer);
       
        //Backward Check
        enemyBackward = Physics.Raycast(transform.position, Vector3.back, 1, enemyLayer);
        enemyBackwardLeft = Physics.Raycast(transform.position, new Vector3(-1, 0, -1), 1, enemyLayer);
        enemyBackwardRight = Physics.Raycast(transform.position, new Vector3(1, 0, -1), 1, enemyLayer);
       
        //Left & Right
        enemyLeft = Physics.Raycast(transform.position, Vector3.left, 1, enemyLayer);
        enemyRight = Physics.Raycast(transform.position, Vector3.right, 1, enemyLayer);


        if (enemyForward || enemyForwardLeft || enemyForwardRight || enemyBackward || enemyBackwardLeft || enemyBackwardRight || enemyLeft || enemyRight)
        {
            playerInRange = true;
        }
        else if (!enemyForward && !enemyForwardLeft && !enemyForwardRight && !enemyBackward && !enemyBackwardLeft && !enemyBackwardRight && !enemyLeft && !enemyRight)
        {
            playerInRange = false;
        }
        
    }
    
    
    #endregion

    protected override void EndTurnModify()
    {
        base.EndTurnModify();
        playerInRange = false;
    }
}
