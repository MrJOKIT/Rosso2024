using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;
using UnityEngine;

public enum LogList
{
    Attacked,
    CriticalAttack,
    GodAttacked,
    Stunned,
    Burn,
    Poison,
    Evade,
    KnockBack,
    Bomb,
}
public class CombatLogSlot : MonoBehaviour
{
    public TextAnimator_TMP logText;
    public void SetLog(string ownerName,string oppositeName,LogList logList,bool isPlayer)
    {
        if (isPlayer)
        {
            logText.SetText($"<color=green>{ownerName} <color=white>{GetLog(logList)} <color=red>{oppositeName}");
        }
        else
        {
            logText.SetText($"<color=red>{ownerName} <color=white>{GetLog(logList)} <color=green>{oppositeName}");
        }
        
    }

    private string GetLog(LogList logList)
    {
        string log = String.Empty;
        switch (logList)
        {
            case LogList.Attacked:
                log = "Attacked";
                break;
            case LogList.CriticalAttack:
                log = "Critical Attack";
                break;
            case LogList.GodAttacked:
                log = "God Attack";
                break;
            case LogList.KnockBack:
                log = "Knockback";
                break;
            case LogList.Stunned:
                log = "Stunned";
                break;
            case LogList.Evade:
                log = "Evade";
                break;
            case LogList.Burn:
                log = "Burn";
                break;
            case LogList.Poison:
                log = "";
                break;
            case LogList.Bomb:
                log = "Bombed";
                break;
        }

        return log;
    }
}
