using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;

public enum LogList
{
    Attacked, CriticalAttack, GodAttacked, Stunned, Burn,
    Poison, Evade, KnockBack, Bomb, Block
}

public class CombatLogSlot : MonoBehaviour
{
    public TextAnimator_TMP logText;
    
    private static readonly Dictionary<LogList, string> logMessages = new()
    {
        { LogList.Attacked, "Attacked" },
        { LogList.CriticalAttack, "Critical Attack" },
        { LogList.GodAttacked, "God Attack" },
        { LogList.KnockBack, "Knockback" },
        { LogList.Stunned, "Stunned" },
        { LogList.Evade, "Evade" },
        { LogList.Burn, "Burn" },
        { LogList.Poison, "Poisoned" },
        { LogList.Bomb, "Bombed" },
        { LogList.Block, "Blocked" }
    };

    public void SetLog(string ownerName, string oppositeName, LogList logList, bool isPlayer)
    {
        if (logText == null || !logMessages.ContainsKey(logList)) return;

        string colorOwner = isPlayer ? "green" : "red";
        string colorOpposite = isPlayer ? "red" : "green";

        logText.SetText($"<color={colorOwner}>{ownerName} <color=white>{logMessages[logList]} <color={colorOpposite}>{oppositeName}");
    }
}