using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NavigationMoveState
{
    Idle,
    Moving,
}
public class PlayerNavigation : MonoBehaviour
{
    public PlayerGridBattle player;

    [Space(10)] [Header("Navigation Setting")]
    public bool navigationSuccess;
    public NavigationMoveState currentState;
    public LayerMask moveBlockLayer;
    [Space(10)]
    public float moveSpeed = 5f;
    public Vector3 gridSize = new Vector3(1f, 1f, 1f);
    [Space(10)]
    public GameObject movePathPrefab;
    public List<GameObject> movePathList;

    [Space(10)] 
    [Header("Check Navigation")]
    public GameObject finalPosPrefab;
    public GameObject arrowPrefab;
    public List<GameObject> moveArrow;
    
    [Space(10)]
    [Header("Check Obstacle")] 
    public bool forwardMoveBlock;
    public bool forwardLeftMoveBlock;
    public bool forwardRightMoveBlock;
    public bool backwardMoveBlock;
    public bool backwardLeftMoveBlock;
    public bool backwardRightMoveBlock;
    public bool leftMoveBlock;
    public bool rightMoveBlock;
    
    [Space(10)]
    [Header("Hide Condition")] 
    private Vector3 targetTransform;
    private Vector3 supTargetTransform;
    private Vector3 lastPlayerTransform;

    private void Awake()
    {
        transform.position = player.transform.position;
        targetTransform = transform.position;
        supTargetTransform = transform.position;
    }

    private void Update()
    {
        if (moveArrow.Count > 100)
        {
            Debug.Log("Error Navigation");   
            ClearMovePath();
            currentState = NavigationMoveState.Idle;
            ResetNavigation();
        }
        if (player.GetPlayerMode == PlayerMode.Combat)
        {
            return;
        }
        MoveStateHandle();
        MoveChecker();
    }

    private void MoveStateHandle()
    {
        switch (currentState)
        {
            case NavigationMoveState.Idle:
                break;
            case NavigationMoveState.Moving:
                MoveToTarget();
                break;
        }
    }
    
    private void MoveChecker() 
    {
        //Forward Check
        forwardMoveBlock = Physics.Raycast(transform.position, Vector3.forward, 1,moveBlockLayer);
        forwardLeftMoveBlock = Physics.Raycast(transform.position, new Vector3(-1,0,1), 1,moveBlockLayer);
        forwardRightMoveBlock = Physics.Raycast(transform.position, new Vector3(1, 0, 1), 1,moveBlockLayer);
       
        //Backward Check
        backwardMoveBlock = Physics.Raycast(transform.position, Vector3.back, 1, moveBlockLayer);
        backwardLeftMoveBlock = Physics.Raycast(transform.position, new Vector3(-1, 0, -1), 1, moveBlockLayer);
        backwardRightMoveBlock = Physics.Raycast(transform.position, new Vector3(1, 0, -1), 1, moveBlockLayer);
       
        //Left & Right
        leftMoveBlock = Physics.Raycast(transform.position, Vector3.left, 1, moveBlockLayer);
        rightMoveBlock = Physics.Raycast(transform.position, Vector3.right, 1, moveBlockLayer);
    }
    
     private void AddMovePath(Vector3 spawnPosition,PlayerMoveDirection direction)
    {
        GameObject movePathManager = Instantiate(movePathPrefab,spawnPosition,Quaternion.identity);
        movePathManager.GetComponent<MovePathManager>().SetPath(direction);
        movePathList.Add(movePathManager);
    }
    private void ClearMovePath()
    {
        foreach (GameObject movePaths in movePathList.ToList())
        {
            Destroy(movePaths);
            movePathList.Remove(movePaths);
        }
    }

