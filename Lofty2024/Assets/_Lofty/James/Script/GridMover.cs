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
    OnTrap,
}

public class GridMover : MonoBehaviour
{
    [Header("Ref")] 
    public Enemy enemy;
    public Material oldMat;

    [Space(10)] 
    [Header("Checker")] 
    public GridState gridState;
    private GridState oldState;
    
    [Space(10)]
    [Header("Optional")]
    public bool enemyActive;
    
    [Header("Trap Setting")]
    public bool isTrap;
    public CurseType trapType;
    public int trapTime;

    private void Awake()
    {
        oldState = gridState;
        oldMat = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        GridSpawnManager.Instance.AddGridList(this);

        if (isTrap)
        {
            gridState = GridState.OnTrap;
        }
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
            case GridState.OnObstacle:
                GetComponent<MeshRenderer>().enabled = false;
                oldState = gridState;
                break;
            case GridState.OnTrap:
                GetComponent<MeshRenderer>().material = GridSpawnManager.Instance.trapMat;
                oldState = gridState;
                break;
            default:
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<MeshRenderer>().material = oldMat;
                oldState = gridState;
                enemyActive = false;
                break;
        }
    }

    public void ClearGrid()
    {
        if (gridState == GridState.Empty || gridState  == GridState.OnTrap)
        {
            return;
        }
        gridState = GridState.Empty;
        GetComponent<MeshRenderer>().material = oldMat;
    }

    public void ActiveEnemy()
    {
        GetComponent<MeshRenderer>().material = GridSpawnManager.Instance.attackMat;
        enemyActive = true;
    }

    public void SetTrap(CurseType curseType)
    {
        if (gridState == GridState.Empty || gridState == GridState.OnMove)
        {
            isTrap = true;
            gridState = GridState.OnTrap;
            trapType = curseType;
        }
    }

    private void TrapActive()
    {
        enemy.AddCurseStatus(trapType,trapTime);
        gridState = GridState.OnEnemy;
        GetComponent<MeshRenderer>().material = oldMat;
    }
    private void OnTriggerEnter(Collider other)
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
                enemy = other.GetComponent<Enemy>();
                if (isTrap)
                {
                    TrapActive();
                }
            }
            catch (Exception a)
            {
                Debug.Log($"No enemy script in collider {a}");
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
