using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public enum ProgressState
{
    StandBy,
    OnProgress,
    ProgressSuccess,
}

public class PortalManager : Singeleton<PortalManager>
{


    [Tab("Random Room Setting")] 
    [SerializeField] private int gameLoopCount;
    public int firstStageNumber = 1;
    public int secondStageNumber = 1;
    public int stageClearCount;
    [Space(10)] 
    [SerializeField] private int maxFirstStage = 3;
    [SerializeField] private int roomCount;
    [Space(10)]
    [Header("Spawn List")]
    public List<GameObject> battleRoomModelList;
    public List<GameObject> bonusRoomModelList;
    public List<GameObject> clearRoomModelList;
    public List<GameObject> bossRoomModelList;
    
    [Header("Current Spawn")]
    public List<GameObject> battleRoomModel;
    public List<GameObject> bonusRoomModel;
    public List<GameObject> clearRoomModel;
    public List<GameObject> bossRoomModel;
    public Vector3 spawnPoint;
    
    [Space(20)]
    [Header("Room One")]
    public PortalToNextRoom portalLeft;
    public RoomManager leftRoom;
    public RoomType leftRoomType;
    public Vector3 leftRoomWarpPoint;
    
    [Space(10)]
    [Header("Room Two")]
    public PortalToNextRoom portalRight;
    public RoomManager rightRoom;
    public RoomType rightRoomType;
    public Vector3 rightRoomWarpPoint;

    [Space(20)] 
    [Header("Clear setting")] 
    public RoomManager currentRoom;
    public Transform playerTransform;
    public Transform portalLeftPos;
    public Transform portalRightPos;
    private bool leftOrRight;
    public bool isBossRoom;

    [Space(20)] 
    [Header("Room Checker")] 
    public LayerMask roomLayer;
    [Space(7)] 
    public Transform checkForwardRoomPos;
    public bool checkForwardRoom;
    [Space(7)]
    public Transform checkLeftRoomPos;
    public bool checkLeftRoom;
    [Space(7)]
    public Transform checkMiddleRoomPos;
    public bool checkMiddleRoom;
    [Space(7)]
    public Transform checkRightRoomPos;
    public bool checkRightRoom;

    [Tab("Game Progress")] 
    public ProgressState progressState;
    public GameObject progressCanvas;
    public TextMeshProUGUI currentStageNumberText;
    public Transform rossoUI;
    public List<ProgressBar> progressList;
    [Space(10)] 
    public float loadTimeMax;
    private float loadTimeCounter;


    public override void Awake()
    {
        base.Awake();
        GetComponent<GameDataManager>().LoadProgress();
    }

    private void Start()
    {
        SetProgress();
        SetRoomList();
    }

    public void SetRoomList()
    {
        battleRoomModel.Clear();
        bonusRoomModel.Clear();
        clearRoomModel.Clear();
        bonusRoomModel.Clear();
        foreach (GameObject room in battleRoomModelList)
        {
            battleRoomModel.Add(room);
        }
        foreach (GameObject room in bonusRoomModelList)
        {
            bonusRoomModel.Add(room);
        }
        foreach (GameObject room in clearRoomModelList)
        {
            clearRoomModel.Add(room);
        }
        foreach (GameObject room in bossRoomModelList)
        {
            bossRoomModel.Add(room);
        }
    }
    private void Update()
    {
        checkForwardRoom = Physics.Raycast(checkForwardRoomPos.position,Vector3.down, Mathf.Infinity, roomLayer);
        checkLeftRoom = Physics.Raycast(checkLeftRoomPos.position,Vector3.down, Mathf.Infinity, roomLayer);
        checkMiddleRoom = Physics.Raycast(checkMiddleRoomPos.position, Vector3.down, Mathf.Infinity, roomLayer);
        checkRightRoom = Physics.Raycast(checkRightRoomPos.position, Vector3.down, Mathf.Infinity, roomLayer);

        if (progressState == ProgressState.OnProgress)
        {
            currentStageNumberText.text = $"STAGE {firstStageNumber} - {secondStageNumber}";
            progressCanvas.SetActive(true);
            ProgressBarUpdate();
        }
        else if (progressState == ProgressState.ProgressSuccess)
        {
            progressState = ProgressState.StandBy;
        }
    }

