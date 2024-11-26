using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealStation : InterfacePopUp<HealStation>
{
    private void Update()
    {
        if (onPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Player player = GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().playerTrans
                    .GetComponent<Player>();
                
                player.TakeHealth(player.MaxPlayerHealth - player.PlayerHealth);
                Destroy(gameObject);
            }
        }
    }
}
