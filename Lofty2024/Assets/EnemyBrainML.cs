using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class EnemyBrainML : Agent
{
    [Header("Debug")] 
    public MeshRenderer meshRenderer;
    public Material completeMat;
    public Material failMat;
    
    [Space(20)]
    public Transform targetTransform;
    

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
    
    
    [Header("Move Checker")]
    public LayerMask moveBlockLayer;
    [Space(10)]
    [ReadOnly] public bool forwardMoveBlock;
    [ReadOnly] public bool forwardLeftMoveBlock;
    [ReadOnly] public bool forwardRightMoveBlock;
    [ReadOnly] public bool backwardMoveBlock;
    [ReadOnly] public bool backwardLeftMoveBlock;
    [ReadOnly] public bool backwardRightMoveBlock;
    [ReadOnly] public bool leftMoveBlock;
    [ReadOnly] public bool rightMoveBlock;

    private void Awake()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        Debug.Log(!forwardMoveBlock);
    }

    private void Update()
    {
        CheckMoveHandle();
    }

    public override void OnEpisodeBegin()
    {
        actionSuccess = false;
        transform.localPosition = new Vector3(6, 0.5f, 3);
        GetComponent<EnemyMovementGrid>().targetPosition = transform.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (GetComponent<Enemy>().onTurn == false)
        {
            return;
        }
        if (actionSuccess)
        {
            return;
        }
        
        Debug.Log("Action Started");
        AddReward(-0.01f);
        var action = actions.DiscreteActions[0];
        switch (action)
        {
            case enemy_NoAction:
                //do nothing
                break;
            case enemy_Foward:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Forward);
                actionSuccess = true;
                break;
            case enemy_Backward:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Backward);
                actionSuccess = true;
                break;
            case enemy_Left:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Left);
                actionSuccess = true;
                break;
            case enemy_Right:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.Right);
                actionSuccess = true;
                break;
            case enemy_ForwardLeft:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardLeft);
                actionSuccess = true;
                break;
            case enemy_ForwardRight:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.ForwardRight);
                actionSuccess = true;
                break;
            case enemy_BackwardLeft:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardLeft);
                actionSuccess = true;
                break;
            case enemy_BackwardRight:
                GetComponent<EnemyMovementGrid>().MoveDirection(EnemyMoveDirection.BackwardRight);
                actionSuccess = true;
                break;
            default:
                throw new ArgumentException("Invalid action value");
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
        
        actionMask.SetActionEnabled(0,enemy_NoAction,false);
        actionMask.SetActionEnabled(0,enemy_Foward,!forwardMoveBlock);
        actionMask.SetActionEnabled(0,enemy_Backward,!backwardMoveBlock);
        actionMask.SetActionEnabled(0,enemy_Left,!leftMoveBlock);
        actionMask.SetActionEnabled(0,enemy_Right,!rightMoveBlock);
        actionMask.SetActionEnabled(1,enemy_ForwardLeft,!forwardLeftMoveBlock);
        actionMask.SetActionEnabled(1,enemy_ForwardRight,!forwardRightMoveBlock);
        actionMask.SetActionEnabled(1,enemy_BackwardLeft,!backwardLeftMoveBlock);
        actionMask.SetActionEnabled(1,enemy_BackwardRight,!backwardRightMoveBlock);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            SetReward(-1);
            meshRenderer.material = failMat;
            actionSuccess = true;
            EndEpisode();
        }
        else if (other.CompareTag("DeadZone"))
        {
            SetReward(-1);
            meshRenderer.material = failMat;
            actionSuccess = true;
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
