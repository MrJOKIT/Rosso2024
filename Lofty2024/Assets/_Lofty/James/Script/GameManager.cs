using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;

public enum CursorType
{
    DefaultCursor,
    SkillCursor,
    DataCursor,
}
public class GameManager : Singeleton<GameManager>
{
    [Tab("Game Manager")]
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

    [Tab("Cursor Setting")] 
    public Texture2D defaultCursor;
    public Texture2D onSkillCursor;
    public Texture2D onDataCursor;

    #region Game Manager

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

    public void TryAgain()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    #endregion


    #region Cursor In Game

    public void ChangeCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.DefaultCursor:
                Cursor.SetCursor(defaultCursor,new Vector2(100,100),CursorMode.Auto);
                break;
            case CursorType.SkillCursor:
                Cursor.SetCursor(onSkillCursor,new Vector2(100,100),CursorMode.Auto);
                break;
            case CursorType.DataCursor:
                Cursor.SetCursor(onDataCursor,new Vector2(100,100),CursorMode.Auto);
                break;
        }
    }

    #endregion
}
