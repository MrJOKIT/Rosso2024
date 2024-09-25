using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomPositionOntrigger : MonoBehaviour
{
    public bool skipRandom;
    public List<Transform> randomPositionTransforms;
    [ReadOnly] public int oldNumber;

    [Space(10)] 
    public List<ClosePlayerChecker> closePlayerCheckers;

    [Button("Test Random")]
    public void RandomPosition()
    {
        if (skipRandom)
        {
            return;
        }
        int randomNumber;
        do
        {
            randomNumber = Random.Range(0, randomPositionTransforms.Count - 1);
        } while (randomNumber == oldNumber);
        
        transform.localPosition = randomPositionTransforms[randomNumber].localPosition;
        oldNumber = randomNumber;
        foreach (ClosePlayerChecker gameObj in closePlayerCheckers)
        {
            gameObj.gameObject.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RandomPosition();
        }
    }
}
