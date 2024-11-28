using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStation : InterfacePopUp<HealStation>
{
    public GameObject healCanvas;
    public int healCost = 20;
    private void Update()
    {
        if (onPlayer)
        {
            healCanvas.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Player player = GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().playerTrans
                    .GetComponent<Player>();
                if (GameManager.Instance.GetComponent<GameCurrency>().EricCoin >= healCost)
                {
                    GameManager.Instance.GetComponent<GameCurrency>().DecreaseEricCoin(healCost);
                    player.TakeHealth(player.MaxPlayerHealth - player.PlayerHealth);
                    Destroy(gameObject);
                }
                
            }
        }
        else
        {
            healCanvas.SetActive(false);
        }
    }
}
