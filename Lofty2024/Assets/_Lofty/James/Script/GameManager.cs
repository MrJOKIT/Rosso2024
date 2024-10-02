using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
using UnityEngine;

public class GameManager : Singeleton<GameManager>
{
    [Header("Clear Stage Setting")] 
    [SerializeField] private string sceneName;
    [SerializeField] private Transform spawnGatePosition;
    [SerializeField] private GameObject gatePrefab;

    [Space(10)]
    [Header("Stage Reward Setting")]
    [SerializeField] private GameObject rewardVFX;
    [MinMaxSlider(0,100)][SerializeField] private Vector2Int dropRate;
    public void StageClear()
    {
        Debug.Log("Stage is clear!!!");
        StageReward();
        GameObject gateObject = Instantiate(gatePrefab, spawnGatePosition.position, Quaternion.identity);
        gateObject.GetComponent<GateToNextScene>().SetNextScene(sceneName);
    }

    private void StageReward()
    {
        Instantiate(rewardVFX, new Vector3(3, 3,3), Quaternion.identity);
        GetComponent<GameCurrency>().IncreaseEricCoin(Random.Range(dropRate.x,dropRate.y));
    }
}
