using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCardManager : MonoBehaviour
{
    public List<ArtifactData> cardList;
    [Space(10)] 
    [Range(1,10)]public int randomCount;
    [Space(10)]
    public int randomCost;
    
    
    public void StartRandomCard()
    {
        
    }
}
