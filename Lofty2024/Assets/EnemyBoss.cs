using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public enum CombatPhase
{
    NormalPhase,
    JumpPhase,
    SummonPhase,
}
public class EnemyBoss : Enemy
{
    [Tab("Boss Setting")]
    public CombatPhase currentPhase;
    private EnemyMovementGrid _enemyMovementGrid;
    public int phaseNumber;
    public bool firstBattle;
    [Tab("Jump Setting")] 
    public GameObject alertMark;
    public GameObject currentAlert;
    [Space(10)] 
    public BossRadiusChecker _bossRadiusChecker;
    [Space(10)]
    public bool jumpPrepare;

    [Tab("Summon Setting")] 
    public GameObject monsterPrefab;
    public int maxSummon;

    private void Start()
    {
        _enemyMovementGrid = GetComponent<EnemyMovementGrid>();
    }

    private void Update()
    {
        //ChangeGridMoverUnder();
        //CheckMoveHandle();
        if (onTurn == false)
        {
            return;
        }
        //EnemyCombatHandle();

        if (firstBattle == false)
        {
            phaseNumber = Random.Range(0, 6);
            firstBattle = true;
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
                    if (_bossRadiusChecker.playerOnRadius)
                    {
                        currentPhase = CombatPhase.JumpPhase;
                        EnemyMoveToPlayer();
                    }
                    else
                    {
                        if (phaseNumber < 3)
                        {
                            currentPhase = CombatPhase.NormalPhase;
                            EnemyMoveToPlayer();
                        }
                        else if (phaseNumber <= 6)
                        {
                            currentPhase = CombatPhase.SummonPhase;
                            EnemyMoveToPlayer();
                            GetComponent<EnemyMovementGrid>().currentState = MovementState.Moving;
                        }
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

    private void EnemyMoveToPlayer()
    {
        
        switch (currentPhase)
        {
            case CombatPhase.NormalPhase:
                NormalMoveToPlayer();
                EndTurn();
                break;
            case CombatPhase.JumpPhase:
                JumpMoveToPlayer();
                break;
            case CombatPhase.SummonPhase:
                SummonToPlayer();
                break;
        }

        phaseNumber += 1;
        if (phaseNumber > 6)
        {
            phaseNumber = 0;
        }
    }
    
    #region Enemy Move
    
    private void NormalMoveToPlayer()
    {
        if (_enemyMovementGrid.forwardBlock && _enemyMovementGrid.backwardBlock && _enemyMovementGrid.leftBlock && _enemyMovementGrid.rightBlock && _enemyMovementGrid.forwardLeftBlock && _enemyMovementGrid.forwardRightBlock && _enemyMovementGrid.backwardLeftBlock && _enemyMovementGrid.backwardRightBlock)
        {
            EndTurnModify();
        }
        
        if (transform.position.z < targetTransform.position.z && transform.position.x == targetTransform.position.x)
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
        else if (transform.position.z < targetTransform.position.z && transform.position.x < targetTransform.position.x)
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
        else if (transform.position.z < targetTransform.position.z && transform.position.x > targetTransform.position.x)
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

        if (transform.position.z > targetTransform.position.z && transform.position.x == targetTransform.position.x)
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
        else if (transform.position.z > targetTransform.position.z && transform.position.x < targetTransform.position.x)
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
        else if (transform.position.z > targetTransform.position.z && transform.position.x > targetTransform.position.x)
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
        
        if (transform.position.x > targetTransform.position.x && transform.position.z == targetTransform.position.z)
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
        
        if (transform.position.x < targetTransform.position.x && transform.position.z == targetTransform.position.z)
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

    private void JumpMoveToPlayer()
    {
        if (jumpPrepare == false)
        {
            currentAlert = Instantiate(alertMark, new Vector3(targetTransform.position.x,alertMark.transform.position.y,targetTransform.position.z),alertMark.transform.rotation);
            jumpPrepare = true;
        }
        else
        {
            //Jump
            Debug.Log("Jump");
            jumpPrepare = false;
        }
        EndTurn();
    }

    [ContextMenu("Test Summon")]
    private void SummonToPlayer()
    {
        if (TurnManager.Instance.turnData.Count >= maxSummon)
        {
            EndTurn();
            return;
        }
        GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().AddNewEnemyInRoom(monsterPrefab);
        EndTurn();
    }

    #endregion
    
}
