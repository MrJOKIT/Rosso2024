using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
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
    [Header("Turn Data")]
    public List<TurnData> turnData;
    [Space(10)]
    public bool onPlayerTurn;
    [Space(10)] 
    public bool onEnemyTurn;

    [Space(10)] 
    [Header("Prefab")] 
    public Transform turnSliderCanvas;
    public Slider turnSliderAllyPrefab;
    public Slider turnSliderEnemyPrefab;

    private void Update()
    {
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
                        td.unitTransform.GetComponent<PlayerMovementGrid>().onTurn = true;
                        onPlayerTurn = true;
                    }
                    else
                    {
                        td.unitTransform.GetComponent<Enemy>().StartTurn();
                        onEnemyTurn = true;
                    }
                    break;
                }
            }
            else
            {
                if (td.isPlayer)
                {
                    td.unitTransform.GetComponent<PlayerMovementGrid>().onTurn = true;
                    onPlayerTurn = true;
                }
                else
                {
                    td.unitTransform.GetComponent<EnemyAI>().onTurn = true;
                    onEnemyTurn = true;
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

    [Button("End Turn")]
    public void TurnSucces(bool isPlayer)
    {
        foreach (TurnData td in turnData)
        {
            if (td.turnCounter >= 100f)
            {
                td.turnCounter = td.baseSpeed;
                break;
            }
        }

        GridSpawnManager.Instance.ClearMover();
        if (isPlayer)
        {
            onPlayerTurn = false;
        }
        else
        {
            onEnemyTurn = false;
        }
        
        
    }

    #endregion
    

    
}
