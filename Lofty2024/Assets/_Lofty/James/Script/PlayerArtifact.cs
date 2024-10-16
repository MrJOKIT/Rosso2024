using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;


public class PlayerArtifact : MonoBehaviour
{
    [Tab("Artifact")]
    [Header("Artifact UI")] public GameObject artifactInventory;
    public bool inventoryActive;

    [Space(10)] [Header("Artifact Data")] public int maxArtifact;
    public List<ArtifactData> artifactHaves;
    public List<ArtifactUI> artifactSlots;

    [Space(20)]
    public List<ArtifactData> statsType;
    public List<ArtifactData> combatType;
    public List<ArtifactData> movementType;
    public List<ArtifactData> abilityType;

    [Tab("Upgrade Setting")]
    [Header("Stats")] 
    [SerializeField] private int addHealthPoint;
    [SerializeField] private int addHealthPointTemp;
    [SerializeField] private int addSkillPoint;
    [SerializeField] private int addHealMultiple;
    public int HealthPoint => addHealthPoint;
    public int HealthPointTemp => addHealthPointTemp;
    public int SkillPoint => addSkillPoint;
    public int HealMultiple => addHealMultiple;

    [Header("Currency")] 
    [SerializeField] private float addSoulMultiple;
    [SerializeField] private float addCoinMultiple;
    public float SoulMultiple => addSoulMultiple;
    public float CoinMultiple => addCoinMultiple;
    
    [Space(10)] 
    [Header("Combat")] 
    [SerializeField] private int addDamage;
    [SerializeField] private int addKnockBackRange;
    public int Damage => addDamage;
    public int KnockBackRange => addKnockBackRange;

    [Space(10)] 
    [Header("Movement")] 
    [SerializeField] private int addActionPoint;
    [SerializeField] private int addSkillDiscount;
    public int ActionPoint => addActionPoint;
    public int SkillDiscount => addSkillDiscount;
    
    [Space(10)] 
    [Header("Ability")] 
    [SerializeField] private bool playerShield;
    public bool PlayerShield => playerShield;
    [SerializeField] private bool moveAfterKill;
    public bool MoveAfterKill => moveAfterKill;
    [SerializeField] private bool trapNotActiveSelf;
    public bool TrapNotActiveSelf => trapNotActiveSelf;
    [SerializeField] private bool godOfWar;
    public bool GodOfWar => godOfWar;
    [SerializeField] private bool eyeKing;
    public bool EyeKing => eyeKing;
    [SerializeField] private bool deathDoor;
    public bool DeathDoor => deathDoor;
    [SerializeField] private bool giftOfDeath;
    public bool GiftOfDeath => giftOfDeath;
    [SerializeField] private bool checkMate;
    public bool CheckMate => checkMate;
    [SerializeField] private bool creepingTerror;
    public bool CreepingTerror => creepingTerror;
    [SerializeField] private bool kamikaze;
    public bool Kamikaze => kamikaze;
    [SerializeField] private bool rabbitPaws;
    public bool RabbitPaws => rabbitPaws;
    [SerializeField] private bool ironBody;
    public bool IronBody => ironBody;
    
    private void Awake()
    {
        maxArtifact = artifactSlots.Count;
    }

    public void AddNewArtifact(ArtifactData newArtifact)
    {
        if (artifactHaves.Count == maxArtifact)
        {
            return;
        }

        if (artifactHaves.Contains(newArtifact))
        {
            return;
        }

        artifactHaves.Add(newArtifact);
        artifactSlots[artifactHaves.Count - 1]
            .SetArtifactUI(newArtifact, newArtifact.artifactName, newArtifact.artifactImage);
        //Destroy(newArtifact.GameObject());
        SortingArtifactType(newArtifact);
        
    }
    
    public void RemoveArtifact(ArtifactData removeArtifact)
    {
        if (!artifactHaves.Contains(removeArtifact))
        {
            return;
        }

        artifactSlots[artifactHaves.IndexOf(removeArtifact)].ClearArtifactSlot();
        artifactHaves.Remove(removeArtifact);
        RemoveByType(removeArtifact);
        SortingSlot();
    }

    private void SortingSlot()
    {
        foreach (ArtifactUI slots in artifactSlots)
        {
            slots.ClearArtifactSlot();
        }

        foreach (ArtifactData artifact in artifactHaves)
        {
            artifactSlots[artifactHaves.IndexOf(artifact)]
                .SetArtifactUI(artifact, artifact.artifactName, artifact.artifactImage);
        }
    }

    public void InventoryAppear()
    {
        if (inventoryActive)
        {
            artifactInventory.SetActive(false);
            inventoryActive = false;
        }
        else
        {
            artifactInventory.SetActive(true);
            inventoryActive = true;
        }
    }

