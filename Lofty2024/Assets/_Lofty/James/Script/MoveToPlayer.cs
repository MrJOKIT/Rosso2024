using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToPlayer : Agent
{
    [Header("Move Setting")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private MovementState currentState;
    public float moveSpeed = 5f;
    public Vector3 gridSize = new Vector3(1f, 1f, 1f);

    [Space(10)] 
    [Header("Debug Test")] 
    public MeshRenderer meshRendererTest;
    public Material correctTest;
    public Material incorrectTest;
    
    [Header("Hide")]
    private Vector3 targetPosition;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(1, 0.5f, 0);
        targetPosition = transform.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //เชื่อมกับ Heuristic
        if (GetComponent<EnemyAI>().onTurn == false)
        {
            return;
        }

        if (currentState == MovementState.Moving)
        {
            return;
        }

        int moveDirection = actions.DiscreteActions[0];
        
        Debug.Log(moveDirection);
        
        switch (moveDirection)
        {
            case 0:
                SetTargetPosition(Vector3.forward);
                break;
            case 1:
                SetTargetPosition(Vector3.left);
                break;
            case 2:
                SetTargetPosition(Vector3.back);
                break;
            case 3:
                SetTargetPosition(Vector3.right);
                break;
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteAction = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            discreteAction[0] = 0;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteAction[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteAction[0] = 2;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteAction[0] = 3;
        }
        else
        {
            discreteAction[0] = 4;
        }
        
    }

    private void Update()
    {
        
        MoveStateHandle();
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
            GridSpawnManager.Instance.ClearMover();
            TurnManager.Instance.TurnSucces(false);
            currentState = MovementState.Idle;
            GetComponent<EnemyAI>().onTurn = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AddReward(+1);
            meshRendererTest.material = correctTest;
            EndEpisode();
        }
        else if (other.CompareTag("Obstacle"))
        {
            AddReward(-1);
            meshRendererTest.material = incorrectTest;
            EndEpisode();
        }
    }
}
