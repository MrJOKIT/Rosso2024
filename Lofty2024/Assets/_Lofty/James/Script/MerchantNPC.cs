using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantNPC : InterfacePopUp<MerchantNPC>
{
    public Animator treeAnimator;
    public bool onRandom;
    public bool randomSuccess;
    
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
            treeAnimator.SetBool("Open",false);
            randomSuccess = true;
            Destroy(gameObject);
        }
        if (onPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.Instance.GetComponent<RandomCardManager>().StartRandomCard();
                onRandom = true;
                treeAnimator.SetBool("Open",true);
            }
        }
    }
}
