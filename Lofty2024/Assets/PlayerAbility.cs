using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AbilityType
{
    Empty,
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King,
}
public class PlayerAbility : MonoBehaviour
{
    [Header("Ability Point")] 
    [SerializeField] private int abilityPoint;
    
    [Space(10)]
    [Header("Ability Setting")]
    public AbilityType currentAbility;

    [Space(10)] 
    [Header("GUI")] 
    public Image abilityImage;

    private void Update()
    {
        
    }

    //ต้องกดใช้แล้วเปลื่ยน Pattern ของ Player
    public void ChangeAbility(AbilityType abilityType)
    {
        currentAbility = abilityType;
    }

    private void AbilityGUI()
    {
        
    }
}
