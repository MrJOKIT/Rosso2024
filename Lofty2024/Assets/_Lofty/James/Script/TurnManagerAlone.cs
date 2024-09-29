using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public class TurnManagerAlone : MonoBehaviour
{
     public List<TurnData> turnData;
    public bool onPlayerTurn;
    public bool onEnemyTurn;
    
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
