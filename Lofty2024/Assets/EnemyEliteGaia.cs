using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Playables;
using VInspector;

public enum CombatPhase
{
    NormalPhase,
    JumpPhase,
    SummonPhase,
}
public class EnemyEliteGaia : Enemy
{
    [Tab("Elite Gaia Setting")] 
    public Animator shadowAnimator;
    public Vector3 jumpPosition;
    public GameObject alertMark;
    public GameObject currentAlert;
    public bool jumpPrepare;
    [Tab("Boss Setting")]
    public CombatPhase currentPhase;
    public float jumpDistanceCheck = 4f;
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
        
        if (onTurn == false)
        {
            return;
        }
       
        EnemyCombatHandle();
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
                        onTurn = false;
                        //enemyAnimator.SetTrigger("Attack");
                        GetComponent<PlayableDirector>().Play();
                        //EndTurn();
                    }
                    else
                    {
                        float playerDistance = Vector3.Distance(transform.position, targetTransform.position);
                        Debug.Log(playerDistance);
                        if (jumpPrepare)
                        {
                            JumpMoveToPlayer();
                        }
                        else
                        {
                            if (playerDistance < jumpDistanceCheck )
                            {
                                EnemyMoveToPlayer();
                            }
                            else
                            {
                                JumpMoveToPlayer();
                            }
                        }
                        
                        
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

    private void JumpMoveToPlayer()
    {
        if (jumpPrepare == false)
        {
            currentAlert = Instantiate(alertMark, new Vector3(targetTransform.position.x,alertMark.transform.position.y,targetTransform.position.z),alertMark.transform.rotation);
            jumpPosition = targetTransform.position;
            jumpPrepare = true;
        }
        else
        {
            //Jump
            currentAlert.GetComponent<SkillAction>().ActiveSkill();
            currentAlert = null;
            transform.position = new Vector3(jumpPosition.x,transform.position.y,jumpPosition.z);
            Debug.Log("Jump");
            jumpPrepare = false;
        }
        EndTurn();
    }
    
}
