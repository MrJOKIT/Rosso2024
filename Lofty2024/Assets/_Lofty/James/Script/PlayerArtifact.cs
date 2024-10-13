using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;
using VInspector.Libs;


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
    [SerializeField] private int addSkillPoint;
   
    [Space(10)] 
    [Header("Combat")] 
    [SerializeField] private int addDamage;
    [SerializeField] private int addKnockBackRange;

    [Space(10)] 
    [Header("Movement")] 
    [SerializeField] private int addActionPoint;

    [Space(10)] 
    [Header("Ability")] 
    public bool playerShield;
    public bool moveAfterKill;
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
        ResultArtifact();
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
        ResultArtifact();
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

    private void ResultArtifact()
    {
        SetDefault();
        foreach (ArtifactData artifact in artifactHaves)
        {
            if (artifact.upgradeType != UpgradeType.PlayerAbility)
            {
                addHealthPoint += artifact.addHealthPoint;
                addSkillPoint += artifact.addSkillPoint;
                addDamage += artifact.addDamage;
                addKnockBackRange += artifact.addKnockBackRange;
                addActionPoint += artifact.addActionPoint;
            }
            else
            {
                switch (artifact.abilityName)
                {
                    case AbilityName.Shield:
                        playerShield = true;
                        break;
                    case AbilityName.FreeMoveWithKill:
                        moveAfterKill = true;
                        break;
                }
            }
        }
        
    }

    private void SetDefault()
    {
        //State
        addHealthPoint = 0;
        addSkillPoint = 0;
        addDamage = 0;
        addKnockBackRange = 0;
        addActionPoint = 0;
        
        //Ability
        playerShield = false;
        moveAfterKill = false;
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
