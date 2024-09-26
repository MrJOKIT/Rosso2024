using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singeleton<GameManager>
{
    [SerializeField] private Transform playerTransform;

    public Transform GetPlayerTransform
    {
        get { return playerTransform; }
    }
    
    public void AddPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }
}