    private void SetTargetPosition(Vector3 direction)
    {
        Vector3 nextPosition;

        if (currentState == NavigationMoveState.Idle || direction != targetTransform - transform.position)
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

            targetTransform = nextPosition;
            currentState = NavigationMoveState.Moving;
        }
    }

    private void MoveToTarget()
    {
        if (transform.position != supTargetTransform )
        {
            transform.position =
                Vector3.MoveTowards(transform.position, supTargetTransform, moveSpeed * Time.deltaTime);
            if (forwardMoveBlock && forwardLeftMoveBlock && forwardRightMoveBlock && backwardMoveBlock && backwardLeftMoveBlock && backwardRightMoveBlock && leftMoveBlock && rightMoveBlock)
            {
                ClearMovePath();
            }
        }
        else
        {
            MoveHandle();
        }
        
        if (transform.position == targetTransform)
        {
            GameObject finalArrow = Instantiate(finalPosPrefab, new Vector3(targetTransform.x,0.02f,targetTransform.z), Quaternion.identity);
            moveArrow.Add(finalArrow);
            
            ClearMovePath();
            currentState = NavigationMoveState.Idle;
            ResetNavigation();
        }
    }
    
    public void SetPlayerMoveDirection(PlayerMoveDirection direction)
    {
        lastPlayerTransform = transform.position;
        switch (direction)
        {
            case PlayerMoveDirection.Forward:
                supTargetTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.ForwardLeft:
                supTargetTransform = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.ForwardRight:
                supTargetTransform = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.Backward:
                supTargetTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.BackwardLeft:
                supTargetTransform = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.BackwardRight:
                supTargetTransform = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break; 
            case PlayerMoveDirection.Left:
                supTargetTransform = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
            case PlayerMoveDirection.Right:
                supTargetTransform = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                //transform.position = Vector3.MoveTowards(transform.position,supTargetTransform, moveSpeed * Time.deltaTime);
                break;
        }

        SpawnNavigationArrow(direction);
        //AddMovePath(lastPlayerTransform,direction);
    }
    private void MoveHandle()
    {
        if (transform.position.z < targetTransform.z && transform.position.x == targetTransform.x)
        {
            //Move Forward
            
            if (!forwardMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Forward);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                }
                else if (!forwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Right);
                    }
                    else
                    {
                        if (!backwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!backwardMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                                ClearMovePath();
                            }
                        }
                    }
                }
            }
            
        }
        else if (transform.position.z < targetTransform.z && transform.position.x < targetTransform.x)
        {
            if (!forwardRightMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                }
                else if (!rightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Right);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!leftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Left);
                        }
                        else if (!backwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardLeftMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                                ClearMovePath();
                            }
                        }
                    }
                }
            }
        }
        else if (transform.position.z < targetTransform.z && transform.position.x > targetTransform.x)
        {
            if (!forwardLeftMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
            }
            else
            {
                if (!forwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                }
                else if (!leftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Left);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!rightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Right);
                        }
                        else if (!backwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                        }
                        else
                        {
                            if (!backwardRightMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                                ClearMovePath();
                            }
                        }
                    }
                }
            }
        }

        if (transform.position.z > targetTransform.z && transform.position.x == targetTransform.x)
        {
            //Move Backward
            if (!backwardMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Backward);
            }
            else
            {
                if (!backwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                }
                else if (!backwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                }
                else
                {
                    if (!leftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Left);
                    }
                    else if (!rightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Right);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                        }
                        else if (!forwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                        }
                        else
                        {
                            if (!forwardMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                                ClearMovePath();
                            }
                        }
                    }
                }
            }
            
            
        }
        else if (transform.position.z > targetTransform.z && transform.position.x < targetTransform.x)
        {
            if (!backwardRightMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
            }
            else
            {
                if (!rightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Right);
                }
                else if (!backwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                }
                else
                {
                    if (!forwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                    }
                    else if (!backwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                        }
                        else if (!leftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Left);
                        }
                        else
                        {
                            if (!forwardLeftMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                                ClearMovePath();
                            }
                        }
                    }
                }
            }
        }
        else if (transform.position.z > targetTransform.z && transform.position.x > targetTransform.x)
        {
            if (!backwardLeftMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
            }
            else
            {
                if (!leftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Left);
                }
                else if (!backwardMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                }
                else
                {
                    if (!forwardLeftMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                    }
                    else if (!backwardRightMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                    }
                    else
                    {
                        if (!forwardMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                        }
                        else if (!rightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.Right);
                        }
                        else
                        {
                                if (!forwardRightMoveBlock)
                                {
                                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                                }
                                else
                                {
                                    Debug.Log("Can't move");
                                    ClearMovePath();
                                }
                        }
                    }
                }
            }
        }
        
        if (transform.position.x > targetTransform.x && transform.position.z == targetTransform.z)
        {
            //Move left
            if (!leftMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Left);
            }
            else
            {
                if (!forwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                }
                else if (!backwardLeftMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                        }
                        else if (!backwardRightMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                        }
                        else
                        {
                            if (!rightMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Right);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                                ClearMovePath();
                            }
                        }
                    }
                }
            }
        }
        
        if (transform.position.x < targetTransform.x && transform.position.z == targetTransform.z)
        {
            //Move right
            if (!rightMoveBlock)
            {
                SetPlayerMoveDirection(PlayerMoveDirection.Right);
            }
            else
            {
                if (!forwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                }
                else if (!backwardRightMoveBlock)
                {
                    SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
                }
                else
                {
                    if (!forwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                    }
                    else if (!backwardMoveBlock)
                    {
                        SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                    }
                    else
                    {
                        if (!forwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                        }
                        else if (!backwardLeftMoveBlock)
                        {
                            SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                        }
                        else
                        {
                            if (!leftMoveBlock)
                            {
                                SetPlayerMoveDirection(PlayerMoveDirection.Left);
                            }
                            else
                            {
                                Debug.Log("Can't move");
                                ClearMovePath();
                            }
                        }
                    }
                }
            }
        }

        
    }

    private void SpawnNavigationArrow(PlayerMoveDirection direction)
    {
        GameObject arrowNavi = Instantiate(arrowPrefab, new Vector3(lastPlayerTransform.x,0.02f,lastPlayerTransform.z), Quaternion.identity);
        switch (direction) 
        {
            case PlayerMoveDirection.Forward:
                arrowNavi.transform.rotation = Quaternion.Euler(0,0,0);
                break;
            case PlayerMoveDirection.Backward:
                arrowNavi.transform.rotation = Quaternion.Euler(0,180,0);
                break;
            case PlayerMoveDirection.Left:
                arrowNavi.transform.rotation = Quaternion.Euler(0,-90,0);
                break;
            case PlayerMoveDirection.Right:
                arrowNavi.transform.rotation = Quaternion.Euler(0,90,0);
                break;
            case PlayerMoveDirection.ForwardLeft:
                arrowNavi.transform.rotation = Quaternion.Euler(0,-45,0);
                break;
            case PlayerMoveDirection.ForwardRight:
                arrowNavi.transform.rotation = Quaternion.Euler(0,45,0);
                break;
            case PlayerMoveDirection.BackwardLeft:
                arrowNavi.transform.rotation = Quaternion.Euler(0,-135,0);
                break;
            case PlayerMoveDirection.BackwardRight:
                arrowNavi.transform.rotation = Quaternion.Euler(0,135,0);
                break;
        }
        
        moveArrow.Add(arrowNavi);
    }

    public void StartNavigation(Vector3 point)
    {
        transform.position = player.transform.position;
        targetTransform = player.transform.position;
        lastPlayerTransform = player.transform.position;
        supTargetTransform = player.transform.position;
        SetTargetPosition(point);
        navigationSuccess = false;
    }
    private void ResetNavigation()
    {
        transform.position = player.transform.position;
        navigationSuccess = true;
    }

    public void ClearNavigation() 
    {
        foreach (GameObject arrow in moveArrow.ToList())
        {
            if (arrow == null)
            {
                moveArrow.Remove(arrow);
            }
            else
            {
                moveArrow.Remove(arrow);
                Destroy(arrow);
            }
        }
    }
    
}
