using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridState
{
    Empty,
    OnMove,
    OnPlayer,
    OnEnemy,
    OnObstacle,
}
public class GridMover : MonoBehaviour
{
    [Header("Ref")] 
    public EnemyAI enemyAI;
    public Material oldMat;

    [Space(10)] 
    [Header("Checker")] 
    public GridState gridState;
    private GridState oldState;
    public bool enemyActive;

    private void Awake()
    {
        oldState = gridState;
        oldMat = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        GridSpawnManager.Instance.AddGridList(this);
    }

    private void LateUpdate()
    {
        if (gridState == oldState)
        {
            return;
        }

        switch (gridState)
        {
            case GridState.OnMove:
                GetComponent<MeshRenderer>().material = GridSpawnManager.Instance.movableMat;
                oldState = gridState;
                break;
            case GridState.OnEnemy:
                break;
            default:
                GetComponent<MeshRenderer>().material = oldMat;
                oldState = gridState;
                enemyActive = false;
                break;
        }
    }

    public void ClearGrid()
    {
        gridState = GridState.Empty;
        GetComponent<MeshRenderer>().material = oldMat;
    }

    public void ActiveEnemy()
    {
        GetComponent<MeshRenderer>().material = GridSpawnManager.Instance.attackMat;
        enemyActive = true;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gridState = GridState.OnPlayer;
        }
        else if(other.CompareTag("Enemy"))
        {
            gridState = GridState.OnEnemy;
            try
            {
                enemyAI = other.GetComponent<EnemyAI>();
            }
            catch (Exception a)
            {
                Debug.Log($"No enemyAI in collider {a}");
            }
            
        }
        else if (other.CompareTag("Obstacle"))
        {
            gridState = GridState.OnObstacle;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gridState = GridState.Empty;
        }
        else if(other.CompareTag("Enemy"))
        {
            gridState = GridState.Empty;
            
        }
        else if (other.CompareTag("Obstacle"))
        {
            gridState = GridState.Empty;
        }
    }
}
