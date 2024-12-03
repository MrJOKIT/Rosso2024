using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

[Serializable]
public class TutorialList
{
    public TutorialName tutorialName;
    public Sprite questScreenShot;
    [TextArea]public string questDetail;
    public bool isComplete;
}

public enum TutorialState
{
    Idle,
    OnProgress,
    CompleteProgress,
}

public enum TutorialName
{
    Empty,
    HowToMove,
    HowToNextRoom,
    HowToAttack,
    HowToGetAbility,
    HowToRandomCard,
    WizardAppear,
    EnemyLanding,
    HowToRestoreHealth,
    CardLink,
    CurseAppear,
    TrapAndObstacle,
}
public class TutorialManager : Singeleton<TutorialManager>
{
    public bool activeTutorial;
    public GameObject tutorialCanvas;
    public GameObject confirmCanvas;
    [Header("GUI")] 
    public Image tutorialImage;
    public TextMeshProUGUI tutorialText;

    [Space(20)] 
    [Header("Tutorial Manage")]
    public TutorialName currentTutorial;
    public TutorialState tutorialState;
    public List<TutorialList> tutorialLists;

    private void Start()
    {
        tutorialLists = ES3.Load("Tutorial",tutorialLists);
    }

    public void OpenTutorial()
    {
        tutorialState = TutorialState.CompleteProgress;
        ResetTutorial();
        activeTutorial = true;
        ActiveTutorial(TutorialName.HowToMove);
        confirmCanvas.SetActive(false);
        
        ES3.Save("TutorialPopUp",false);
    }

    public void CloseTutorial()
    {
        tutorialState = TutorialState.CompleteProgress;
        activeTutorial = false;
        foreach (TutorialList tutorialList in tutorialLists)
        {
            tutorialList.isComplete = true;
        }
        confirmCanvas.SetActive(false);
        ES3.Save("TutorialPopUp",false);
    }
    [Button("Test HowToMove")]
    public void TestTutorial()
    {
        ActiveTutorial(TutorialName.HowToMove);
    }
    public void ActiveTutorial(TutorialName tutorialName)
    {
        if (tutorialState == TutorialState.OnProgress)
        {
            return;
        }
        if (activeTutorial == false)
        {
            return;
        }
        if (GetTutorial(tutorialName).isComplete)
        {
            return;
        }

        currentTutorial = tutorialName;
        tutorialImage.sprite = GetTutorial(tutorialName).questScreenShot;
        tutorialText.text = GetTutorial(tutorialName).questDetail;
        
        tutorialCanvas.SetActive(true);
        ES3.Save("Tutorial",tutorialLists);
        tutorialState = TutorialState.OnProgress;
    }

    public void ResetTutorial()
    {
        foreach (TutorialList tutorial in tutorialLists)
        {
            tutorial.isComplete = false;
        }
        ES3.Save("Tutorial",tutorialLists);
    }
    public void AgreeTutorial()
    {
        GetTutorial(currentTutorial).isComplete = true;
        tutorialImage.sprite = null;
        tutorialText.text = String.Empty;
        tutorialState = TutorialState.CompleteProgress;
        tutorialCanvas.SetActive(false);
    }

    public TutorialList GetTutorial(TutorialName tutorialName)
    {
        return tutorialLists.Find(x => x.tutorialName == tutorialName);
    }
}
