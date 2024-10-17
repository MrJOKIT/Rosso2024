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
    OnTrap,
}

public class GridMover : MonoBehaviour
{
    [Header("Ref")]
    public bool isPortal;
    public Enemy enemy;
    public Material oldMat;

    [Space(10)] 
    [Header("Checker")] 
    public GridState gridState;
    private GridState oldState;
    public bool gridActive;
    
    [Space(10)]
    [Header("Optional")]
    public bool enemyActive;
    
    [Header("Trap Setting")]
    public bool isTrap;
    public CurseType trapType;
    public int trapTime;

    private CurseType oldTrapType;

    private void Awake()
    {
        oldState = gridState;
        oldTrapType = trapType;
        oldMat = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        if (isTrap)
        {
            gridState = GridState.OnTrap;
        }
        
    }

    private void LateUpdate()
    {
        if (enemy != null)
        {
            if (enemy.isDead)
            {
                enemy = null;
                enemyActive = false;
                gridState = GridState.Empty;
            }
        }
        GridStateHandle();
        TrapStateHandle();
    }

    private void TrapStateHandle()
    {
        if (trapType == oldTrapType)
        {
            return;
        }
        gridState = GridState.OnTrap;
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
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<MeshRenderer>().material = GridSpawnManager.Instance.movableMat;
                oldState = gridState;
                break;
            case GridState.OnEnemy:
                GetComponent<MeshRenderer>().enabled = true;
                oldState = gridState;
                break;
            case GridState.OnObstacle:
                GetComponent<MeshRenderer>().enabled = false;
                oldState = gridState;
                break;
            case GridState.OnTrap:
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<MeshRenderer>().material = GridSpawnManager.Instance.trapMat;
                oldState = gridState;
                break;
            case GridState.Empty:
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<MeshRenderer>().material = oldMat;
                oldState = gridState;
                enemyActive = false;
                break;
        }
    }

    public void ClearGrid()
    {
        switch (gridState)
        {
            case GridState.OnTrap:
                break;
            case GridState.OnObstacle:
                break;
            case GridState.OnPlayer:
                break;
            case GridState.OnMove:
                GetComponent<MeshRenderer>().material = oldMat;
                gridState = GridState.Empty;
                break;
            case GridState.OnEnemy:
                GetComponent<MeshRenderer>().material = oldMat;
                break;
            default:
                GetComponent<MeshRenderer>().material = oldMat;
                gridState = GridState.Empty;
                break;
        }
        CheckMoveType();
    }

    public void ActiveEnemy()
    {
        GetComponent<MeshRenderer>().material = GridSpawnManager.Instance.attackMat;
        enemyActive = true;
    }

    public void AttackEnemy(AttackType attackType,int damage,CurseType effectiveType,int effectiveTurnTime,int knockBackRange,Transform playerTrans)
    {
        try
        {
            switch (attackType)
            {
                case AttackType.NormalAttack:
                    if (GetComponent<PlayerArtifact>().GodOfWar)
                    {
                        float randomNumber = Random.Range(0, 1f);
                        if (randomNumber <= 0.2f)
                        {
                            enemy.TakeDamage(damage + 5);
                        }
                        else
                        {
                            enemy.TakeDamage(damage);
                        }
                    }
                    else
                    {
                        enemy.TakeDamage(damage);
                    }
                    if (enemy.enemyHealth <= 0)
                    {
                        gridState = GridState.Empty;
                        enemy = null;
                    }
                    break;
                case AttackType.SpecialAttack:
                    enemy.TakeDamage(damage * 2); 
                    if (enemy.enemyHealth <= 0)
                    {
                        gridState = GridState.Empty;
                        enemy = null;
                    }
                    break;
                case AttackType.KnockBackAttack:
                    enemy.TakeDamage(damage);
                    enemy.GetComponent<EnemyMovementGrid>().KnockBack(playerTrans,knockBackRange);
                    if (enemy.enemyHealth <= 0)
                    {
                        gridState = GridState.Empty;
                        enemy = null;
                    }
                    break;
                case AttackType.EffectiveAttack:
                    enemy.TakeDamage(damage);
                    enemy.AddCurseStatus(effectiveType,effectiveTurnTime);
                    if (enemy.enemyHealth <= 0)
                    {
                        gridState = GridState.Empty;
                        enemy = null;
                    }
                    break;
            }

            
        }
        catch (Exception a)
        {
            Debug.Log($"No enemy script in gird {a}");
        }
    }

    [Button("Test Set Trap")]
    public void SetTrap(CurseType curseType)
    {
        if (gridState == GridState.Empty || gridState == GridState.OnMove)
        {
            isTrap = true;
            gridState = GridState.OnTrap;
            trapType = curseType;
        }
        else if (gridState == GridState.OnEnemy)
        {
            enemy.AddCurseStatus(curseType,2);
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
        if (other.CompareTag("Obstacle"))
        {
            gridState = GridState.OnObstacle;
            GridSpawnManager.Instance.RemoveGrid(this);
            GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().currentGrid.Remove(this);
            Destroy(gameObject);
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
        else if(other.CompareTag("Enemy"))
        {
            gridState = GridState.Empty;
            enemy = null;

        }
        else if (other.CompareTag("Obstacle"))
        {
            gridState = GridState.Empty;
        }
    }
}
