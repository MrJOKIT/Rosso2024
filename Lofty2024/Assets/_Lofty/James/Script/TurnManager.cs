using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

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
    [Tab("Turn Handle")]
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
    public bool gameEnd;

    [Tab("Combat Log")] 
    [Header("Log Ref")]
    public Transform logSpawnPoint;
    public GameObject logPrefab;
    public List<CombatLogSlot> currentLog;
    [Header("Log Setting")]
    public int maxLog;
    public float logTime;
    private float logTimeCounter;
    
    private void Update()
    {
        CombatLogHandle();
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
        if (onPlayerTurn || onEnemyTurn || gameEnd)
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
            data.unitTransform.GetComponent<Enemy>().enemyTurnData = data;
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
        foreach (var _turn in turnData)
        {
            if (queueTransform.Count == turnData.Count)
            {
                continue;
            }
            GameObject queue = Instantiate(queuePrefab, turnSlotCanvas);
            queueTransform.Add(queue);
            if (_turn.isPlayer)
            {
                GameObject turnSlot = Instantiate(turnSlotAllyPrefab, queue.transform);
                _turn.turnSlot = turnSlot.GetComponent<TurnSlot>();
            }
            else
            {
                GameObject turnSlot =Instantiate(turnSlotEnemyPrefab, queue.transform); 
                _turn.turnSlot = turnSlot.GetComponent<TurnSlot>();
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
            if (turnData[a].isPlayer == false)
            {
                turnData[a].turnSlot.SetSlot(turnData[a].unitTransform.GetComponent<Enemy>(),turnData[a].unitTransform.GetComponent<Enemy>().enemyData.enemySprite);
            }
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
            turnData[0].unitTransform.GetComponent<Enemy>().onTurn = false;
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
        GridSpawnManager.Instance.useWarp = false;
        GridSpawnManager.Instance.ClearMover();
    }

    #endregion


    public void AddNewQueue(Transform transform)
    {
        GameObject queue = Instantiate(queuePrefab, turnSlotCanvas);
        queueTransform.Add(queue);
        GameObject turnSlot =Instantiate(turnSlotEnemyPrefab, queue.transform); 
        turnData.Find(x => x.unitTransform == transform).turnSlot = turnSlot.GetComponent<TurnSlot>();;
    }

    #region Combat Log

    public void AddLog(string ownerName, string oppositeName, LogList logList,bool isPlayer)
    {
        GameObject logObject = Instantiate(logPrefab, logSpawnPoint);
        logObject.GetComponent<CombatLogSlot>().SetLog(ownerName, oppositeName, logList,isPlayer);
        
        currentLog.Add(logObject.GetComponent<CombatLogSlot>());
    }

    private void CombatLogHandle()
    {
        if (currentLog.Count > 0)
        {
            logTimeCounter += Time.deltaTime;
            if (logTimeCounter >= logTime)
            {
                Destroy(currentLog[0].gameObject);
                currentLog.Remove(currentLog[0]);
                logTimeCounter = 0;
            }
            else if (currentLog.Count > maxLog)
            {
                Destroy(currentLog[0].gameObject);
                currentLog.Remove(currentLog[0]);
                logTimeCounter = 0;
            }
        }
    }

    #endregion

}
