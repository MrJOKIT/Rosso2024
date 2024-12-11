using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthData : MonoBehaviour
{
    public TextMeshProUGUI enemyNameText;
    public Image enemyImage;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyDamageText;

    public void SetEnemyData(string name, Sprite sprite,int enemyHealth,int enemyDamage)
    {
        enemyNameText.text = name;
        enemyImage.sprite = sprite;
        enemyHealthText.text = "HP: " + enemyHealth;
        enemyDamageText.text = "DMG: " + enemyDamage;
    }
}
