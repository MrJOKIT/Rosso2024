using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthData : MonoBehaviour
{
    public TextMeshProUGUI enemyNameText;
    public Image enemyImage;

    public void SetEnemyData(string name, Sprite sprite)
    {
        enemyNameText.text = name;
        enemyImage.sprite = sprite;
    }
}
