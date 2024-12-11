using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStation : InterfacePopUp<HealStation>
{
    public GameObject healCanvas;
    public GameObject vfxObject;
    public GameObject rayObject;
    public int healCost = 20;
    private void Update()
    {
        if (onPlayer)
        {
            healCanvas.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<PlayerMovementGrid>().currentState == MovementState.Combat)
            {
                Player player = GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().playerTrans
                    .GetComponent<Player>();
                if (GameManager.Instance.GetComponent<GameCurrency>().EricCoin >= healCost)
                {
                    close = true;
                    rayObject.SetActive(false);
                    vfxObject.SetActive(true);
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
