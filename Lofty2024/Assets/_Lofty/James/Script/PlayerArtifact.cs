using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using VInspector;

[Serializable]
public class CardArtifact
{
    public ArtifactData artifactData;
    public bool isActivate;
    public CardArtifact(ArtifactData artifactData)
    {
        this.artifactData = artifactData;
    }
}

public enum ClassType
{
    Normal,
    SwordKnight,
    BladeMaster,
    ShootingCaster,
}
public class PlayerArtifact : MonoBehaviour
{
    [Tab("Artifact")]
    [Header("Artifact UI")] public GameObject artifactInventory;
    public PlayableDirector classUnlockDirector;
    public TextMeshProUGUI classTextDirector;
    public bool inventoryActive;

    [Space(10)] [Header("Artifact Data")] public int maxArtifact;
    public List<CardArtifact> artifactHaves;
    public List<ArtifactUI> artifactSlots;

    [Space(20)] 
    public List<ArtifactData> normalType;
    [Space(10)]
    public List<ArtifactData> swordKnightType;
    public List<ArtifactUI> swordKnightSlots;
    [Space(10)]
    public List<ArtifactData> bladeMasterType;
    public List<ArtifactUI> bladeMasterSlots;
    [Space(10)]
    public List<ArtifactData> shootCasterType;
    public List<ArtifactUI> shootCasterSlots;

    [Tab("Upgrade Setting")]
    [Header("Stats")] 
    [SerializeField] private int addHealthPoint;
    [SerializeField] private int addHealthPointTemp;
    [SerializeField] private int addSkillPoint;
    [SerializeField] private int addHealMultiple;
    public int HealthPoint
    {
        get { return addHealthPoint; }
        set { addHealthPoint = value; }
    }

    public int HealthPointTemp
    {
        get { return addHealthPointTemp; }
        set { addHealthPointTemp = value; }
    }

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
    [SerializeField] private bool lastChance;

    [Tab("Class Unlock")] 

    public bool DeathDoor
    {
        get { return deathDoor; }
        set { deathDoor = value; }
    }

    public Image iconBorder;
    public Color swordColor;
    public Color bladeColor;
    public Color shootColor;
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

    [Tab("Class Unlock UI")] 
    public ClassType classType;
    private ClassType oldClassType;
    public GameObject crownObject;
    public Animator crownAnimator;
    public bool firstUnlockSuccess;
    public bool secondUnlockSuccess;
    [Space(10)] 
    public Image playerProfile;
    public Sprite classicProfile;
    public Sprite swordKnightProfile;
    public Sprite bladeMasterProfile;
    public Sprite shootCasterProfile;
    
    [Space(20)] [Tab("Mode Active")] [Space(10)] 
    [Header("Link UI")]
    public TextMeshProUGUI swordKnightTier1; 
    public TextMeshProUGUI swordKnightTier2;
    public TextMeshProUGUI bladeMasterTier1;
    public TextMeshProUGUI bladeMasterTier2;
    public TextMeshProUGUI shootCasterTier1;
    public TextMeshProUGUI shootCasterTier2;
    [Space(10)]
    public Color deActiveColor;
    public Color activeColor;
    [Space(10)] 
    [Header("Sword Knight")] 
    public bool swordKnightPassiveOne;
    public bool swordKnightPassiveTwo;
    [Space(10)]
    [Header("Blade Master")] 
    public bool bladeMasterPassiveOne;
    public bool bladeMasterPassiveTwo;
    [Space(10)]
    [Header("Shoot Caster")]
    public bool shootCasterPassiveOne;
    public bool shootCasterPassiveTwo;
    
    private void Awake()
    {
        maxArtifact = artifactSlots.Count;
    }

    public void AddNewArtifact(ArtifactData newArtifact)
    {
        CardArtifact newCard = new CardArtifact(newArtifact);
        if (artifactHaves.Count == maxArtifact)
        {
            return;
        }

        if (artifactHaves.Contains(newCard))
        {
            return;
        }

        artifactHaves.Add(newCard);
        artifactSlots[artifactHaves.Count - 1]
            .SetArtifactUI( newArtifact.artifactName, newArtifact.artifactImage,newArtifact.artifactDetail);
        //Destroy(newArtifact.GameObject());
        SortingArtifactType(newArtifact);
        ResultArtifact();
    }
    
