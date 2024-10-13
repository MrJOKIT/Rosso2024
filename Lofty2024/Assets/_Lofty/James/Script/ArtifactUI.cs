using System.Collections;
using System.Collections.Generic;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactUI : MonoBehaviour
{
    public ArtifactData currenData;
    public string artifactName;
    public Image artifactImage;
    public TooltipTrigger toolTipText;

    public void SetArtifactUI(ArtifactData artifactData,string artifactName,Sprite artifactSprite)
    {
        this.currenData = artifactData;
        this.artifactName = artifactName;
        artifactImage.sprite = artifactSprite;
    }

    public void ClearArtifactSlot()
    {
        artifactName = null;
        artifactImage.sprite = null;
    }
    
    
    //ทำให้ปุ่มรับค่าแล้วลบขากของ player
}
