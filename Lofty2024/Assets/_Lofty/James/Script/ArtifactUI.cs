using System.Collections;
using System.Collections.Generic;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactUI : MonoBehaviour
{
    public string artifactName;
    public Image artifactImage;
    public TooltipTrigger toolTipText;

    public void SetArtifactUI(string artifactName,Sprite artifactSprite)
    {
        this.artifactName = artifactName;
        artifactImage.sprite = artifactSprite;
    }

    public void ClearArtifactSlot()
    {
        artifactName = null;
        artifactImage.sprite = null;
    }
}
