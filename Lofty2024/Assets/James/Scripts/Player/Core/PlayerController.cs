using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public GameObject playerHUD;

    private void Awake()
    {
        instance = this;
    }

    public void TurnOnPlayerHUD()
    {
        playerHUD.SetActive(true);
    }
    
    public void TurnOffPlayerHUD()
    {
        playerHUD.SetActive(false);
    }
}
