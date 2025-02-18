using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantItem : InterfacePopUp<MerchantItem>
{
    public Animator treeAnimator;
    
    private void Update()
    {
        if (MerchantManager.Instance.shopActive)
        {
            treeAnimator.SetBool("Open",true);
            return;
        }
        treeAnimator.SetBool("Open",false);
        Destroy(gameObject);
        if (onPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                MerchantManager.Instance.OpenShop();
            }
        }
    }
}