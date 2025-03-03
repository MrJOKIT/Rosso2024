using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using VInspector;

public enum BomberState
{
    OnHolding,
    OnBombing,
}
public class EnemyBomber : Enemy
{
    [Tab("Bomber Setting")] 
    public GameObject currentBomber;

    [Space(10)] 
    public Transform bombPos;
    public GameObject bomberArea;
    public BomberState bomberState;
    [Space(10)]
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
        //ChangeGridMoverUnder();
        //CheckMoveHandle();
        ;EnemyCombatHandle();
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
                    if (bomberState == BomberState.OnHolding)
                    {
                        if (playerInRange)
                        {
                            onTurn = false; 
                            bomberState = BomberState.OnBombing;
                            currentBomber = Instantiate(bomberArea, bombPos);
                            onImmortalObject = true;
                            EndTurn();
                        }
                        else
                        {
                            NormalMoveToPlayer();
                        }
                    }
                    else
                    {
                        GetComponent<PlayableDirector>().Play();
                        //EndTurn();
                    }
                    
                } 
                
                break;
            case MovementState.Moving:
                // if (enemyMovementGrid.forwardBlock 
                //     && enemyMovementGrid.backwardBlock 
                //     && enemyMovementGrid.leftBlock 
                //     && enemyMovementGrid.rightBlock 
                //     && enemyMovementGrid.forwardLeftBlock 
                //     && enemyMovementGrid.forwardRightBlock 
                //     && enemyMovementGrid.backwardLeftBlock 
                //     && enemyMovementGrid.backwardRightBlock)
                // {
                //     EndTurnModify();
                // }
                if (enemyMovementGrid.CheckIsMoving())
                {
                    EndTurnModify();
                }
                break;
                
        }
        
    }
    
    
    #region Enemy Move
    
    private void NormalMoveToPlayer()
    {
        // if (enemyMovementGrid.forwardBlock && enemyMovementGrid.backwardBlock && enemyMovementGrid.leftBlock && enemyMovementGrid.rightBlock && enemyMovementGrid.forwardLeftBlock && enemyMovementGrid.forwardRightBlock && enemyMovementGrid.backwardLeftBlock && enemyMovementGrid.backwardRightBlock)
        // {
        //     EndTurnModify();
        // }
        if (enemyMovementGrid.CheckIsMoving())
        {
            EndTurnModify();
        }
        
        if (transform.position.z < targetTransform.position.z && transform.position.x == targetTransform.position.x)
        {
            //Move Forward
            
            if (!enemyMovementGrid.forwardBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
            }
            else
            {
                if (!enemyMovementGrid.forwardLeftBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!enemyMovementGrid.forwardRightBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else
                {
                    if (!enemyMovementGrid.leftBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!enemyMovementGrid.rightBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!enemyMovementGrid.backwardLeftBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else if (!enemyMovementGrid.backwardRightBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!enemyMovementGrid.backwardBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                            }
                            else
                            {
                                EndTurnModify();
                            }
                        }
                    }
                }
            }
            
        }
        else if (transform.position.z < targetTransform.position.z && transform.position.x < targetTransform.position.x)
        {
            if (!enemyMovementGrid.forwardRightBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
            }
            else
            {
                if (!enemyMovementGrid.forwardBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!enemyMovementGrid.rightBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                }
                else
                {
                    if (!enemyMovementGrid.forwardLeftBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!enemyMovementGrid.backwardRightBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!enemyMovementGrid.leftBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                        }
                        else if (!enemyMovementGrid.backwardBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!enemyMovementGrid.backwardLeftBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                            }
                            else
                            {
                                EndTurnModify();
                            }
                        }
                    }
                }
            }
        }
        else if (transform.position.z < targetTransform.position.z && transform.position.x > targetTransform.position.x)
        {
            if (!enemyMovementGrid.forwardLeftBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
            }
            else
            {
                if (!enemyMovementGrid.forwardBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!enemyMovementGrid.leftBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                }
                else
                {
                    if (!enemyMovementGrid.forwardRightBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!enemyMovementGrid.backwardLeftBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!enemyMovementGrid.rightBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                        }
                        else if (!enemyMovementGrid.backwardBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!enemyMovementGrid.backwardRightBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                            }
                            else
                            {
                                EndTurnModify();
                            }
                        }
                    }
                }
            }
        }

        if (transform.position.z > targetTransform.position.z && transform.position.x == targetTransform.position.x)
        {
            //Move Backward
            if (!enemyMovementGrid.backwardBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
            }
            else
            {
                if (!enemyMovementGrid.backwardLeftBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else if (!enemyMovementGrid.backwardRightBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!enemyMovementGrid.leftBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!enemyMovementGrid.rightBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!enemyMovementGrid.forwardLeftBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!enemyMovementGrid.forwardRightBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else
                        {
                            if (!enemyMovementGrid.forwardBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                            }
                            else
                            {
                                EndTurnModify();
                            }
                        }
                    }
                }
            }
            
            
        }
        else if (transform.position.z > targetTransform.position.z && transform.position.x < targetTransform.position.x)
        {
            if (!enemyMovementGrid.backwardRightBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
            }
            else
            {
                if (!enemyMovementGrid.rightBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                }
                else if (!enemyMovementGrid.backwardBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!enemyMovementGrid.forwardRightBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!enemyMovementGrid.backwardLeftBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!enemyMovementGrid.forwardBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!enemyMovementGrid.leftBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                        }
                        else
                        {
                            if (!enemyMovementGrid.forwardLeftBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                            }
                            else
                            {
                                EndTurnModify();
                            }
                        }
                    }
                }
            }
        }
        else if (transform.position.z > targetTransform.position.z && transform.position.x > targetTransform.position.x)
        {
            if (!enemyMovementGrid.backwardLeftBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
            }
            else
            {
                if (!enemyMovementGrid.leftBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                }
                else if (!enemyMovementGrid.backwardBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!enemyMovementGrid.forwardLeftBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!enemyMovementGrid.backwardRightBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!enemyMovementGrid.forwardBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!enemyMovementGrid.rightBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                        }
                        else
                        {
                                if (!enemyMovementGrid.forwardRightBlock)
                                {
                                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                                }
                                else
                                {
                                    EndTurnModify();
                                }
                        }
                    }
                }
            }
        }
        
        if (transform.position.x > targetTransform.position.x && transform.position.z == targetTransform.position.z)
        {
            //Move left
            if (!enemyMovementGrid.leftBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
            }
            else
            {
                if (!enemyMovementGrid.forwardLeftBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!enemyMovementGrid.backwardLeftBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else
                {
                    if (!enemyMovementGrid.forwardBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!enemyMovementGrid.backwardBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!enemyMovementGrid.forwardRightBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else if (!enemyMovementGrid.backwardRightBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!enemyMovementGrid.rightBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
                            }
                            else
                            {
                                EndTurnModify();
                            }
                        }
                    }
                }
            }
        }
        
        if (transform.position.x < targetTransform.position.x && transform.position.z == targetTransform.position.z)
        {
            //Move right
            if (!enemyMovementGrid.rightBlock)
            {
                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Right);
            }
            else
            {
                if (!enemyMovementGrid.forwardRightBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else if (!enemyMovementGrid.backwardRightBlock)
                {
                    enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!enemyMovementGrid.forwardBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!enemyMovementGrid.backwardBlock)
                    {
                        enemyMovementGrid.MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!enemyMovementGrid.forwardLeftBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!enemyMovementGrid.backwardLeftBlock)
                        {
                            enemyMovementGrid.MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else
                        {
                            if (!enemyMovementGrid.leftBlock)
                            {
                                enemyMovementGrid.MoveDirection(EnemyMoveDirection.Left);
                            }
                            else
                            {
                                EndTurnModify();
                            }
                        } 
                    }
                }
            }
        }
    }

    #endregion
    

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

    public void Bomb()
    {
        TurnManager.Instance.AddLog(enemyData.enemyName,"",LogList.Bomb,false);
        currentBomber.GetComponent<SkillAction>().ActiveSkill();
        EnemyDie();
    }
   
}
