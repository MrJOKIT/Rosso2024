using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public enum UpgradeType
{
    PlayerStats,
    PlayerCombat,
    PlayerMovement,
    PlayerAbility,
}

public enum AbilityName
{
    Shield,
    FreeMoveWithKill,
}
[CreateAssetMenu(menuName = "Artifact",fileName = "ArtifactData",order = 3)]
public class ArtifactData : ScriptableObject
{
    [Header("GUI")]
    public string artifactName;
    public Sprite artifactImage;
    [Space(10)] 
    public UpgradeType upgradeType;
    
    [ShowIf("upgradeType",UpgradeType.PlayerStats)]
    [Header("Stats")] 
    public int addHealthPoint;
    public int addSkillPoint;
    [Space(10)] 
    [ShowIf("upgradeType",UpgradeType.PlayerCombat)]
    [Header("Combat")] 
    public int addDamage;
    public int addKnockBackRange;

    [Space(10)] 
    [ShowIf("upgradeType",UpgradeType.PlayerMovement)]
    [Header("Movement")] 
    public int addActionPoint;

    [Space(10)] 
    [ShowIf("upgradeType",UpgradeType.PlayerAbility)]
    [Header("Ability")] 
    public AbilityName abilityName;
}
