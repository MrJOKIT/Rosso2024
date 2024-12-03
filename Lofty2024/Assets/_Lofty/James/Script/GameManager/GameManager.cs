using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;
using Random = UnityEngine.Random;

public enum CursorType
{
    DefaultCursor,
    SkillCursor,
    DataCursor,
    AttackCursor,
}
public class GameManager : Singeleton<GameManager>
{
    [Tab("Game Manager")] 
    [SerializeField] private bool onLoad;
    public bool OnLoad => onLoad;
    [Header("Clear Stage Setting")] 
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject gatePrefab;

    [Space(10)] 
    [Header("Stage Reward Setting")]
    public GameObject cardSelectCanvas;
    public Transform currentRoomPos;
    [SerializeField] private GameObject rewardVFX;
    [SerializeField] private Vector2Int dropRate;

    [Space(10)] [Header("Game Clear Setting")]
    public GameObject clearGameCanvas;
    public TextRevealer textRevealer;
    [Header("Game Over Setting")] 
    public GameObject deadCanvas;

    [Tab("Time Count")]
    public TextMeshProUGUI timeText; // Drag a UI Text element to this in the Inspector.
    public TextMeshProUGUI timeTextTwo;
    private float elapsedTime; // Variable to store the elapsed time.
    private bool isCounting; // Flag to check if the timer is running.
    
    [Tab("Cursor Setting")] 
    public Texture2D defaultCursor;
    public Texture2D onSkillCursor;
    public Texture2D onDataCursor;
    public Texture2D onAttackCursor;

    public Vector2 mouseCursorHotSpot;

    private void Start()
    {
        SetCursorVisible(false);
        elapsedTime = ES3.Load<float>("TimeCount", 0f);
    }

    private void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay(); 
        }
    }

    public void OnLoadStage()
    {
        //SetCursorVisible(false);
        onLoad = true;
    }

    public void StageLoadSuccess()
    {
        //SetCursorVisible(true);
        onLoad = false;
    }

    public void SetCursorVisible(bool visible)
    {
        if (visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
        timeTextTwo.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    #region Game Manager

    public void StageClear()
    {
        PauseTimer();
        Debug.Log("Stage is clear!!!");
        StageReward();
        if (currentRoomPos.GetComponent<RoomManager>().roomType == RoomType.Boss)
        {
            return;
        }
        GameObject gateObject = Instantiate(gatePrefab, currentRoomPos.GetComponent<RoomManager>().CheckSpawnPoint(), Quaternion.identity);
        gateObject.GetComponent<GateToNextScene>().SetNextScene(sceneName);
        cardSelectCanvas.SetActive(true);
        GetComponent<RandomCardManager>().StartRandomCardFixGrade(ArtifactGrade.All,4);
        currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().SavePlayerData();
        ES3.Save<float>("TimeCount",elapsedTime);
    }

    public void GameClearRevealer()
    {
        textRevealer.Reveal();
    }

    public void GameClearCorutine()
    {
        StartCoroutine(BossRoomClear());
    }
    IEnumerator BossRoomClear()
    {
        yield return new WaitForSeconds(2f);
        TransitionAnimator transitionAnimator = TransitionAnimator.Start(TransitionType.Smear);
        transitionAnimator.onTransitionEnd.AddListener(GameManager.Instance.GameClear);
    }
    public void GameClear()
    {
        PauseTimer();
        SoundManager.instace.Play(SoundManager.SoundName.VictoryBGM);
        Debug.Log("Game is clear!!!");
        TurnManager.Instance.gameEnd = true;
        clearGameCanvas.SetActive(true);
        //GameObject gateObject = Instantiate(gatePrefab, currentRoomPos.GetComponent<RoomManager>().CheckSpawnPoint(), Quaternion.identity);
        //gateObject.GetComponent<GateToNextScene>().SetNextScene(sceneName);
        //cardSelectCanvas.SetActive(true);
        //GetComponent<RandomCardManager>().StartRandomCardFixGrade(ArtifactGrade.All,4);
        currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().SavePlayerData();
        ES3.Save<float>("TimeCount",elapsedTime);
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
        SoundManager.instace.Play(SoundManager.SoundName.DeadBGM);
        TurnManager.Instance.gameEnd = true;
        Debug.Log("Game Over");
        currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().FormatPlayerData();
        deadCanvas.SetActive(true);
    }

    public void TryAgain()
    {
        currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().FormatPlayerData();
        GetComponent<GameDataManager>().FormatAllData();
        SceneManager.LoadSceneAsync("Menu");
    }

    #endregion


    #region Cursor In Game

    public void ChangeCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.DefaultCursor:
                Cursor.SetCursor(defaultCursor,mouseCursorHotSpot,CursorMode.Auto);
                break;
            case CursorType.SkillCursor:
                Cursor.SetCursor(onSkillCursor,new Vector2(0,0),CursorMode.Auto);
                break;
            case CursorType.DataCursor:
                Cursor.SetCursor(onDataCursor,new Vector2(0,0),CursorMode.Auto);
                break;
            case CursorType.AttackCursor:
                Cursor.SetCursor(onAttackCursor,new Vector2(0,0),CursorMode.Auto);
                break;
        }
    }

    #endregion
}