    public void ShowStageNumber()
    {
        GetComponent<AnnouncementManager>().ShowTextTimer($"Stage {firstStageNumber} - {secondStageNumber}",5f);
        
    }
    private Vector3 GetSpawnPoint()
    {
        

        if (!checkForwardRoom)
        {
            spawnPoint = new Vector3(checkForwardRoomPos.position.x, 0, checkForwardRoomPos.position.z);
        }
        else if (!checkLeftRoom)
        {
            spawnPoint = new Vector3(checkLeftRoomPos.position.x, 0, checkLeftRoomPos.position.z);
        }
        else if (!checkMiddleRoom)
        {
            spawnPoint = new Vector3(checkMiddleRoomPos.position.x, 0, checkMiddleRoomPos.position.z);
        }
        else if (!checkRightRoom)
        {
            spawnPoint = new Vector3(checkRightRoomPos.position.x, 0, checkRightRoomPos.position.z);
        }

        return spawnPoint;
    }
    
    
    public void SetUpNextRoom(Transform portalLeftPos,Transform portalRightPos,Transform playerTransform)
    {
        SetRoomList();
        if (roomCount <= 0)
        {
            portalLeft.DeActivePortal();;
            portalRight.DeActivePortal();
        }
        
        
        roomCount -= 1;

        this.playerTransform = playerTransform;
        this.portalLeftPos = portalLeftPos;
        this.portalRightPos = portalRightPos;


        
        if (isBossRoom)
        {
            Invoke("SpawnRoom",0.2f);
        }
        else
        {
            Invoke("SpawnRoom",0.2f);
            Invoke("SpawnRoom",0.3f);
        }

        if (roomCount <= 0 && firstStageNumber >= maxFirstStage)
        {
            isBossRoom = true;
        }
    }

    private void SpawnRoom()
    {
        if (isBossRoom)
        {
            //show one portal
            rightRoomType = RandomRoom();
            GameObject rightRoomObject = Instantiate(GetRoom(rightRoomType), GetSpawnPoint(), Quaternion.identity);
            rightRoom = rightRoomObject.GetComponent<RoomManager>();
            rightRoomWarpPoint = rightRoom.GetComponent<RoomManager>().startPoint.transform.position;
            portalRight.SetPortal(rightRoomType,rightRoomWarpPoint,rightRoom.transform,playerTransform,rightRoom.iconAnimator);
            portalRight.transform.position = portalRightPos.position;
            portalRight.GetComponent<PortalToNextRoom>().ActivePortal();
            if (roomCount == 0)
            {
                rightRoom.isLastRoom = true;
            }
        }
        else
        {
            if (leftOrRight)
            {
                leftRoomType = RandomRoom();
                GameObject leftRoomObject = Instantiate(GetRoom(leftRoomType),GetSpawnPoint(),Quaternion.identity);
                leftRoom = leftRoomObject.GetComponent<RoomManager>();
                leftRoomWarpPoint = leftRoom.GetComponent<RoomManager>().startPoint.transform.position;
                portalLeft.SetPortal(leftRoomType,leftRoomWarpPoint,leftRoom.transform,playerTransform,leftRoom.iconAnimator);
                portalLeft.transform.position = portalLeftPos.position;
                portalLeft.GetComponent<PortalToNextRoom>().ActivePortal();
                if (roomCount == 0)
                {
                    leftRoom.isLastRoom = true;
                }

                leftOrRight = false;
            }
            else
            {
                rightRoomType = RandomRoom();
                GameObject rightRoomObject = Instantiate(GetRoom(rightRoomType), GetSpawnPoint(), Quaternion.identity);
                rightRoom = rightRoomObject.GetComponent<RoomManager>();
                rightRoomWarpPoint = rightRoom.GetComponent<RoomManager>().startPoint.transform.position;
                portalRight.SetPortal(rightRoomType,rightRoomWarpPoint,rightRoom.transform,playerTransform,rightRoom.iconAnimator);
                portalRight.transform.position = portalRightPos.position;
                portalRight.GetComponent<PortalToNextRoom>().ActivePortal();
                if (roomCount == 0)
                {
                    rightRoom.isLastRoom = true;
                }

                leftOrRight = true;
            }
        }
        
    }
    

