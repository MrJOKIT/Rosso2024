using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;
using VInspector;
using Random = UnityEngine.Random;

public enum EnemyMoveDirection
{
    Forward,
    Backward,
    Left,
    Right,
    ForwardLeft,
    ForwardRight,
    BackwardLeft,
    BackwardRight,
}
public class EnemyMovementGrid : MonoBehaviour
{  [Header("Move Setting")]
   public MovementState currentState;
   public float moveSpeed = 5;
   public float knockBackSpeed = 100f;
   public Vector3 gridSize = new Vector3(1,1,1);

   [Space(10)]
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
   

   private void Update()
   {
       CheckBlockHandle();
       MoveStateHandle();
       if (GetComponent<Enemy>().autoSkip)
       {
           if (GetComponent<Enemy>().onTurn)
           {
               int randomNumber = Random.Range(0, 3);
               switch (randomNumber)
               {
                   case 0:
                       SetTargetPosition(Vector3.forward);
                       break;
                   case 1:
                       SetTargetPosition(Vector3.back);
                       break;
                   case 2:
                       SetTargetPosition(Vector3.left);
                       break;
                   case 3:
                       SetTargetPosition(Vector3.right);
                       break;
               }
           
               GetComponent<Enemy>().EndTurn();
           }
       }
       
   }

