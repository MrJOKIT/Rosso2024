using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DamageNumbersPro;
using TransitionsPlus;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, ITakeDamage
{
    [Tab("Player")] 
    public string playerName = "ROSSO";
    [Header("Default Stats")] 
    [SerializeField] private int defaultMaxHealth;
    [SerializeField] private int defaultHealthTemp;

    [Space(10)] [Header("Game Stats")] 
    [SerializeField] private bool haveShield;
    [SerializeField] private int maxHealth;
    [SerializeField] private int playerHealthTemp;
    [SerializeField] private int playerHealth;
    public int MaxPlayerHealth => maxHealth;
    public int PlayerHealth => playerHealth;
    [SerializeField] private int takeDamageCount = 0;
    public bool isDead;

    [Space(5)] [Header("GUI")] 
    [Header("Damage Number")] public Transform focusTransform;
    [SerializeField] private Transform damageParent;
    [SerializeField] private DamageNumber damageNumbers;
    [Header("Health")]
    [SerializeField] private Transform healthPrefabUI;
    [SerializeField] private Transform healthParentUI;
    [SerializeField] private Transform healthTempParentUI;
    [SerializeField] private List<HealthUI> healthUI;
    [SerializeField] private List<HealthUI> healthTempUI;
    [Header("Alert")] 
    [SerializeField] private LayerMask enemyLayer;
    public bool enemyForward;
    public bool enemyForwardLeft;
    public bool enemyForwardRight;
    public bool enemyBackward;
    public bool enemyBackwardLeft;
    public bool enemyBackwardRight;
    public bool enemyLeft;
    public bool enemyRight;
    [SerializeField] private GameObject alertObject;
    [SerializeField] private GameObject alertObjectTwo;
    
    [Space(10)] 
    [Tab("Usage Item")] 
    [SerializeField] private int healMultiple;
    [Tab("Curse")]
    [SerializeField] private List<CurseData> curseHave;
    [SerializeField] private GameObject curseUiPrefab;
    [SerializeField] private Transform curseUiParent;
    

    private void Awake()
    {
        LoadPlayerData();
        
    }

    private void Update()
    {
        AlertChecker();
        if (isDead)
        {
            return;
        }
        PlayerDeadHandle();
        CurseHandle();
    }

    private void AlertChecker()
    {
        //Forward Check
        enemyForward = Physics.Raycast(transform.position, Vector3.forward, 1,enemyLayer);
        enemyForwardLeft = Physics.Raycast(transform.position, new Vector3(-1,0,1), 1,enemyLayer);
        enemyForwardRight = Physics.Raycast(transform.position, new Vector3(1, 0, 1), 1,enemyLayer);
       
        //Backward Check
        enemyBackward = Physics.Raycast(transform.position, Vector3.back, 1, enemyLayer);
        enemyBackwardLeft = Physics.Raycast(transform.position, new Vector3(-1, 0, -1), 1, enemyLayer);
        enemyBackwardRight = Physics.Raycast(transform.position, new Vector3(1, 0, -1), 1, enemyLayer);
       
        //Left & Right
        enemyLeft = Physics.Raycast(transform.position, Vector3.left, 1, enemyLayer);
        enemyRight = Physics.Raycast(transform.position, Vector3.right, 1, enemyLayer);


        if (enemyForward || enemyForwardLeft || enemyForwardRight || enemyBackward || enemyBackwardLeft || enemyBackwardRight || enemyLeft || enemyRight)
        {
            alertObject.SetActive(true);
            alertObjectTwo.SetActive(true);   
        }
        else if (!enemyForward && !enemyForwardLeft && !enemyForwardRight && !enemyBackward && !enemyBackwardLeft && !enemyBackwardRight && !enemyLeft && !enemyRight)
        {
            alertObject.SetActive(false);
            alertObjectTwo.SetActive(false);
        }
    }
    

    private void PlayerDeadHandle()
    {
        if (playerHealth <= 0) 
        {
            isDead = true;
            if (GetComponent<PlayerArtifact>().DeathDoor)
            {
                playerHealth = maxHealth;
                CreateHealthUI();
                GetComponent<PlayerArtifact>().DeathDoor = false;
                VisualEffectManager.Instance.CallEffect(EffectName.Revive,transform,2f);
            }
            else
            {
                GetComponent<PlayerMovementGrid>().playerAnimator.SetBool("IsDead",true);
                VisualEffectManager.Instance.CallEffect(EffectName.Dead,transform,1f);
            }
            
        }
    }

    public void PlayerDie()
    {
        //ใช้ตอน Player ตาย 
        TransitionAnimator transitionAnimator = TransitionAnimator.Start(TransitionType.Smear,2f,playDelay:2f);
        transitionAnimator.onTransitionEnd.AddListener(GameManager.Instance.GameOver);
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

    public void ActiveShield()
    {
        haveShield = true;
    }
    
    public void TakeDamage(int damage)
    {
        CameraManager.Instance.TriggerShake();
        
        if (haveShield)
        {
            haveShield = false;
            TurnManager.Instance.AddLog(playerName,"",LogList.Block,true);
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
                TurnManager.Instance.AddLog(GetComponent<Player>().playerName,"",LogList.Evade,true);
                Debug.Log("Miss");
            }
        }
        else
        {
            if (playerHealthTemp > 0)
            {
                int currentHealth = playerHealthTemp - damage;
                
                if (currentHealth <= 0)
                {
                    foreach (HealthUI health in healthTempUI.ToList())
                    {
                        Destroy(health.gameObject);
                        healthTempUI.Remove(health);
                    }
                    playerHealthTemp = 0;
                    playerHealth += currentHealth;
                }
                else
                {
                    Destroy(healthTempUI[^1].gameObject);
                    healthTempUI.Remove(healthTempUI[^1]);
                    playerHealthTemp -= damage;
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

        UpdateHealthUI();
        
    }

    public void TakeHealth(int health)
    {
        VisualEffectManager.Instance.CallEffect(EffectName.Heal,transform,1.5f);
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

    

    public void UpgradeStats()
    {
        defaultHealthTemp = playerHealthTemp + GetComponent<PlayerArtifact>().HealthPointTemp;
        defaultMaxHealth = maxHealth + GetComponent<PlayerArtifact>().HealthPoint;
        maxHealth += GetComponent<PlayerArtifact>().HealthPoint;
        playerHealthTemp += GetComponent<PlayerArtifact>().HealthPointTemp;
        
        CreateHealthUI();
        UpdateHealthUI();
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
            if (a >= maxHealth)
            {
                GameObject health = Instantiate(healthPrefabUI.gameObject, healthTempParentUI);
                healthTempUI.Add(health.GetComponent<HealthUI>());
                health.GetComponent<HealthUI>().ChangeToTemp();
            }
            else
            {
                GameObject health = Instantiate(healthPrefabUI.gameObject, healthParentUI);
                healthUI.Add(health.GetComponent<HealthUI>());
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (playerHealth <= 0)
        {
            foreach (HealthUI health in healthUI)
            {
                health.ActiveHearth(false);
            }

            foreach (HealthUI health in healthTempUI.ToList())
            {
                Destroy(health.gameObject);
                healthTempUI.Remove(health);
            }
        }
        else
        {
            for (int a = 0; a < maxHealth; a++) 
            {
                healthUI[a].ActiveHearth(a < playerHealth);
            }
        }
        
    }

    public void PlayerKnockBack(Transform center,int knockBackRange)
    {
        //forward
        if (center.position.z < transform.position.z && Mathf.Approximately(center.position.x, transform.position.x))
        {
            //knock back forward
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + knockBackRange);
        }
        else if (center.position.z < transform.position.z && center.position.x < transform.position.x)
        {
            //knock back forward left
            transform.position = new Vector3(transform.position.x - knockBackRange, transform.position.y, transform.position.z + knockBackRange);
        }
        else if (center.position.z < transform.position.z && center.position.x > transform.position.x)
        {
            //knock back forward right
            transform.position = new Vector3(transform.position.x + knockBackRange, transform.position.y, transform.position.z + knockBackRange);
        }
        //backward
        if (center.position.z > transform.position.z && Mathf.Approximately(center.position.x, transform.position.x))
        {
            //knock back backward
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - knockBackRange);
        }
        else if (center.position.z > transform.position.z && center.position.x > transform.position.x)
        {
            //knock back backward left
            transform.position = new Vector3(transform.position.x - knockBackRange, transform.position.y, transform.position.z - knockBackRange);
        }
        else if (center.position.z > transform.position.z && center.position.x < transform.position.x)
        {
            //knock back backward right
            transform.position = new Vector3(transform.position.x + knockBackRange, transform.position.y, transform.position.z - knockBackRange);
        }
        
        //left
        if (center.position.x > transform.position.x && Mathf.Approximately(center.position.z, transform.position.z))
        {
            transform.position = new Vector3(transform.position.x - knockBackRange, transform.position.y, transform.position.z);
        }
        //right
        if (center.position.x < transform.position.x && Mathf.Approximately(center.position.z, transform.position.z))
        {
            transform.position = new Vector3(transform.position.x + knockBackRange, transform.position.y, transform.position.z);
        }
    }
    public void SavePlayerData()
    {
        ES3.Save("PlayerDefaultHealth",defaultMaxHealth);
        ES3.Save("PlayerDefaultHealthTemp",defaultHealthTemp);
        ES3.Save("PlayerCurrentHealth",playerHealth);
        ES3.Save("PlayerCurrentHealthTemp",playerHealthTemp);
        
        ES3.Save("PlayerDefaultMovePoint",GetComponent<PlayerMovementGrid>().DefaultMovePoint);
        ES3.Save("PlayerDefaultDamage",GetComponent<PlayerMovementGrid>().DefaultDamage);
        ES3.Save("PlayerDefaultKnockBackRange",GetComponent<PlayerMovementGrid>().DefaultKnockBackRange);
        
        ES3.Save("ArtifactHave",GetComponent<PlayerArtifact>().artifactHaves);
    }

    [Button("Load Data")]
    public void LoadPlayerData()
    {
        GetComponent<PlayerArtifact>().artifactHaves = ES3.Load("ArtifactHave",GetComponent<PlayerArtifact>().artifactHaves);
        GetComponent<PlayerArtifact>().ResultArtifact();
        
        playerHealthTemp = ES3.Load("PlayerDefaultHealthTemp",0) + GetComponent<PlayerArtifact>().HealthPointTemp; 
        maxHealth = ES3.Load("PlayerDefaultHealth",3) + GetComponent<PlayerArtifact>().HealthPoint;

        playerHealthTemp = ES3.Load("PlayerCurrentHealthTemp", playerHealthTemp);
        playerHealth = ES3.Load("PlayerCurrentHealth", maxHealth);

        GetComponent<PlayerMovementGrid>().DefaultMovePoint = ES3.Load("PlayerDefaultMovePoint", 2);
        GetComponent<PlayerMovementGrid>().DefaultDamage = ES3.Load("PlayerDefaultDamage", 1);
        GetComponent<PlayerMovementGrid>().DefaultKnockBackRange = ES3.Load("PlayerDefaultKnockBackRange", 1);
        
        CreateHealthUI();

    }

    [Button("Format Data")]
    public void FormatPlayerData()
    {
        ES3.DeleteKey("PlayerDefaultHealth");
        ES3.DeleteKey("PlayerDefaultHealthTemp");
        ES3.DeleteKey("PlayerCurrentHealth");
        ES3.DeleteKey("PlayerCurrentHealthTemp");
        
        ES3.DeleteKey("PlayerDefaultMovePoint");
        ES3.DeleteKey("PlayerDefaultDamage");
        ES3.DeleteKey("PlayerDefaultKnockBackRange");
        
        ES3.DeleteKey("ArtifactHave");
    }
}
