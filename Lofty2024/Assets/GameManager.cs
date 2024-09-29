using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singeleton<GameManager>
{
    [Header("Clear Stage Setting")] 
    [SerializeField] private string sceneName;
    [SerializeField] private Transform spawnGatePosition;
    [SerializeField] private GameObject gatePrefab;
    
    public void StageClear()
    {
        Debug.Log("Stage is clear!!!");
        GameObject gateObject = Instantiate(gatePrefab, spawnGatePosition.position, Quaternion.identity);
        gateObject.GetComponent<GateToNextScene>().SetNextScene(sceneName);
    }
}