    public void StartClearRoom()
    {
        Invoke("ClearRoom",0.1f);
    }
    private void ClearRoom()
    {
        if (leftRoom != null)
        {
            /*switch (leftRoom.roomType)
            {
                case RoomType.Combat:
                    battleRoomModel.Add(leftRoom.gameObject);
                    break;
                case RoomType.Bonus:
                    bonusRoomModel.Add(leftRoom.gameObject);
                    break;
                case RoomType.Clear:
                    clearRoomModel.Add(leftRoom.gameObject);
                    break;
                case RoomType.Boss:
                    bossRoomModel.Add(leftRoom.gameObject);
                    break;
            }*/
            if (leftRoom.playerTrans != null)
            {
                if (currentRoom == null)
                {
                    currentRoom = leftRoom;
                    leftRoom = null;
                }
                else
                {
                    currentRoom.DestroyRoom();
                    currentRoom = leftRoom;
                    leftRoom = null;
                }
                
            }
            else
            {
                leftRoom.DestroyRoom();
                leftRoom = null;
            }
        }
         
        if (rightRoom != null)
        {
            /*switch (rightRoom.roomType)
            {
                case RoomType.Combat:
                    battleRoomModel.Add(rightRoom.gameObject);
                    break;
                case RoomType.Bonus:
                    bonusRoomModel.Add(rightRoom.gameObject);
                    break;
                case RoomType.Clear:
                    clearRoomModel.Add(rightRoom.gameObject);
                    break;
                case RoomType.Boss:
                    bossRoomModel.Add(rightRoom.gameObject);
                    break;
            }*/
            if (rightRoom.playerTrans != null)
            {
                if (currentRoom == null)
                {
                    currentRoom = rightRoom;
                    rightRoom = null;
                }
                else
                {
                    currentRoom.DestroyRoom();
                    currentRoom = rightRoom;
                    rightRoom = null;
                }
                
            }
            else
            {
                rightRoom.DestroyRoom();
                rightRoom = null;
            }
        }
    }
    
    
    public RoomType RandomRoom()
    {
        RoomType roomType = RoomType.Combat;
        int randomNumber = Random.Range(0, 2);
        switch (randomNumber)
        {
            case 0:
                roomType = RoomType.Combat;
                break;
            case 1:
                if (bonusRoomModel.Count > 0)
                {
                    roomType = RoomType.Bonus;
                }
                else
                {
                    roomType = RoomType.Combat;
                }
                
                break;
            case 2:
                if (clearRoomModel.Count > 0)
                {
                    roomType = RoomType.Clear;
                }
                else
                {
                    roomType = RoomType.Combat;
                }
                
                break;
        }

        if (roomCount == 0)
        {
            if (isBossRoom)
            {
                roomType = RoomType.Boss;
            }
            else
            {
                roomType = RoomType.Combat;
            }
        }

        return roomType;
    }
     
    private GameObject GetRoom(RoomType roomType)
    {
        GameObject room = null;
        switch (roomType)
        {
            case RoomType.Clear:
                room = clearRoomModel[Random.Range(0, clearRoomModel.Count - 1)];
                clearRoomModel.Remove(room);
                break;
            case RoomType.Bonus:
                room = bonusRoomModel[Random.Range(0, bonusRoomModel.Count - 1)];
                bonusRoomModel.Remove(room);
                break;
            case RoomType.Combat:
                room = battleRoomModel[Random.Range(0, battleRoomModel.Count - 1)];
                //battleRoomModel.Remove(room);
                break;
            case RoomType.Boss:
                room = bossRoomModel[Random.Range(0, bossRoomModel.Count - 1)];
                break;
        }

        if (roomCount == 0)
        {
            if (isBossRoom)
            {
                //random boss room
                room = bossRoomModel[Random.Range(0, bossRoomModel.Count - 1)];
            }
            else
            {
                room = battleRoomModel[Random.Range(0, battleRoomModel.Count - 1)];
            }
        }

        return room;
    }

    private void SetProgress()
    {
        for (int a = 0; a < progressList.Count; a++)
        {
            progressList[a].stageText.text = $"{firstStageNumber} - {a + 1}";
        }
    }
    private void ProgressBarUpdate()
    { 
        Transform target = progressList[secondStageNumber - 1].barPoint;
        if (rossoUI.position == target.position || loadTimeCounter > 20)
        {
            progressState = ProgressState.ProgressSuccess;
            StartCoroutine(UpdateProgressSuccess());
        }
        else
        {
            loadTimeCounter += Time.deltaTime;
            rossoUI.position = Vector3.MoveTowards(rossoUI.position,target.position, 100 * Time.deltaTime);
        }
    }

    public void HidePortal()
    {
        portalLeft.transform.position = new Vector3(-20f, 0.5f, 0);
        portalRight.transform.position = new Vector3(-20f, 0.5f, 0);
    }
    IEnumerator UpdateProgressSuccess()
    {
        if (loadTimeCounter < loadTimeMax)
        {
            yield return new WaitForSeconds(5f - loadTimeCounter);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        progressCanvas.SetActive(false);
        switch (GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().roomType)
        {
            case RoomType.Combat:
                SoundManager.instace.Play(SoundManager.SoundName.BattleBGM);
                break;
            case RoomType.Bonus:
                SoundManager.instace.Play(SoundManager.SoundName.BonusBGM);
                break;
            case RoomType.Clear:
                SoundManager.instace.Play(SoundManager.SoundName.BonusBGM);
                break;
            case RoomType.Boss:
                SoundManager.instace.Play(SoundManager.SoundName.BattleBGM);
                break;
        }
        
        TransitionAnimator transitionAnimator = TransitionAnimator.Start(TransitionType.Fade,duration: 2f,invert:true,autoDestroy:true);
        GameManager.Instance.GetComponent<PortalManager>().ShowStageNumber();
        if (secondStageNumber <= 3)
        {
            secondStageNumber += 1;
        }
        else
        {
            firstStageNumber += 1;
            secondStageNumber = 1;
        }

        stageClearCount += 1;
        
        GetComponent<GameDataManager>().SaveProgress();
        transitionAnimator.onTransitionEnd.AddListener(GetComponent<GameManager>().StageLoadSuccess);

    }

    
}
