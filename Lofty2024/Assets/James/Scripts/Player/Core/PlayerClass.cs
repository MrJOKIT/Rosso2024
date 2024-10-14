using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Class", menuName = "Player/Class")]
public class PlayerClass : ScriptableObject
{
    public string className;
    public float damage;
    public float delayAttack;

    public Sprite artwork;
}
