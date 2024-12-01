using System;
using System.Collections;
using System.Collections.Generic;
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
    [Range(0, 100)] public float turnCounter;
    public Slider turnSlider;

    public TurnData(bool isPlayer,Transform unitTransform,float baseSpeed,Slider turnSlider)
    {
        this.isPlayer = isPlayer;
        this.unitTransform = unitTransform;
        this.baseSpeed = baseSpeed;
        this.turnCounter = baseSpeed;
        this.turnSlider = turnSlider;
    }
}
public class TurnManager : Singeleton<TurnManager>
{
    [Header("AI Learn")]
    public bool multipleTurn;

    [Space(10)] 
    [Header("Stage Manage")] 
    public bool currentRoomClear;
    
    [Space(10)]
    [Header("Turn Data")]
    public List<TurnData> turnData;
    [Space(10)]
    public bool onPlayerTurn;
    [Space(10)] 
    public bool onEnemyTurn;

    [Space(10)] 
    [Header("Prefab")] 
    public GameObject turnCanvas;
    public Transform turnSliderCanvas;
    public Slider turnSliderAllyPrefab;
    public Slider turnSliderEnemyPrefab;

    private void Update()
    {
        if (currentRoomClear)
        {
            onPlayerTurn = true;
            turnCanvas.SetActive(false);
            return;
        }
        
        
        if (!multipleTurn)
        {
            if (onPlayerTurn || onEnemyTurn)
            {
                return;
            }
        }
        TurnHandle();
        UpdateTurnSliderGUI();
    }

    public void TurnStart()
    {
        currentRoomClear = false;
        onPlayerTurn = false;
        onEnemyTurn = false;
        turnCanvas.SetActive(true);
        foreach (TurnData td in turnData)
        {
            td.turnCounter = td.baseSpeed;
        }
        UpdateTurnSliderGUI();
    }
    
    #region In Game Unit

    public void AddUnit(bool isPlayer,Transform unitTransform,float baseSpeed)
    {
        if (isPlayer)
        {
            turnData.Add(new TurnData(
                isPlayer,unitTransform,baseSpeed
                ,Instantiate(turnSliderAllyPrefab,turnSliderCanvas)));
        }
        else
        {
            TurnData data = new TurnData(
                isPlayer, unitTransform, baseSpeed
                , Instantiate(turnSliderEnemyPrefab, turnSliderCanvas));
            
            turnData.Add(data);

            data.unitTransform.GetComponent<EnemyAI>().enemyTurnData = data;
        }
        
    }
    
    public void RemoveUnit(TurnData turnDataUnit)
    {
        Destroy(turnData.Find(x=> x.unitTransform == turnDataUnit.unitTransform).turnSlider.gameObject);
        turnData.Remove(turnDataUnit);
    }

    #endregion

    #region TurnManager

    private void TurnHandle()
    {
        foreach (TurnData td in turnData)
        {
            td.turnCounter += Time.deltaTime * 100f;
            if (!multipleTurn)
            {
                if (td.turnCounter >= 100f)
                {
                    if (td.isPlayer)
                    {
                        onPlayerTurn = true;
                        td.unitTransform.GetComponent<PlayerMovementGrid>().StartTurn();
                    }
                    else
                    {
                        onEnemyTurn = true;
                        td.unitTransform.GetComponent<Enemy>().StartTurn();
                    }
                    break;
                }
            }
            else
            {
                
                if (td.isPlayer)
                {
                    onPlayerTurn = true;
                    td.unitTransform.GetComponent<PlayerMovementGrid>().StartTurn();
                }
                else
                {
                    onEnemyTurn = true;
                    if (td.unitTransform.GetComponent<Enemy>().onTurn == false)
                    {
                        td.unitTransform.GetComponent<Enemy>().StartTurn();
                    }
                }
            }
        }
    }

    private void UpdateTurnSliderGUI()
    {
        foreach (TurnData td in turnData)
        {
            td.turnSlider.value = td.turnCounter / 100;
        }
    }

    [VInspector.Button("End Turn")]
    public void TurnSucces()
    {
        foreach (TurnData td in turnData)
        {
            if (td.turnCounter >= 100f)
            {
                td.turnCounter = 0;
                break;
            }
        }
        GridSpawnManager.Instance.ClearMover();
        if (!multipleTurn)
        {
            onPlayerTurn = false;
            onEnemyTurn = false;
        }
    }

    public void TurnContinue()
    {
        GridSpawnManager.Instance.ClearMover();
    }

    #endregion
    

    
}
