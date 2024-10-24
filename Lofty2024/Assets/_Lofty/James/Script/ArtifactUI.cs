using System;
using System.Collections;
using System.Collections.Generic;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactUI : MonoBehaviour
{
    public TextMeshProUGUI artifactName;
    public Image artifactImage;
    public Sprite defaultImage;

    public void SetArtifactUI(string artifactName,Sprite artifactSprite)
    {
        this.artifactName.text = artifactName;
        artifactImage.sprite = artifactSprite;
    }

    public void ClearArtifactSlot()
    {
        artifactName.text = "Empty";
        artifactImage.sprite = defaultImage;
    }
    
    
    //ทำให้ปุ่มรับค่าแล้วลบขากของ player
}
