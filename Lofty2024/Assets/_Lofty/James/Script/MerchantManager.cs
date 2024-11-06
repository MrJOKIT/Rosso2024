using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactStock
{
    public ArtifactData artifactData;
    public Button buyButton;
}
public class MerchantManager : Singeleton<MerchantManager>
{
    [Header("GUI")]
    public GameObject merchantCanvas;
    public bool shopActive;
    [Space(10)] 
    [Header("Merchant")] 
    public ArtifactData slotOne;
    public ArtifactData slotTwo;
    public ArtifactData slotThree;
    public ArtifactData slotFour;
    public ArtifactData slotFive;
    
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

    public void BuyArtifact(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
}
