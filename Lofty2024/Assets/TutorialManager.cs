using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class TutorialList
{
    [TextArea]public string questDetail;
    public int taskMax;
    public int currentTask;
}

public enum TutorialState
{
    OnProgress,
    CompleteProgress,
}
public class TutorialManager : Singeleton<TutorialManager>
{
    public TextMeshProUGUI tutorialText;
    public int tutorialNumber;
    public TutorialState tutorialState;
    public float completeTime;
    private float completeTimeCounter;
    public List<TutorialList> tutorialLists;
    
    private void Update()
    {
        switch (tutorialState)
        {
            case TutorialState.OnProgress:
                switch (tutorialNumber)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }
                break;
            case TutorialState.CompleteProgress:
                completeTimeCounter += Time.deltaTime;
                if (completeTimeCounter > completeTime)
                {
                    tutorialState = TutorialState.OnProgress;
                }
                break;
        }
        
    }

    public void UpdateTaskCount()
    {
        tutorialLists[tutorialNumber].currentTask += 1;
        if (tutorialLists[tutorialNumber].currentTask >= tutorialLists[tutorialNumber].taskMax)
        {
            
        }
    }
}
