using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
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
    private EnemyMovementGrid _enemyMovementGrid;
    [Space(10)]
    public bool playerInRange;
    public LayerMask gridLayer;
    public List<Transform> combatChecker;

    private void Start()
    {
        _enemyMovementGrid = GetComponent<EnemyMovementGrid>();
    }

    private void Update()
    {
        //ChangeGridMoverUnder();
        //CheckMoveHandle();
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
                    if (bomberState == BomberState.OnHolding)
                    {
                        if (playerInRange)
                        {
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
                        currentBomber.GetComponent<SkillAction>().ActiveSkill();
                        EnemyDie();
                        //EndTurn();
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

    #endregion
    

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
}