    private void SortingArtifactType(ArtifactData artifactData)
    {
        switch (artifactData.upgradeType)
        {
            case UpgradeType.PlayerStats:
                statsType.Add(artifactData);
                break;
            case UpgradeType.PlayerCombat:
                combatType.Add(artifactData);
                break;
            case UpgradeType.PlayerMovement:
                movementType.Add(artifactData);
                break;
            case UpgradeType.PlayerAbility:
                abilityType.Add(artifactData);
                break;
        }
    }

    private void RemoveByType(ArtifactData removeArtifact)
    {
        switch (removeArtifact.upgradeType)
        {
            case UpgradeType.PlayerStats:
                statsType.Remove(removeArtifact);
                break;
            case UpgradeType.PlayerCombat:
                combatType.Remove(removeArtifact);
                break;
            case UpgradeType.PlayerMovement:
                movementType.Remove(removeArtifact);
                break;
            case UpgradeType.PlayerAbility:
                abilityType.Remove(removeArtifact);
                break;
        }
    }

    [Button("Result Artifact")]
    public void ResultArtifact()
    {
        SetDefault();
        foreach (ArtifactData artifact in artifactHaves)
        {
            switch (artifact.upgradeType)
            {
                case UpgradeType.PlayerCurrency:
                    addSoulMultiple += artifact.addSoulMultiple;
                    addCoinMultiple += artifact.addCoinMultiple;
                    GameManager.Instance.GetComponent<GameCurrency>().UpgradeMultiple(addCoinMultiple,addSoulMultiple);
                    break;
                case UpgradeType.PlayerAbility:
                    switch (artifact.abilityName)
                    {
                        case AbilityName.Shield:
                            playerShield = true;
                            break;
                        case AbilityName.FreeMoveWithKill:
                            moveAfterKill = true;
                            break;
                        case AbilityName.TrapNotActiveSelf:
                            trapNotActiveSelf = true;
                            break;
                        case AbilityName.GodOfWar:
                            godOfWar = true;
                            break;
                        case AbilityName.TheEyeKing:
                            if (eyeKing)
                            {
                                return;
                            }
                            Debug.Log("Eye King");
                            GameManager.Instance.GetComponent<RandomCardManager>().StartRandomCardFixGrade(ArtifactGrade.Epic,1);
                            eyeKing = true;
                            break;
                        case AbilityName.DeathDoor:
                            deathDoor = true;
                            break;
                        case AbilityName.GiftOfDeath:
                            giftOfDeath = true;
                            break;
                        case AbilityName.CheckMate:
                            checkMate = true;
                            break;
                        case AbilityName.CreepingTerror:
                            creepingTerror = true;
                            break;
                        case AbilityName.Kamikaze:
                            kamikaze = true;
                            break;
                        case AbilityName.RabbitPaws:
                            rabbitPaws = true;
                            break;
                        case AbilityName.IronBody:
                            ironBody = true;
                            break;
                        case AbilityName.LastChance:
                            GameManager.Instance.GetComponent<RandomCardManager>().haveArtifact = true;
                            break;
                    }
                    break;
                default:
                    addHealthPoint += artifact.addHealthPoint;
                    addHealthPointTemp += artifact.addHealthPointTemp;
                    addHealMultiple += artifact.addHealMultiple;
                    addSkillPoint += artifact.addSkillPoint;
                    addDamage += artifact.addDamage;
                    addKnockBackRange += artifact.addKnockBackRange;
                    addActionPoint += artifact.addActionPoint;
                    addSkillDiscount += artifact.addSkillDiscount;
                    
                    GetComponent<Player>().SetStats();
                    GetComponent<PlayerMovementGrid>().SetStats();
                    break;
            }
        }
        
    }

    private void SetDefault()
    {
        //State 
        addHealthPoint = 0;
        addHealthPointTemp = 0;
        addHealMultiple = 0;
        addSkillPoint = 0;
        addDamage = 0;
        addKnockBackRange = 0;
        addActionPoint = 0;
        addSkillDiscount = 0;
        addCoinMultiple = 0;
        addSoulMultiple = 0;
        
        //Ability
        playerShield = false;
        moveAfterKill = false;
        trapNotActiveSelf = false;
        godOfWar = false;
        deathDoor = false;
        giftOfDeath = false;
        checkMate = false;
        creepingTerror = false;
        kamikaze = false;
        rabbitPaws = false;
        ironBody = false;
        GameManager.Instance.GetComponent<RandomCardManager>().haveArtifact = false;
    }

    public void ActiveStartRoom()
    {
        
    }
    public void ActiveEarlyTurn()
    {
        
    }

    public void ActiveAfterCombat()
    {
        
    }
    public void ActiveEndTurn()
    {
        
    }

}
