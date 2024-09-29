using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using Unity.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

public class EnemyBrainML : Agent
{
    [Header("Debug")] 
    public MeshRenderer meshRenderer;
    public Material completeMat;
    public Material failMat;
    
    [Space(20)]
    public Transform targetTransform;

    public bool onTurn;
    public int stepCount;
    public bool actionSuccess;
    
    private const int enemy_NoAction = 0;  // do nothing!
    private const int enemy_Foward = 1;
    private const int enemy_Backward = 2;
    private const int enemy_Left = 3;
    private const int enemy_Right = 4;
    private const int enemy_ForwardLeft = 5;
    private const int enemy_ForwardRight = 6;
    private const int enemy_BackwardLeft = 7;
    private const int enemy_BackwardRight = 8;
    
    
    

    private void Awake()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        onTurn = GetComponent<Enemy>().onTurn;
    }

    public override void OnEpisodeBegin()
    {
        actionSuccess = false;
        stepCount = 0;
        transform.localPosition = new Vector3(3, 0.5f, 1);
        GetComponent<EnemyMovementGrid>().targetPosition = transform.localPosition;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        
        /*sensor.AddObservation(forwardMoveBlock);
        sensor.AddObservation(backwardMoveBlock);
        sensor.AddObservation(leftMoveBlock);
        sensor.AddObservation(rightMoveBlock);
        sensor.AddObservation(forwardLeftMoveBlock);
        sensor.AddObservation(forwardRightMoveBlock);
        sensor.AddObservation(backwardLeftMoveBlock);
        sensor.AddObservation(backwardRightMoveBlock);*/
        
        sensor.AddObservation(onTurn);
        sensor.AddObservation(stepCount);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (onTurn == false)
        {
            return;
        }
        if (actionSuccess)
        {
            return;
        }
        
        AddReward(-0.01f);
        var action = actions.DiscreteActions[0];
        switch (action)
        {
            case enemy_NoAction:
                //do nothing
                break;
            case enemy_Foward:
                /*if (forwardMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                actionSuccess = true;
                stepCount += 1;
                break;
            case enemy_Backward:
                /*if (backwardMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                actionSuccess = true;
                stepCount += 1;
                break;
            case enemy_Left:
                /*if (leftMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                actionSuccess = true;
                stepCount += 1;
                break;
            case enemy_Right:
                /*if (rightMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                actionSuccess = true;
                stepCount += 1;
                break;
            case enemy_ForwardLeft:
                /*if (forwardLeftMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                actionSuccess = true;
                stepCount += 1;
                break;
            case enemy_ForwardRight:
                /*if (forwardRightMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                actionSuccess = true;
                stepCount += 1;
                break;
            case enemy_BackwardLeft:
                /*if (backwardLeftMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                actionSuccess = true;
                stepCount += 1;
                break;
            case enemy_BackwardRight:
                /*if (backwardRightMoveBlock)
                {
                    return;
                }*/
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                actionSuccess = true;
                stepCount += 1;
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }
        Debug.Log("Action Started");
        if (stepCount > 20)
        {
            AddReward(-0.1f);
            meshRenderer.material = failMat;
            EndEpisode();
        }
    }
    
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        /*actionMask.SetActionEnabled(0,enemy_NoAction,false);
        actionMask.SetActionEnabled(0,enemy_Foward,true);
        actionMask.SetActionEnabled(0,enemy_Backward,true);
        actionMask.SetActionEnabled(0,enemy_Left,true);
        actionMask.SetActionEnabled(0,enemy_Right,true);
        actionMask.SetActionEnabled(1,enemy_ForwardLeft,true);
        actionMask.SetActionEnabled(1,enemy_ForwardRight,true);
        actionMask.SetActionEnabled(1,enemy_BackwardLeft,true);
        actionMask.SetActionEnabled(1,enemy_BackwardRight,true);*/
        
        //actionMask.SetActionEnabled(0,enemy_NoAction,false);
        /*if (!forwardMoveBlock)
        {
            actionMask.SetActionEnabled(0,enemy_Foward,false);
        }

        if (!backwardMoveBlock)
        {
            actionMask.SetActionEnabled(0,enemy_Backward,false);
        }

        if (!leftMoveBlock)
        {
            actionMask.SetActionEnabled(0,enemy_Left,false);
        }

        if (!rightMoveBlock)
        {
            actionMask.SetActionEnabled(0,enemy_Right,false);
        }

        if (!forwardLeftMoveBlock)
        {
            actionMask.SetActionEnabled(1,enemy_ForwardLeft,false);
        }

        if (!forwardRightMoveBlock)
        {
            actionMask.SetActionEnabled(1,enemy_ForwardRight,false);
        }

        if (!backwardLeftMoveBlock)
        {
            actionMask.SetActionEnabled(1,enemy_BackwardLeft,false);
        }

        if (!backwardRightMoveBlock)
        {
            actionMask.SetActionEnabled(1,enemy_BackwardRight,false);
        }*/
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            SetReward(-1);
            meshRenderer.material = failMat;
            actionSuccess = false;
            EndEpisode();
        }
        else if (other.CompareTag("DeadZone"))
        {
            SetReward(-1);
            meshRenderer.material = failMat;
            actionSuccess = false;
            EndEpisode();
        }
        else if (other.CompareTag("CloseArea"))
        { 
            SetReward(1);
            targetTransform.GetComponent<RandomPositionOntrigger>().RandomPosition();
            meshRenderer.material = completeMat;
            actionSuccess = false;
            EndEpisode();
        }
    }
    private void OnDrawGizmos()
    {
        //Forward Check
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(0,0,1) * 1.1f,Color.red);
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(-1,0,1) * 1.1f,Color.red);
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(1,0,1) * 1.1f,Color.red);
           
        //Backward Check
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(0,0,-1) * 1.1f,Color.red);
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(-1,0,-1) * 1.1f,Color.red);
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(1,0,-1) * 1.1f,Color.red);
           
        //Left & Right
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(-1,0,0) * 1.1f,Color.red);
        Debug.DrawRay(new Vector3(transform.position.x,transform.position.y + 0.2f,transform.position.z),new Vector3(1,0,0) * 1.1f,Color.red);
    }
}
