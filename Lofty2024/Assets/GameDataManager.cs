using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class GameDataManager : MonoBehaviour
{
    private void Awake()
    {
        LoadProgress();
        LoadCardManager();
    }

    [Button("Format All")]
    public void FormatAllData()
    {
        FormatProgress();
        FormatCardManager();
        FormatPlayerData();
        ES3.DeleteKey("TimeCount");
    }
    
    #region GameProgress

    public void SaveProgress()
    {
        ES3.Save("FirstStage",GetComponent<PortalManager>().firstStageNumber);
        ES3.Save("SecondStage",GetComponent<PortalManager>().secondStageNumber);
        ES3.Save("ClearCount",GetComponent<PortalManager>().stageClearCount);
    }

    public void LoadProgress()
    {
        GetComponent<PortalManager>().firstStageNumber = ES3.Load<int>("FirstStage",1);
        GetComponent<PortalManager>().stageClearCount = ES3.Load<int>("ClearCount",0);
    }

    public void FormatProgress()
    {
        ES3.DeleteKey("FirstStage");
        ES3.DeleteKey("SecondStage");
        ES3.DeleteKey("ClearCount");
    }

    #endregion
    

    #region Card Manager

    public void SaveCardManager()
    {
        ES3.Save("CardStock",GetComponent<RandomCardManager>().cardList);
    }

    public void LoadCardManager()
    {
        GetComponent<RandomCardManager>().cardList = ES3.Load("CardStock", GetComponent<RandomCardManager>().cardList);
    }

    public void FormatCardManager()
    {
        ES3.DeleteKey("CardStock");
    }
    #endregion
    
    private void FormatPlayerData()
    {
        ES3.DeleteKey("PlayerDefaultHealth");
        ES3.DeleteKey("PlayerDefaultHealthTemp");
        ES3.DeleteKey("PlayerCurrentHealth");
        ES3.DeleteKey("PlayerCurrentHealthTemp");
        
        ES3.DeleteKey("PlayerDefaultMovePoint");
        ES3.DeleteKey("PlayerDefaultDamage");
        ES3.DeleteKey("PlayerDefaultKnockBackRange");
        
        ES3.DeleteKey("ArtifactHave");
    }
}