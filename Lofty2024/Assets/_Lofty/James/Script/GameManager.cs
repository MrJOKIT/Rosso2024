using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;
using Random = UnityEngine.Random;

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

    [Tab("Time Count")]
    public TextMeshProUGUI timeText; // Drag a UI Text element to this in the Inspector.
    private float elapsedTime; // Variable to store the elapsed time.
    private bool isCounting; // Flag to check if the timer is running.
    
    [Tab("Cursor Setting")] 
    public Texture2D defaultCursor;
    public Texture2D onSkillCursor;
    public Texture2D onDataCursor;

    private void Start()
    {
        elapsedTime = ES3.Load("TimeCount", 0);
    }

    private void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay(); 
        }
    }
    
    public void StartTimer()
    {
        isCounting = true; 
    }

    public void PauseTimer()
    {
        isCounting = false; 
    }

    public void ResetTimer()
    {
        isCounting = false;
        elapsedTime = 0f;
        UpdateTimeDisplay();
    }

    void UpdateTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime % 1) * 1000);
        
        timeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    #region Game Manager

    public void StageClear()
    {
        PauseTimer();
        Debug.Log("Stage is clear!!!");
        StageReward();
        GameObject gateObject = Instantiate(gatePrefab, currentRoomPos.GetComponent<RoomManager>().CheckSpawnPoint(), Quaternion.identity);
        gateObject.GetComponent<GateToNextScene>().SetNextScene(sceneName);
        cardSelectCanvas.SetActive(true);
        GetComponent<RandomCardManager>().StartRandomCardFixGrade(ArtifactGrade.All,4);
        currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().SavePlayerData();
        ES3.Save("TimeCount",elapsedTime);
    }

    public void RoomClear()
    {
        StageReward();
    }
    private void StageReward()
    {
        PauseTimer();
        Instantiate(rewardVFX, new Vector3(currentRoomPos.position.x, 3,currentRoomPos.position.z), Quaternion.identity);
        //GetComponent<VisualEffectManager>().CallEffect(EffectName.CoinReward,currentRoomPos,3f);
        GetComponent<GameCurrency>().IncreaseEricCoin(Random.Range(dropRate.x,dropRate.y));
    }
    
    public void UpdateCurrentRoom(Transform newCurrentRoom)
    {
        currentRoomPos = newCurrentRoom;
    }

    public void GameOver()
    {
        PauseTimer();
        TurnManager.Instance.gameEnd = true;
        Debug.Log("Game Over");
        currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().FormatPlayerData();
        deadCanvas.SetActive(true);
    }

    public void TryAgain()
    {
        currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().FormatPlayerData();
        GetComponent<GameDataManager>().FormatAllData();
        SceneManager.LoadSceneAsync("Lobby");
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
