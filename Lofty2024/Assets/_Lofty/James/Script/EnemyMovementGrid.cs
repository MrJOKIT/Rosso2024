using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum KnockBackDirection
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
{
   public MovementState currentState;
   public float moveSpeed = 5;

   public bool isKnockBack;
   public KnockBackDirection knockBackDirection;
   
   public Vector3 gridSize = new Vector3(1,1,1);
   private Vector3 targetPosition;

   private void Update()
   {
       MoveStateHandle();
       if (GetComponent<Enemy>().autoSkip)
       {
           return;
       }
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

   public void KnockBack(Transform playerTrans,int gridDistance)
   {
      //ถอยกี่ช่อง
      isKnockBack = true;
      BackDirectionHandle(playerTrans);
      switch (knockBackDirection)
      {
          case KnockBackDirection.Forward:
              SetTargetPosition(new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z + gridDistance));
              break;
          case KnockBackDirection.Left:
              SetTargetPosition(new Vector3(transform.localPosition.x  - gridDistance,transform.localPosition.y,transform.localPosition.z));
              break;
          case KnockBackDirection.Backward:
              SetTargetPosition(new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z - gridDistance));
              break;
          case KnockBackDirection.Right:
              SetTargetPosition(new Vector3(transform.localPosition.x + gridDistance,transform.localPosition.y,transform.localPosition.z));
              break;
          case KnockBackDirection.ForwardLeft:
              SetTargetPosition(new Vector3(transform.localPosition.x - gridDistance,transform.localPosition.y,transform.localPosition.z + gridDistance));
              break;
          case KnockBackDirection.ForwardRight:
              SetTargetPosition(new Vector3(transform.localPosition.x + gridDistance,transform.localPosition.y,transform.localPosition.z + gridDistance));
              break;
          case KnockBackDirection.BackwardLeft:
              SetTargetPosition(new Vector3(transform.localPosition.x - gridDistance,transform.localPosition.y,transform.localPosition.z - gridDistance));
              break;
          case KnockBackDirection.BackwardRight:
              SetTargetPosition(new Vector3(transform.localPosition.x + gridDistance,transform.localPosition.y,transform.localPosition.z + gridDistance));
              break;
      }
   }
   
   private void BackDirectionHandle(Transform playerTransform)
   {
       
       if (playerTransform.position.x < transform.position.x && playerTransform.position.z < transform.position.z)
       {
           knockBackDirection = KnockBackDirection.ForwardRight;
       }
       else if (playerTransform.position.x < transform.position.x && playerTransform.position.z > transform.position.z)
       {
           knockBackDirection = KnockBackDirection.BackwardRight;
       }
       else if (playerTransform.position.x > transform.position.x && playerTransform.position.z > transform.position.z)
       {
           knockBackDirection = KnockBackDirection.ForwardLeft;
       }
       else if (playerTransform.position.x > transform.position.x && playerTransform.position.z < transform.position.z)
       {
           knockBackDirection = KnockBackDirection.BackwardLeft;
       }
       else if (playerTransform.position.x < transform.position.x)
       {
           // x + 1
           knockBackDirection = KnockBackDirection.Right;
       }
       else if (playerTransform.position.x > transform.position.x)
       {
           // x - 1
           knockBackDirection = KnockBackDirection.Left;
       }
       else if (playerTransform.position.z < transform.position.z)
       {
           // z + 1
           knockBackDirection = KnockBackDirection.Forward;
       }
       else if (playerTransform.position.z > transform.position.z)
       {
           // z - 1
           knockBackDirection = KnockBackDirection.Backward;
       }
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
           transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

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
}
