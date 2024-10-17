using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class TurnData
{
    public bool isPlayer;
    public Transform unitTransform;
    public float baseSpeed;
    public TurnSlot turnSlot;

    public TurnData(bool isPlayer,Transform unitTransform,float baseSpeed,TurnSlot turnSlot)
    {
        this.isPlayer = isPlayer;
        this.unitTransform = unitTransform;
        this.baseSpeed = baseSpeed;
        this.turnSlot = turnSlot;
    }
}
public class TurnManager : Singeleton<TurnManager>
{
    [Space(10)] 
    [Header("Stage Manage")] 
    public bool currentRoomClear;
    
    [Space(10)]
    [Header("Turn Data")]
    public List<TurnData> turnData;
    public List<GameObject> queueTransform;
    [Space(10)]
    public bool onPlayerTurn;
    [Space(10)] 
    public bool onEnemyTurn;

    [Space(10)] 
    [Header("Prefab")] 
    public GameObject turnCanvas;
    public Transform turnSlotCanvas;
    public GameObject queuePrefab;
    public GameObject turnSlotAllyPrefab;
    public GameObject turnSlotEnemyPrefab;

    private void Update()
    {
        if (currentRoomClear)
        {
            onPlayerTurn = true;
            turnCanvas.SetActive(false);
            if (queueTransform.Count > 0)
            {
                foreach (GameObject queue in queueTransform.ToList())
                {
                    queueTransform.Remove(queue);
                    Destroy(queue);
                }
            }
            return;
        }
        
        
    }

    private void FixedUpdate()
    {
        if (onPlayerTurn || onEnemyTurn)
        {
            return;
        }
        TurnHandle();
    }

    public void TurnStart()
    {
        currentRoomClear = false;
        onPlayerTurn = false;
        onEnemyTurn = false;
        turnCanvas.SetActive(true);
        
        Invoke("CreateQueue",0.5f);
    }
    
    #region In Game Unit

    public void AddUnit(bool isPlayer,Transform unitTransform,float baseSpeed)
    { 
        if (isPlayer)
        {
            turnData.Add(new TurnData(isPlayer,unitTransform,baseSpeed,null));
        }
        else
        {
            TurnData data = new TurnData(isPlayer, unitTransform, baseSpeed, null);
            turnData.Add(data);
            data.unitTransform.GetComponent<EnemyAI>().enemyTurnData = data;
        }

        turnData.Sort(((data, data1) => data1.baseSpeed.CompareTo(data.baseSpeed)));
    }
    
    public void RemoveUnit(TurnData turnDataUnit)
    {
        turnDataUnit.turnSlot.ClearSlot();
        Destroy(turnData.Find(x=> x.unitTransform == turnDataUnit.unitTransform).turnSlot.gameObject);
        turnData.Remove(turnDataUnit);
        Destroy(queueTransform[queueTransform.Count - 1]);
        queueTransform.Remove(queueTransform[queueTransform.Count - 1]);   
        UpdateTurnGUI();
    }

    #endregion

    #region TurnManager

    private void CreateQueue()
    {
        if (queueTransform.Count == turnData.Count)
        {
            return;
        }
        for (int a = 0; a < turnData.Count; a++)
        {
            if (queueTransform.Count == turnData.Count)
            {
                continue;
            }
            GameObject queue = Instantiate(queuePrefab, turnSlotCanvas);
            queueTransform.Add(queue);
            if (turnData[a].isPlayer)
            {
                GameObject turnSlot = Instantiate(turnSlotAllyPrefab, queue.transform);
                turnData[a].turnSlot = turnSlot.GetComponent<TurnSlot>();
            }
            else
            {
                GameObject turnSlot =Instantiate(turnSlotEnemyPrefab, queue.transform); 
                turnData[a].turnSlot = turnSlot.GetComponent<TurnSlot>();
            }
        }
        UpdateTurnGUI();
    }
    
    private void TurnHandle()
    {
        if (turnData[0].isPlayer)
        {
            turnData[0].unitTransform.GetComponent<PlayerMovementGrid>().StartTurn();
            onPlayerTurn = true;
        }
        else if (!turnData[0].isPlayer)
        {
            turnData[0].unitTransform.GetComponent<Enemy>().StartTurn();
            onEnemyTurn = true;
        }
        
    } 

    private void UpdateTurnGUI() 
    { 
        for (int a = 0; a < turnData.Count; a++)
        {
            turnData[a].turnSlot.transform.SetParent(queueTransform[a].transform);
            turnData[a].turnSlot.transform.position = queueTransform[a].transform.position; 
        }
    }

    private void NextQueueTurn()
    {
        TurnData currentTurn = turnData[0];
        turnData.Remove(turnData[0]);
        turnData.Add(currentTurn);
        if (turnData[0].isPlayer)
        {
            turnData[0].unitTransform.GetComponent<PlayerMovementGrid>().onTurn = false;
            
        }
        else
        {
            turnData[0].unitTransform.GetComponent<EnemyAI>().onTurn = false;
        }
        
        UpdateTurnGUI();
    }
    [VInspector.Button("End Turn")] 
    public void TurnSucces()
    {
        NextQueueTurn();
        GridSpawnManager.Instance.ClearMover();
        onPlayerTurn = false;
        onEnemyTurn = false;
    }

    public void TurnContinue()
    {
        GridSpawnManager.Instance.ClearMover();
    }
    public void CurrentRoomClear()
    {
        currentRoomClear = true;
        GridSpawnManager.Instance.ClearMover();
    }

    #endregion
    

    
}