   public void KnockBack(Transform playerTrans,int gridDistance)
   {
      //ถอยกี่ช่อง
      isKnockBack = true;
      BackDirectionHandle(playerTrans);
      switch (enemyMoveDirection)
      {
          case EnemyMoveDirection.Forward:
              if (forwardBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z + gridDistance));
              break;
          case EnemyMoveDirection.Left:
              if (leftBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x  - gridDistance,transform.localPosition.y,transform.localPosition.z));
              break;
          case EnemyMoveDirection.Backward:
              if (backwardBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z - gridDistance));
              break;
          case EnemyMoveDirection.Right:
              if (rightBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x + gridDistance,transform.localPosition.y,transform.localPosition.z));
              break;
          case EnemyMoveDirection.ForwardLeft:
              if (forwardLeftBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x - gridDistance,transform.localPosition.y,transform.localPosition.z + gridDistance));
              break;
          case EnemyMoveDirection.ForwardRight:
              if (forwardRightBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x + gridDistance,transform.localPosition.y,transform.localPosition.z + gridDistance));
              break;
          case EnemyMoveDirection.BackwardLeft:
              if (backwardLeftBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x - gridDistance,transform.localPosition.y,transform.localPosition.z - gridDistance));
              break;
          case EnemyMoveDirection.BackwardRight:
              if (backwardRightBlock)
              {
                  return;
              }
              SetTargetPosition(new Vector3(transform.localPosition.x + gridDistance,transform.localPosition.y,transform.localPosition.z - gridDistance));
              break;
      }
   }
   
   private void BackDirectionHandle(Transform playerTransform)
   {
       
       if (playerTransform.position.x < transform.position.x && playerTransform.position.z < transform.position.z)
       {
           enemyMoveDirection = EnemyMoveDirection.ForwardRight;
       }
       else if (playerTransform.position.x < transform.position.x && playerTransform.position.z > transform.position.z)
       {
           enemyMoveDirection = EnemyMoveDirection.BackwardRight;
       }
       else if (playerTransform.position.x > transform.position.x && playerTransform.position.z > transform.position.z)
       {
           enemyMoveDirection = EnemyMoveDirection.BackwardLeft;
       }
       else if (playerTransform.position.x > transform.position.x && playerTransform.position.z < transform.position.z)
       {
           enemyMoveDirection = EnemyMoveDirection.ForwardLeft;
       }
       else if (playerTransform.position.x < transform.position.x)
       {
           // x + 1
           enemyMoveDirection = EnemyMoveDirection.Right;
       }
       else if (playerTransform.position.x > transform.position.x)
       {
           // x - 1
           enemyMoveDirection = EnemyMoveDirection.Left;
       }
       else if (playerTransform.position.z < transform.position.z)
       {
           // z + 1
           enemyMoveDirection = EnemyMoveDirection.Forward;
       }
       else if (playerTransform.position.z > transform.position.z)
       {
           // z - 1
           enemyMoveDirection = EnemyMoveDirection.Backward;
       }
   }

   
   private void CheckBlockHandle()
   {
       //Forward Check
       forwardBlock = Physics.Raycast(transform.position, Vector3.forward, 1,obstacleLayer);
       forwardLeftBlock = Physics.Raycast(transform.position, new Vector3(-1,0,1), 1,obstacleLayer);
       forwardRightBlock = Physics.Raycast(transform.position, new Vector3(1, 0, 1), 1,obstacleLayer);
       
       //Backward Check
       backwardBlock = Physics.Raycast(transform.position, Vector3.back, 1, obstacleLayer);
       backwardLeftBlock = Physics.Raycast(transform.position, new Vector3(-1, 0, -1), 1, obstacleLayer);
       backwardRightBlock = Physics.Raycast(transform.position, new Vector3(1, 0, -1), 1, obstacleLayer);
       
       //Left & Right
       leftBlock = Physics.Raycast(transform.position, Vector3.left, 1, obstacleLayer);
       rightBlock = Physics.Raycast(transform.position, Vector3.right, 1, obstacleLayer);
   }
   
   private void MoveStateHandle()
       {
           switch (currentState)
           {
               case MovementState.Idle:
                   break;
               case MovementState.Moving:
                   MoveToTarget();
                   break;
           }
       }
       private void SetTargetPosition(Vector3 direction)
       {
           Vector3 nextPosition;
   
           if (currentState == MovementState.Idle || direction != targetPosition - transform.localPosition)
           {
               if (direction == Vector3.forward || direction == Vector3.back || direction == Vector3.left || direction == Vector3.right)
               {
                   nextPosition = transform.localPosition + Vector3.Scale(direction, gridSize);
               }
               else
               {
                   // Handling grid snapping for mouse click input
                   float gridX = Mathf.Round(direction.x / gridSize.x) * gridSize.x;
                   float gridZ = Mathf.Round(direction.z / gridSize.z) * gridSize.z;
                   nextPosition = new Vector3(gridX, transform.localPosition.y, gridZ);
               }
   
               targetPosition = nextPosition;
               currentState = MovementState.Moving;
           }
       }
       
   
       private void MoveToTarget()
       {
           if (isKnockBack)
           {
               transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, knockBackSpeed * Time.deltaTime);
           }
           else
           {
               transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
           }
           

           if (transform.localPosition == targetPosition)
           {
               if (isKnockBack)
               {
                   currentState = MovementState.Idle;
                   isKnockBack = false;
               } 
               else
               {
                   GridSpawnManager.Instance.ClearMover();
                   GetComponent<Enemy>().EndTurn();
                   currentState = MovementState.Idle;
               }
                  
           }
           
       }

       [Button("Move")]
       public void MoveDirection(EnemyMoveDirection direction)
       {
           switch (direction)
           { 
               case EnemyMoveDirection.Forward: 
                   SetTargetPosition(new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z + 1));
                  break;
               case EnemyMoveDirection.Left:
                   SetTargetPosition(new Vector3(transform.localPosition.x  - 1,transform.localPosition.y,transform.localPosition.z));
                  break;
               case EnemyMoveDirection.Backward:
                  SetTargetPosition(new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z - 1));
                  break;
               case EnemyMoveDirection.Right:
                  SetTargetPosition(new Vector3(transform.localPosition.x + 1,transform.localPosition.y,transform.localPosition.z));
                  break;
               case EnemyMoveDirection.ForwardLeft:
                  SetTargetPosition(new Vector3(transform.localPosition.x - 1,transform.localPosition.y,transform.localPosition.z + 1));
                  break;
               case EnemyMoveDirection.ForwardRight:
                  SetTargetPosition(new Vector3(transform.localPosition.x + 1,transform.localPosition.y,transform.localPosition.z + 1));
                  break;
               case EnemyMoveDirection.BackwardLeft:
                  SetTargetPosition(new Vector3(transform.localPosition.x - 1,transform.localPosition.y,transform.localPosition.z - 1));
                  break;
               case EnemyMoveDirection.BackwardRight:
                  SetTargetPosition(new Vector3(transform.localPosition.x + 1,transform.localPosition.y,transform.localPosition.z - 1));
                  break;
               }
       }
       private void OnDrawGizmos()
       {
           //Forward Check
           Debug.DrawRay(transform.position,Vector3.forward,Color.green);
           Debug.DrawRay(transform.position,new Vector3(-1,0,1) * 1,Color.green);
           Debug.DrawRay(transform.position,new Vector3(1,0,1) * 1,Color.green);
           
           //Backward Check
           Debug.DrawRay(transform.position,Vector3.back,Color.green);
           Debug.DrawRay(transform.position,new Vector3(-1,0,-1) * 1,Color.green);
           Debug.DrawRay(transform.position,new Vector3(1,0,-1) * 1,Color.green);
           
           //Left & Right
           Debug.DrawRay(transform.position,Vector3.left,Color.green);
           Debug.DrawRay(transform.position,Vector3.right,Color.green);
       }
}
