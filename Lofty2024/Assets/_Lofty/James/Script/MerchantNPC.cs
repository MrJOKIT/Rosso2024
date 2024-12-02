using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNPC : InterfacePopUp<MerchantNPC>
{
    public Animator treeAnimator;
    public bool onRandom;
    public bool randomSuccess;
    public GameObject rayObject;
    
    private void Update()
    {
        if (GameManager.Instance.GetComponent<RandomCardManager>().isRandom)
        {
            return; 
        }

        if (randomSuccess)
        {
            return;
        }
        if (onRandom)
        {
            rayObject.SetActive(false);
            close = true;
            treeAnimator.SetBool("Open",false);
            randomSuccess = true;
        }
        if (onPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<PlayerMovementGrid>().currentState == MovementState.Combat)
            {
                GameManager.Instance.GetComponent<RandomCardManager>().StartRandomCard();
                onRandom = true;
                treeAnimator.SetBool("Open",true);
            }
        }
    }
}
