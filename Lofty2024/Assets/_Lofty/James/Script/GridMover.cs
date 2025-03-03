using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public bool enemyDie;
    public bool isPortal;
    public bool isAlert;
    public Enemy enemy;
    public Material oldMat;

    [Space(10)] 
    [Header("Checker")] 
    public bool onHover;
    public GameObject selectedObject;
    [Space(10)]
    public GridState gridState;
    private GridState oldState;
    public bool gridActive;
    
    [Space(10)]
    [Header("Optional")]
    public bool enemyActive;

    private MeshRenderer mesh;
    

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        
        oldState = gridState;
        oldMat = GetComponent<MeshRenderer>().material;
    }
    

    private void LateUpdate()
    {
        selectedObject.SetActive(onHover);
        if (enemy != null)
        {
            if (enemy.isDead)
            {
                enemy = null;
                enemyActive = false;
                if (enemyDie)
                {
                    gridState = GridState.OnMove;
                    enemyDie = false;
                }
                else
                {
                    gridState = GridState.Empty;
                }
            }
        }
        GridStateHandle();
    }
    
    private void GridStateHandle()
    {
        if (gridState == oldState)
        {
            return;
        }

        CheckMoveType();
    }

    private void CheckMoveType()
    {
        switch (gridState)
        {
            case GridState.OnMove:
                mesh.enabled = true;
                mesh.material = GridSpawnManager.Instance.movableMat;
                oldState = gridState;
                break;
            case GridState.OnEnemy:
                mesh.enabled = true;
                oldState = gridState;
                break;
            case GridState.OnObstacle:
                mesh.enabled = false;
                oldState = gridState;
                break;
            case GridState.Empty:
                mesh.enabled = true;
                mesh.material = oldMat;
                oldState = gridState;
                enemyActive = false;
                break;
        }
    }

    public void ClearGrid()
    {
        isAlert = false;
        switch (gridState)
        {
            case GridState.OnObstacle:
                break;
            case GridState.OnPlayer:
                break;
            case GridState.OnMove:
                mesh.material = oldMat;
                gridState = GridState.Empty;
                break;
            case GridState.OnEnemy:
                mesh.material = oldMat;
                break;
            default:
                mesh.material = oldMat;
                gridState = GridState.Empty;
                break;
        }
        CheckMoveType();
    }

    public void ActiveEnemy()
    {
        mesh.material = GridSpawnManager.Instance.attackMat;
        enemyActive = true;
    }
    
    public void BombEnemy()
    {
        float randomNumber = Random.Range(0, 1f);
        if (randomNumber < 0.5f)
        {
            enemy.BombEnemy();
        }
        gridState = GridState.Empty;
        enemy = null;
    }
    
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            gridState = GridState.OnObstacle;
            GridSpawnManager.Instance.RemoveGrid(this);
            GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().currentGrid.Remove(this);
            Destroy(gameObject);
        }
        else if(other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            gridState = GridState.OnEnemy;
            try
            {
                enemy = other.GetComponent<Enemy>();
            }
            catch (Exception a)
            {
                Debug.Log($"No enemy script in collider {a}");
            }
        }
        else if (other.CompareTag("Player"))
        {
            gridState = GridState.OnPlayer;
        }
        
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (!gridActive)
        {
            return;
        }
        if (other.CompareTag("Obstacle"))
        {
            gridState = GridState.OnObstacle;
            GridSpawnManager.Instance.RemoveGrid(this);
            GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().currentGrid.Remove(this);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
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
        else if (other.CompareTag("Player"))
        {
            gridState = GridState.OnPlayer;
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gridState = GridState.Empty;
        }
        else if(other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            if (enemyDie)
            {
                gridState = GridState.OnMove;
                enemyDie = false;
            }
            else
            {
                gridState = GridState.Empty;
            }
            
            enemy = null;

        }
        else if (other.CompareTag("Obstacle"))
        {
            gridState = GridState.Empty;
        }
    }
}
