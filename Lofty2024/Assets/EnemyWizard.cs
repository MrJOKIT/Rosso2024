using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using VInspector;

public enum SummonState
{
    OnPrepareSummon,
    OnSummon,
}
public class EnemyWizard : Enemy
{
    [Tab("Summon Setting")] 
    public GameObject summonEnemyPrefab;
    public int summonCount;
    [Header("Cooldown")]
    public int summonCooldown;
    private int summonCooldownCounter;
    [Space(10)]
    private EnemyMovementGrid _enemyMovementGrid;

    [Space(10)] 
    public SummonState summonState; 

    private void Start()
    {
        _enemyMovementGrid = GetComponent<EnemyMovementGrid>();
        summonCooldownCounter = summonCooldown - 1;
    }

    private void Update()
    {
        //ChangeGridMoverUnder();
        //CheckMoveHandle();
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
                    if (summonState == SummonState.OnPrepareSummon)
                    {
                        if (summonCooldownCounter >= summonCooldown)
                        {
                            summonState = SummonState.OnSummon;
                            GetComponent<PlayableDirector>().Play();
                        }
                        else
                        {
                            summonCooldownCounter += 1;
                            NormalMoveToPlayer();
                        }
                    }
                    else
                    {
                        EnemySummon();
                    }
                    
                    
                } 
                
                break;
            case MovementState.Moving:
                if (_enemyMovementGrid.forwardBlock && _enemyMovementGrid.backwardBlock && _enemyMovementGrid.leftBlock && _enemyMovementGrid.rightBlock && _enemyMovementGrid.forwardLeftBlock && _enemyMovementGrid.forwardRightBlock && _enemyMovementGrid.backwardLeftBlock && _enemyMovementGrid.backwardRightBlock)
                {
                    EndTurnModify();
                }
                break;
                
        }
        
    }
    
    
    #region Enemy Move
    
    private void NormalMoveToPlayer()
    {
        if (_enemyMovementGrid.forwardBlock && _enemyMovementGrid.backwardBlock && _enemyMovementGrid.leftBlock && _enemyMovementGrid.rightBlock && _enemyMovementGrid.forwardLeftBlock && _enemyMovementGrid.forwardRightBlock && _enemyMovementGrid.backwardLeftBlock && _enemyMovementGrid.backwardRightBlock)
        {
            EndTurnModify();
        }
        
        if (transform.position.z > targetTransform.position.z && transform.position.x == targetTransform.position.x)
        {
            //Move Forward
            
            if (!_enemyMovementGrid.forwardBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
            }
            else
            {
                if (!_enemyMovementGrid.forwardLeftBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!_enemyMovementGrid.forwardRightBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else
                {
                    if (!_enemyMovementGrid.leftBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!_enemyMovementGrid.rightBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.backwardLeftBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else if (!_enemyMovementGrid.backwardRightBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!_enemyMovementGrid.backwardBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
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
            if (!_enemyMovementGrid.forwardRightBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
            }
            else
            {
                if (!_enemyMovementGrid.forwardBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!_enemyMovementGrid.rightBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                }
                else
                {
                    if (!_enemyMovementGrid.forwardLeftBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!_enemyMovementGrid.backwardRightBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.leftBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                        }
                        else if (!_enemyMovementGrid.backwardBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!_enemyMovementGrid.backwardLeftBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
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
            if (!_enemyMovementGrid.forwardLeftBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
            }
            else
            {
                if (!_enemyMovementGrid.forwardBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                }
                else if (!_enemyMovementGrid.leftBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                }
                else
                {
                    if (!_enemyMovementGrid.forwardRightBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!_enemyMovementGrid.backwardLeftBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.rightBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                        }
                        else if (!_enemyMovementGrid.backwardBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                        }
                        else
                        {
                            if (!_enemyMovementGrid.backwardRightBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
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

        if (transform.position.z < targetTransform.position.z && transform.position.x == targetTransform.position.x)
        {
            //Move Backward
            if (!_enemyMovementGrid.backwardBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
            }
            else
            {
                if (!_enemyMovementGrid.backwardLeftBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else if (!_enemyMovementGrid.backwardRightBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!_enemyMovementGrid.leftBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                    }
                    else if (!_enemyMovementGrid.rightBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.forwardLeftBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!_enemyMovementGrid.forwardRightBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else
                        {
                            if (!_enemyMovementGrid.forwardBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
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
            if (!_enemyMovementGrid.backwardRightBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
            }
            else
            {
                if (!_enemyMovementGrid.rightBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                }
                else if (!_enemyMovementGrid.backwardBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!_enemyMovementGrid.forwardRightBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                    }
                    else if (!_enemyMovementGrid.backwardLeftBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.forwardBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!_enemyMovementGrid.leftBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                        }
                        else
                        {
                            if (!_enemyMovementGrid.forwardLeftBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
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
            if (!_enemyMovementGrid.backwardLeftBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
            }
            else
            {
                if (!_enemyMovementGrid.leftBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                }
                else if (!_enemyMovementGrid.backwardBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                }
                else
                {
                    if (!_enemyMovementGrid.forwardLeftBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                    }
                    else if (!_enemyMovementGrid.backwardRightBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.forwardBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                        }
                        else if (!_enemyMovementGrid.rightBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                        }
                        else
                        {
                                if (!_enemyMovementGrid.forwardRightBlock)
                                {
                                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
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
            //Move left
            if (!_enemyMovementGrid.leftBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
            }
            else
            {
                if (!_enemyMovementGrid.forwardLeftBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                }
                else if (!_enemyMovementGrid.backwardLeftBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                }
                else
                {
                    if (!_enemyMovementGrid.forwardBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!_enemyMovementGrid.backwardBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.forwardRightBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                        }
                        else if (!_enemyMovementGrid.backwardRightBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!_enemyMovementGrid.rightBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
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
            //Move right
            if (!_enemyMovementGrid.rightBlock)
            {
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
            }
            else
            {
                if (!_enemyMovementGrid.forwardRightBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                }
                else if (!_enemyMovementGrid.backwardRightBlock)
                {
                    GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                }
                else
                {
                    if (!_enemyMovementGrid.forwardBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                    }
                    else if (!_enemyMovementGrid.backwardBlock)
                    {
                        GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                    }
                    else
                    {
                        if (!_enemyMovementGrid.forwardLeftBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                        }
                        else if (!_enemyMovementGrid.backwardLeftBlock)
                        {
                            GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                        }
                        else
                        {
                            if (!_enemyMovementGrid.leftBlock)
                            {
                                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
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
    

    private void EnemySummon()
    {
        for (int a = 0; a < summonCount; a++)
        {
            GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().AddNewEnemyInRoom(summonEnemyPrefab);
        }
        summonCooldownCounter = 0;
        summonState = SummonState.OnPrepareSummon;
        enemyAnimator.SetBool("OnSummon",false);
        EndTurn();
    }
}