    public void RemoveArtifact(ArtifactData removeArtifact)
    {
        CardArtifact removeCard = artifactHaves.Find(x=>x.artifactData == removeArtifact);
        if (!artifactHaves.Contains(removeCard))
        {
            return;
        }

        artifactSlots[artifactHaves.IndexOf(removeCard)].ClearArtifactSlot();
        artifactHaves.Remove(removeCard);
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
        foreach (ArtifactUI slots in swordKnightSlots)
        {
            slots.ClearArtifactSlot();
        }
        foreach (ArtifactUI slots in bladeMasterSlots)
        {
            slots.ClearArtifactSlot();
        }
        foreach (ArtifactUI slots in shootCasterSlots)
        {
            slots.ClearArtifactSlot();
        }

        foreach (CardArtifact artifact in artifactHaves)
        {
            artifactSlots[artifactHaves.IndexOf(artifact)]
                .SetArtifactUI(artifact.artifactData.artifactName, artifact.artifactData.artifactImage,artifact.artifactData.artifactDetail);
        }
        
        foreach (ArtifactData artifact in swordKnightType)
        {
            swordKnightSlots[swordKnightType.IndexOf(artifact)]
                .SetArtifactUI(artifact.artifactName, artifact.artifactImage,artifact.artifactDetail);
        }
        foreach (ArtifactData artifact in bladeMasterType)
        {
            bladeMasterSlots[bladeMasterType.IndexOf(artifact)]
                .SetArtifactUI(artifact.artifactName, artifact.artifactImage,artifact.artifactDetail);
        }
        foreach (ArtifactData artifact in shootCasterType)
        {
            shootCasterSlots[shootCasterType.IndexOf(artifact)]
                .SetArtifactUI(artifact.artifactName, artifact.artifactImage,artifact.artifactDetail);
        }
    }

    public void InventoryAppear()
    {
        if (GetComponent<PlayerMovementGrid>().currentState == MovementState.Moving)
        {
            return;
        }
        if (inventoryActive)
        {
            artifactInventory.SetActive(false);
            inventoryActive = false;
            GetComponent<PlayerMovementGrid>().currentState = MovementState.Idle;
        }
        else
        {
            artifactInventory.SetActive(true); 
            inventoryActive = true;
            GetComponent<PlayerMovementGrid>().currentState = MovementState.Freeze;
        }
    }

    private void SortingArtifactType(ArtifactData artifactData)
    {
        switch (artifactData.artifactClass)
        {
            case CardClass.Normal:
                normalType.Add(artifactData);
                break; 
            case CardClass.SwordKnight:
                swordKnightType.Add(artifactData);
                swordKnightSlots[swordKnightType.Count - 1].SetArtifactUI(artifactData.artifactName,artifactData.artifactImage,artifactData.artifactDetail);
                break;
            case CardClass.BladeMaster:
                bladeMasterType.Add(artifactData);
                bladeMasterSlots[bladeMasterType.Count - 1].SetArtifactUI(artifactData.artifactName,artifactData.artifactImage,artifactData.artifactDetail);
                break;
            case CardClass.ShootingCaster:
                shootCasterType.Add(artifactData);
                shootCasterSlots[shootCasterType.Count - 1].SetArtifactUI(artifactData.artifactName,artifactData.artifactImage,artifactData.artifactDetail);
                break;
        }
    }

    private void RemoveByType(ArtifactData removeArtifact)
    {
        switch (removeArtifact.artifactClass)
        {
            case CardClass.Normal:
                normalType.Remove(removeArtifact);
                break;
            case CardClass.SwordKnight:
                swordKnightType.Remove(removeArtifact);
                break;
            case CardClass.BladeMaster:
                bladeMasterType.Remove(removeArtifact);
                break;
            case CardClass.ShootingCaster:
                shootCasterType.Remove(removeArtifact);
                break;
        }
    }

