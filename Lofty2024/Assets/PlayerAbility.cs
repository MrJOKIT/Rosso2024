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
    public AbilityType swapAbility;

    [Space(10)] 
    [Header("GUI")] 
    public Button abilityButton;
    public Image abilityImage;
    [Space(5)] 
    public Image swapImage;
    
    [Space(10)] 
    [Header("Skill Image")] 
    public Sprite emptySkillImage;
    public Sprite pawnSkillImage;
    public Sprite rookSkillImage;
    public Sprite knightSkillImage;
    public Sprite bishopSkillImage;
    public Sprite queenSkillImage;
    public Sprite kingSkillImage;

    private void Awake()
    {
        currentAbility = GetComponent<PlayerMovementGrid>().movePattern;
        swapAbility = AbilityType.Empty;
    }

    private void Update()
    {
        AbilityGUI();
        if (currentAbility == AbilityType.Empty)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwapAbility();
        }
    }

    //ต้องกดใช้แล้วเปลื่ยน Pattern ของ Player
    public void ChangeAbility(AbilityType abilityType)
    {
        swapAbility = abilityType;
    }

    public void SwapAbility()
    {
        
        if (swapAbility != AbilityType.Empty)
        {
            GetComponent<PlayerMovementGrid>().ChangePatternNow(swapAbility);
            if (currentAbility == AbilityType.King)
            {
                currentAbility = swapAbility;
                swapAbility = AbilityType.King;
            }
            else
            {
                swapAbility = currentAbility;
                currentAbility = AbilityType.King;
            }
        }
        
    }

    public void CheckAbilityUse()
    {
        if (currentAbility != AbilityType.King)
        {
            currentAbility = AbilityType.King;
            swapAbility = AbilityType.Empty;
        }
    }
    private void AbilityGUI()
    {
        switch (currentAbility)
        {
            case AbilityType.Empty:
                abilityImage.sprite = emptySkillImage;
                break;
            case AbilityType.Pawn:
                abilityImage.sprite = pawnSkillImage;
                break;
            case AbilityType.Rook:
                abilityImage.sprite = rookSkillImage;
                break;
            case AbilityType.Knight:
                abilityImage.sprite = knightSkillImage;
                break;
            case AbilityType.Bishop:
                abilityImage.sprite = bishopSkillImage;
                break;
            case AbilityType.Queen:
                abilityImage.sprite = queenSkillImage;
                break;
            case AbilityType.King:
                abilityImage.sprite = kingSkillImage;
                break;
        }
        
        switch (swapAbility)
        {
            case AbilityType.Empty:
                swapImage.sprite = emptySkillImage;
                break;
            case AbilityType.Pawn:
                swapImage.sprite = pawnSkillImage;
                break;
            case AbilityType.Rook:
                swapImage.sprite = rookSkillImage;
                break;
            case AbilityType.Knight:
                swapImage.sprite = knightSkillImage;
                break;
            case AbilityType.Bishop:
                swapImage.sprite = bishopSkillImage;
                break;
            case AbilityType.Queen:
                swapImage.sprite = queenSkillImage;
                break;
            case AbilityType.King:
                swapImage.sprite = kingSkillImage;
                break;
        }
    }
}
