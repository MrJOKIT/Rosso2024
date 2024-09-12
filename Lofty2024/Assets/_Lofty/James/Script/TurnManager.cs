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
    public List<TurnData> turnData;
    public bool onPlayerTurn;
    public bool onEnemyTurn;

    [Space(10)] 
    [Header("Prefab")] 
    public Transform turnSliderCanvas;
    public Slider turnSliderAllyPrefab;
    public Slider turnSliderEnemyPrefab;

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
            turnData.Add(new TurnData(
                isPlayer,unitTransform,baseSpeed
                ,Instantiate(turnSliderEnemyPrefab,turnSliderCanvas)));
        }
        
    }

    private void Update()
    {
        if (onPlayerTurn || onEnemyTurn)
        {
            return;
        }
        TurnHandle();
        UpdateTurnSliderGUI();
    }

    private void TurnHandle()
    {
        foreach (TurnData td in turnData)
        {
            td.turnCounter += Time.deltaTime * 100f;
            if (td.turnCounter >= 100f)
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
                break;
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
    public void TurnSucces()
    {
        foreach (TurnData td in turnData)
        {
            if (td.turnCounter >= 100f)
            {
                td.turnCounter = td.baseSpeed;
                break;
            }

            if (td.isPlayer)
            {
                td.unitTransform.GetComponent<PlayerMovementGrid>().onTurn = false;
            }
            else
            {
                td.unitTransform.GetComponent<EnemyAI>().onTurn = false;
            }
        }

        GridSpawnManager.Instance.ClearMover();
        onPlayerTurn = false;
        onEnemyTurn = false;
    }
}
