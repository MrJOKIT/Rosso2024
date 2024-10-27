using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, ITakeDamage
{
    [Tab("Player")] 
    [Header("Default Stats")] 
    [SerializeField] private int defaultMaxHealth;
    [SerializeField] private int defaultHealthTemp;
    [Space(10)]
    [Header("Game Stats")]
    [SerializeField] private bool haveShield;
    [SerializeField] private int maxHealth;
    [SerializeField] private int playerHealthTemp;
    [SerializeField] private int playerHealth;
    [SerializeField] private int takeDamageCount = 0;
    public bool isDead;

    [Space(5)] [Header("GUI")] 
    [SerializeField] private Transform healthPrefabUI;
    [SerializeField] private Transform healthParentUI;
    [SerializeField] private List<HealthUI> healthUI;
    
    [Space(10)] 
    [Tab("Usage Item")] 
    [SerializeField] private int healMultiple;
    [Tab("Curse")]
    [SerializeField] private List<CurseData> curseHave;
    [SerializeField] private GameObject curseUiPrefab;
    [SerializeField] private Transform curseUiParent;
    

    private void Awake()
    {
        SetStats();
        CreateHealthUI();
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }
        PlayerDeadHandle();
        CurseHandle();
    }

    private void PlayerDeadHandle()
    {
        if (playerHealth <= 0) 
        {
            if (GetComponent<PlayerArtifact>().DeathDoor)
            {
                playerHealth = 1;
                GetComponent<PlayerArtifact>().RemoveArtifact(GetComponent<PlayerArtifact>().artifactHaves.Find(x=> x.abilityName == AbilityName.DeathDoor)); 
            }
            else
            {
                PlayerDie();
            }
            
        }
    }

    private void PlayerDie()
    {
        //ใช้ตอน Player ตาย 
        isDead = true;
    }

    [Button("Test Health Up")]
    private void TestAddMaxHealth()
    {
        AddMaxHealth(1);
    }
    private void AddMaxHealth(int count)
    {
        maxHealth += count;
        CreateHealthUI();
    }
    [Button("Test Damage")]
    private void TestTakeDamage()
    {
        TakeDamage(1);
        UpdateHealthUI();
    }

    [Button("Test Heal")]
    private void TestHeal()
    {
        TakeHealth(1);
        UpdateHealthUI();
    }
    
    public void TakeDamage(int damage)
    {
        CameraManager.Instance.TriggerShake();
        
        if (haveShield)
        {
            haveShield = false;
            return;
        }

        if (GetComponent<PlayerArtifact>().swordKnightPassiveOne)
        {
            damage -= 1;
            if (damage < 0)
            {
                damage = 0;
            }
        }

        if (GetComponent<PlayerArtifact>().bladeMasterPassiveTwo)
        {
            float randomNumber = Random.Range(0f, 1f);
            if (randomNumber > 0.25f)
            {
                if (playerHealthTemp > 0)
                {
                    int currentHealth = playerHealthTemp - damage;
                    if (currentHealth < 0)
                    {
                        playerHealth += currentHealth;
                    }
                }
                else
                {
                    playerHealth -= damage;
                    if (playerHealth < 0)
                    {
                        playerHealth = 0;
                    }
                }
            }
            else
            {
                Debug.Log("Miss");
            }
        }
        else
        {
            if (playerHealthTemp > 0)
            {
                int currentHealth = playerHealthTemp - damage;
                
                if (currentHealth < 0)
                {
                    playerHealthTemp = 0;
                    playerHealth += currentHealth;
                }
                else
                {
                    playerHealthTemp -= damage;
                }
                Destroy(healthUI[^1].gameObject);
                healthUI.Remove(healthUI[^1]);
            }
            else
            {
                playerHealth -= damage;
                if (playerHealth < 0)
                {
                    playerHealth = 0;
                }
            }
        }
        

        if (GetComponent<PlayerArtifact>().swordKnightPassiveTwo)
        {
            if (takeDamageCount < 6)
            {
                takeDamageCount += 1;
            }
            else
            {
                Debug.LogWarning("Don't forgot create stun around");
            }
        }

        if (playerHealthTemp <= 0)
        {
            UpdateHealthUI();
        }
        
    }

    public void TakeHealth(int health)
    {
        playerHealth += health + GetComponent<PlayerArtifact>().HealMultiple;
        
        if (playerHealth >= maxHealth + GetComponent<PlayerArtifact>().HealthPoint + playerHealthTemp)
        {
            playerHealth = maxHealth + GetComponent<PlayerArtifact>().HealthPoint;
        }
        UpdateHealthUI();
    }
    
    public void AddCurseStatus(CurseType curseType, int turnTime)
    {
        GameObject curseGUI = Instantiate(curseUiPrefab, curseUiParent);
        curseHave.Add(new CurseData(curseType,turnTime,curseGUI.GetComponent<CurseUI>()));
    }
    
    public void CurseHandle()
    {
        if (curseHave == null)
        {
            return;
        }
        CurseUiUpdate();
        foreach (CurseData curse in curseHave)
        {
            if (curse.curseActivated || GetComponent<PlayerArtifact>().IronBody)
            {
                continue;
            }
            switch (curse.curseType)
            {
                case CurseType.Stun:
                    GetComponent<PlayerMovementGrid>().EndTurn();
                    break;
                case CurseType.Blood:
                    //ลดช่องเดิน 1 ตามั้ง?
                    playerHealth -= 1;
                    break;
                case CurseType.Burn:
                    playerHealth -= 1;
                    break;
                case CurseType.Provoke:
                    //คิดก่อน มีทำไม
                    break;
            }

            curse.curseActivated = true;
        }
    }

    public void CurseUiUpdate()
    {
        foreach (CurseData curse in curseHave)
        {
            curse.curseUI.curseType = curse.curseType;
            curse.curseUI.turnCount.text = curse.curseTurn.ToString();
        }
    }

    public void SetStats()
    {
        playerHealthTemp += defaultHealthTemp + GetComponent<PlayerArtifact>().HealthPointTemp; 
        maxHealth += defaultMaxHealth + GetComponent<PlayerArtifact>().HealthPoint;
        
        LoadData();
    }

    private void CreateHealthUI()
    {
        foreach (HealthUI health in healthUI.ToList())
        {
            Destroy(health.gameObject);
            healthUI.Remove(health);
        }
        for (int a = 0; a < maxHealth + playerHealthTemp; a++)
        {
            GameObject health = Instantiate(healthPrefabUI.gameObject, healthParentUI);
            healthUI.Add(health.GetComponent<HealthUI>());
            if (a >= maxHealth)
            {
                health.GetComponent<HealthUI>().ChangeToTemp();
            }
        }
    }

    private void UpdateHealthUI()
    {
        for (int a = 0; a < maxHealth; a++) 
        {
            healthUI[a].ActiveHearth(a < playerHealth);
        }
    }

    private void SaveData()
    {
        ES3.Save<int>("PlayerHealth", playerHealth);
    }
    private void LoadData()
    {
        playerHealth = ES3.Load<int>("PlayerHealth",maxHealth);
    }

    private void ClearData()
    {
        ES3.DeleteKey("PlayerHealth");
    }
}
