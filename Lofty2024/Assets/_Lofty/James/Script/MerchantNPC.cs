using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNPC : InterfacePopUp<MerchantNPC>
{
    private void Update()
    {
        if (GameManager.Instance.GetComponent<RandomCardManager>().isRandom)
        {
            return; 
        }
        if (onPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.Instance.GetComponent<RandomCardManager>().StartRandomCard();
            }
        }
    }
}
