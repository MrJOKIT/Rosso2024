using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Transform barPoint;
    public RoomType barType;
    public TextMeshProUGUI stageText;
    public Image barImage;
    public Material combatMat;
    public Material bonusMat;
    public Material bossMat;
    
    public void SetBarType(RoomType roomType)
    {
        barType = roomType;
        switch (roomType)
        {
            case RoomType.Combat:
                barImage.material = combatMat;
                break;
            case RoomType.Bonus:
                barImage.material = bonusMat;
                break;
            case RoomType.Boss:
                barImage.material = bossMat;
                break;
        }
    }
}
