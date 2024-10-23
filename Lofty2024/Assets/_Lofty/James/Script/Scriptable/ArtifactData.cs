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
    PlayerCurrency,
}

public enum AbilityName
{
    None,
    FreeMoveWithKill,
    TrapNotActiveSelf,
    GodOfWar,
    TheEyeKing,
    DeathDoor,
    GiftOfDeath,
    CheckMate,
    CreepingTerror,
    Kamikaze,
    RabbitPaws,
    IronBody,
    LastChance,
    Shield,
}

public enum ArtifactGrade
{
    Common,
    Rare,
    Epic,
    All,
}

public enum CardClass
{
    Normal,
    SwordKnight,
    BladeMaster,
    ShootingCaster,
}
[CreateAssetMenu(menuName = "Artifact",fileName = "ArtifactData",order = 3)]
public class ArtifactData : ScriptableObject
{
    public ArtifactGrade artifactGrade;
    public CardClass artifactClass;
    [Space(10)]
    [Header("GUI")]
    public string artifactName;
    public Sprite artifactImage;
    [Space(10)] 
    [TextArea] public string artifactDetail;
    [Space(10)] 
    public UpgradeType upgradeType;
    
    [ShowIf("upgradeType",UpgradeType.PlayerStats)]
    [Header("Stats")] 
    public int addHealthPoint;
    public int addHealthPointTemp;
    public int addSkillPoint;
    public int addHealMultiple;
    
    [Space(10)] 
    [ShowIf("upgradeType",UpgradeType.PlayerCurrency)]
    [Header("Currency")] 
    public float addCoinMultiple;
    public float addSoulMultiple;
    [Space(10)] 
    [ShowIf("upgradeType",UpgradeType.PlayerCombat)]
    [Header("Combat")] 
    public int addDamage;
    public int addKnockBackRange;

    [Space(10)] 
    [ShowIf("upgradeType",UpgradeType.PlayerMovement)]
    [Header("Movement")] 
    public int addActionPoint;
    public int addSkillDiscount;

    [Space(10)] 
    [ShowIf("upgradeType",UpgradeType.PlayerAbility)]
    [Header("Ability")] 
    public AbilityName abilityName;
}
