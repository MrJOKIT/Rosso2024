using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public class GameManager : Singeleton<GameManager>
{
    [Header("Clear Stage Setting")] 
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject gatePrefab;

    [Space(10)] 
    [Header("Stage Reward Setting")]
    public GameObject cardSelectCanvas;
    public Transform currentRoomPos;
    [SerializeField] private GameObject rewardVFX;
    [SerializeField] private Vector2Int dropRate;

    [Space(10)] 
    [Header("Game Over Setting")] 
    public GameObject deadCanvas;
    public void StageClear()
    {
        Debug.Log("Stage is clear!!!");
        StageReward();
        GameObject gateObject = Instantiate(gatePrefab, currentRoomPos.GetComponent<RoomManager>().CheckSpawnPoint(), Quaternion.identity);
        gateObject.GetComponent<GateToNextScene>().SetNextScene(sceneName);
        cardSelectCanvas.SetActive(true);
        GetComponent<RandomCardManager>().StartRandomCardFixGrade(ArtifactGrade.All,4);
    }

    public void RoomClear()
    {
        StageReward();
    }
    private void StageReward()
    {
        Instantiate(rewardVFX, new Vector3(currentRoomPos.position.x, 3,currentRoomPos.position.z), Quaternion.identity);
        GetComponent<GameCurrency>().IncreaseEricCoin(Random.Range(dropRate.x,dropRate.y));
    }
    
    public void UpdateCurrentRoom(Transform newCurrentRoom)
    {
        currentRoomPos = newCurrentRoom;
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        deadCanvas.SetActive(true);
    }
}
