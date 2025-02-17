using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    public TextMeshProUGUI maxDamageText;
    public TextMeshProUGUI maxHealthText;
    public TextMeshProUGUI maxActionPointText;

    public void SetStatsText(int maxDamage, int maxHealth, int maxActionPoint)
    {
        maxDamageText.text = maxDamage.ToString();
        maxHealthText.text = maxHealth.ToString();
        maxActionPointText.text = maxActionPoint.ToString();
    }
}