    [Button("Result Artifact")]
    public void ResultArtifact()
    { 
        //SetDefault();
        foreach (CardArtifact artifact in artifactHaves)
        {
            if (artifact.isActivate)
            {
                continue;
            }
            switch (artifact.artifactData.upgradeType)
            {
                case UpgradeType.PlayerCurrency:
                    addSoulMultiple += artifact.artifactData.addSoulMultiple;
                    addCoinMultiple += artifact.artifactData.addCoinMultiple;
                    GameManager.Instance.GetComponent<GameCurrency>().UpgradeMultiple(addCoinMultiple,addSoulMultiple);
                    break;
                case UpgradeType.PlayerAbility:
                    switch (artifact.artifactData.abilityName)
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
                            if (!eyeKing)
                            {
                                GameManager.Instance.GetComponent<RandomCardManager>().StartRandomCardFixGrade(ArtifactGrade.Epic,1);
                                ES3.Save ("EyeKing",true);
                                eyeKing = true;
                            }
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
                            GetComponent<PlayerMovementGrid>().rabbitPaws = true;
                            break;
                        case AbilityName.IronBody:
                            ironBody = true;
                            break;
                        case AbilityName.LastChance:
                            GameManager.Instance.GetComponent<RandomCardManager>().StartRandomCard();
                            lastChance = true;
                            ES3.Save("LastChance",true);
                            GameManager.Instance.GetComponent<RandomCardManager>().haveArtifact = true;
                            break;
                    }
                    break;
                default:
                    addHealthPoint += artifact.artifactData.addHealthPoint;
                    addHealthPointTemp += artifact.artifactData.addHealthPointTemp;
                    addHealMultiple += artifact.artifactData.addHealMultiple;
                    addSkillPoint += artifact.artifactData.addSkillPoint;
                    addDamage += artifact.artifactData.addDamage;
                    addKnockBackRange += artifact.artifactData.addKnockBackRange;
                    addActionPoint += artifact.artifactData.addActionPoint;
                    addSkillDiscount += artifact.artifactData.addSkillDiscount;
                    //GetComponent<Player>().LoadPlayerData();
                    GetComponent<Player>().UpgradeStats();
                    GetComponent<PlayerMovementGrid>().UpgradeStats();
                    break;
            }

            artifact.isActivate = true;
        }
        CardLinkUpdate();
    }

    public void ResetArtifact()
    {
        foreach (CardArtifact artifact in artifactHaves)
        {
            artifact.isActivate = false;
        }

        lastChance = ES3.Load("LastChance", false);
        eyeKing = ES3.Load("EyeKing", false);
        classType = ES3.Load("ClassType", ClassType.Normal);
        firstUnlockSuccess = ES3.Load("FirstClassUnlock", false);
        secondUnlockSuccess = ES3.Load("SecondClassUnlock", false);
        swordKnightPassiveOne = ES3.Load("SwordPassiveOne", false);
        swordKnightPassiveTwo = ES3.Load("SwordPassiveTwo", false);
        bladeMasterPassiveOne = ES3.Load("BladePassiveOne", false);
        bladeMasterPassiveTwo = ES3.Load("BladePassiveTwo", false);
        shootCasterPassiveOne = ES3.Load("ShootPassiveOne", false);
        shootCasterPassiveTwo = ES3.Load("ShootPassiveTwo", false);
        
        for (int i = 0; i < artifactHaves.Count; i++)
        {
            artifactSlots[i].SetArtifactUI(artifactHaves[i].artifactData.artifactName,artifactHaves[i].artifactData.artifactImage,artifactHaves[i].artifactData.artifactDetail);
            SortingArtifactType(artifactHaves[i].artifactData);
            CardLinkUpdate();
        }
    }

    private void CardLinkUpdate()
    {
        //Sword knight
        if (swordKnightType.Count >= 5)
        {
            swordKnightPassiveOne = true;
            swordKnightPassiveTwo = true;
            swordKnightTier1.color = activeColor;
            swordKnightTier2.color = activeColor;
        }
        else if (swordKnightType.Count >= 3)
        {
            swordKnightPassiveOne = true;
            swordKnightTier1.color = activeColor;
        }

        if (swordKnightType.Count < 3)
        {
            swordKnightPassiveOne = false;
            swordKnightPassiveTwo = false;
            swordKnightTier1.color = deActiveColor;
            swordKnightTier2.color = deActiveColor;
        }
        else if (swordKnightType.Count < 5)
        {
            swordKnightPassiveTwo = false;
            swordKnightTier2.color = deActiveColor;
        }
        //Blade Master
        if (bladeMasterType.Count >= 5)
        {
            bladeMasterPassiveOne = true;
            bladeMasterPassiveTwo = true;
            bladeMasterTier1.color = activeColor;
            bladeMasterTier2.color = activeColor;
        }
        else if (bladeMasterType.Count >= 3)
        {
            bladeMasterPassiveOne = true;
            bladeMasterTier1.color = activeColor;
        }

        if (bladeMasterType.Count < 3)
        {
            bladeMasterPassiveOne = false;
            bladeMasterPassiveTwo = false;
            bladeMasterTier1.color = deActiveColor;
            bladeMasterTier2.color = deActiveColor; 
        }
        else if (bladeMasterType.Count < 5)
        {
            bladeMasterPassiveTwo = false;
            bladeMasterTier2.color = deActiveColor;
        }
        //Shoot Caster
        if (shootCasterType.Count >= 5)
        {
            shootCasterPassiveOne = true;
            shootCasterPassiveTwo = true;
            shootCasterTier1.color = activeColor;
            shootCasterTier2.color = activeColor;
        }
        else if (shootCasterType.Count >= 3)
        {
            shootCasterPassiveOne = true;
            shootCasterTier1.color = activeColor;
        }

        if (shootCasterType.Count < 3)
        {
            shootCasterPassiveOne = false;
            shootCasterPassiveTwo = false;
            shootCasterTier1.color = deActiveColor;
            shootCasterTier2.color = deActiveColor;
        }
        else if (shootCasterType.Count < 5)
        {
            shootCasterPassiveTwo = false;
            shootCasterTier2.color = deActiveColor;
        }
        
        CrownClassHandle();
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

    private void CrownClassHandle()
    {
        if (firstUnlockSuccess == false)
        {
            if (swordKnightPassiveOne)
            {
                classUnlockDirector.Play();
                classTextDirector.text = "Sword Knight";
                playerProfile.sprite = swordKnightProfile;
                iconBorder.sprite = swordKnightProfile;
                iconBorder.color = swordColor;
                crownObject.SetActive(true);
                crownAnimator.SetBool("BlueCrown",false);
                crownAnimator.SetBool("RedCrown",false);
                crownAnimator.SetBool("RainbowCrown",false);
                crownAnimator.SetBool("YellowCrown",true);
                
                classType = ClassType.SwordKnight;
                ES3.Save("ClassType",classType);
                
                firstUnlockSuccess = true;
            }
            if (bladeMasterPassiveOne)
            {
                classUnlockDirector.Play();
                classTextDirector.text = "Blade Master";
                playerProfile.sprite = bladeMasterProfile;
                iconBorder.sprite = bladeMasterProfile;
                iconBorder.color = bladeColor;
                crownObject.SetActive(true);
                crownAnimator.SetBool("BlueCrown",false);
                crownAnimator.SetBool("YellowCrown",false);
                crownAnimator.SetBool("RainbowCrown",false);
                crownAnimator.SetBool("RedCrown",true);

                classType = ClassType.BladeMaster;
                ES3.Save("ClassType",classType);
                
                firstUnlockSuccess = true;
            }
            if (shootCasterPassiveOne)
            {
                classUnlockDirector.Play();
                classTextDirector.text = "Shoot Caster";
                playerProfile.sprite = shootCasterProfile;
                iconBorder.sprite = shootCasterProfile;
                iconBorder.color = shootColor;
                crownObject.SetActive(true);
                crownAnimator.SetBool("YellowCrown",false);
                crownAnimator.SetBool("RedCrown",false);
                crownAnimator.SetBool("RainbowCrown",false);
                crownAnimator.SetBool("BlueCrown",true);
                
                classType = ClassType.ShootingCaster;
                ES3.Save("ClassType",classType);
                
                firstUnlockSuccess = true;
            }

            
        }
        else if (secondUnlockSuccess == false)
        {
            if (swordKnightPassiveTwo)
            {
                classUnlockDirector.Play();
                secondUnlockSuccess = true;
            }

            if (bladeMasterPassiveTwo)
            {
                classUnlockDirector.Play();
                secondUnlockSuccess = true;
            }

            if (shootCasterPassiveTwo)
            {
                classUnlockDirector.Play();
                secondUnlockSuccess = true;
            }
        }

        if (oldClassType == classType)
        {
            return;
        }
        switch (classType)
        {
            case ClassType.SwordKnight:
                playerProfile.sprite = swordKnightProfile;
                iconBorder.sprite = swordKnightProfile;
                iconBorder.color = swordColor;
                crownObject.SetActive(true);
                crownAnimator.SetBool("BlueCrown",false);
                crownAnimator.SetBool("RedCrown",false);
                crownAnimator.SetBool("RainbowCrown",false);
                crownAnimator.SetBool("YellowCrown",true);
                break;
            case ClassType.BladeMaster:
                playerProfile.sprite = bladeMasterProfile;
                iconBorder.sprite = bladeMasterProfile;
                iconBorder.color = bladeColor;
                crownObject.SetActive(true);
                crownAnimator.SetBool("BlueCrown",false);
                crownAnimator.SetBool("YellowCrown",false);
                crownAnimator.SetBool("RainbowCrown",false);
                crownAnimator.SetBool("RedCrown",true);
                break;
            case ClassType.ShootingCaster:
                playerProfile.sprite = shootCasterProfile;
                iconBorder.sprite = shootCasterProfile;
                iconBorder.color = shootColor;
                crownObject.SetActive(true);
                crownAnimator.SetBool("YellowCrown",false);
                crownAnimator.SetBool("RedCrown",false);
                crownAnimator.SetBool("RainbowCrown",false);
                crownAnimator.SetBool("BlueCrown",true);
                break;
        }

        oldClassType = classType;

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
