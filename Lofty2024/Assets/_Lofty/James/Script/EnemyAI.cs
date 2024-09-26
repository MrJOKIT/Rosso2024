using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public class EnemyAI : Enemy
{
    [Header("Move Checker")]
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

    public bool playerClose;
    private void FixedUpdate()
    {
        CheckMoveHandle();
        if (onTurn == false)
        {
            return;
        }
        EnemyMoveToPlayer();
        if (playerClose)
        {
            //Combat time
        }
        else
        {
            EndTurn();
        }
    }

    private void EnemyMoveToPlayer()
    {
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
}
