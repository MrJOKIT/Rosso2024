using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

[Serializable]
public class ChaosData
{
    public string chaosName;
}
public class ChaosManager : Singeleton<ChaosManager>
{
    [Header("Chaos Point")] 
    [SerializeField] private int bigChaos;
    [SerializeField] private int maxBigChaos = 5;
    [Space(10)] 
    [SerializeField] private int chaosPoint;
    [SerializeField] private int maxChaosPoint = 50;
    [Space(20)] 
    [Header("Chaos GUI")] 
    public TextMeshProUGUI chaosPointText;
    public TextMeshProUGUI maxChaosPointText;
    [Space(5)] 
    public List<GameObject> chaosPointImage;
    
    [Space(10)]
    [Header("Chaos Setting")] 
    public List<ChaosData> currentChaos;
    public List<ChaosData> chaosList;

    private void Start()
    {
        ChaosUpdateUI();
    }

    private void ChaosUpdateUI()
    {
        chaosPointText.text = "" + chaosPoint;
        maxChaosPointText.text = "" + maxChaosPoint;

        switch (bigChaos)
        {
            case 0:
                chaosPointImage[0].SetActive(false);
                chaosPointImage[1].SetActive(false);
                chaosPointImage[2].SetActive(false);
                chaosPointImage[3].SetActive(false);
                chaosPointImage[4].SetActive(false);
                break;
            case 1:
                chaosPointImage[0].SetActive(true);
                chaosPointImage[1].SetActive(false);
                chaosPointImage[2].SetActive(false);
                chaosPointImage[3].SetActive(false);
                chaosPointImage[4].SetActive(false);
                break;
            case 2:
                chaosPointImage[0].SetActive(true);
                chaosPointImage[1].SetActive(true);
                chaosPointImage[2].SetActive(false);
                chaosPointImage[3].SetActive(false);
                chaosPointImage[4].SetActive(false);
                break;
            case 3:
                chaosPointImage[0].SetActive(true);
                chaosPointImage[1].SetActive(true);
                chaosPointImage[2].SetActive(true);
                chaosPointImage[3].SetActive(false);
                chaosPointImage[4].SetActive(false);
                break;
            case 4:
                chaosPointImage[0].SetActive(true);
                chaosPointImage[1].SetActive(true);
                chaosPointImage[2].SetActive(true);
                chaosPointImage[3].SetActive(true);
                chaosPointImage[4].SetActive(false);
                break;
            case 5:
                chaosPointImage[0].SetActive(true);
                chaosPointImage[1].SetActive(true);
                chaosPointImage[2].SetActive(true);
                chaosPointImage[3].SetActive(true);
                chaosPointImage[4].SetActive(true);
                break;
        }
    }

    public void IncreaseChaosPoint(int count) 
    {
        chaosPoint += count;
        if (chaosPoint >= maxChaosPoint)
        {
            if (bigChaos >= maxBigChaos)
            {
                return;
            }
            chaosPoint -= maxChaosPoint;
            ChaosActive();
        }
        
        ChaosUpdateUI();
    }

    private void ChaosActive()
    {
        bigChaos += 1;
        GetRandomChaosData();
    }

    [Button("Random Chaos")]
    private void GetRandomChaosData()
    {
        if (chaosList == null)
        {
            return;
        }
        int randomNumber = Random.Range(0, chaosList.Count - 1);
        currentChaos.Add(chaosList[randomNumber]);
        chaosList.Remove(chaosList[randomNumber]);
    }

    [Button("Remove Chaos")]
    private void RemoveChaos()
    {
        if (currentChaos == null)
        {
            return;
        }
        chaosList.Add(currentChaos[^1]);
        currentChaos.Remove(currentChaos[^1]);
    }
}
