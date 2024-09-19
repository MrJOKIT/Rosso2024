using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurseUI : MonoBehaviour
{
    public CurseType curseType;
    public Image curseImage;
    public TextMeshProUGUI turnCount;
    [Space(10)] 
    public Sprite stunImage;
    public Sprite bloodImage;
    public Sprite burnImage;
    public Sprite provokeImage;
    private void FixedUpdate()
    {
        switch (curseType)
        {
            case CurseType.Stun:
                curseImage.sprite = stunImage;
                break;
            case CurseType.Blood:
                curseImage.sprite = bloodImage;
                break;
            case CurseType.Burn:
                curseImage.sprite = burnImage;
                break;
            case CurseType.Provoke:
                curseImage.sprite = provokeImage;
                break;
        }
    }
}
