using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFolders.Libs;

public class MerchantManager : Singeleton<MerchantManager>
{
    public GameObject merchantCanvas;
    public bool shopActive;
    
    
    public void OpenShop()
    {
        merchantCanvas.SetActive(true);
        shopActive = true;
    }

    public void CloseShop()
    {
        merchantCanvas.SetActive(false);
        shopActive = false;
    }
}
