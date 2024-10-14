using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using VInspector;

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
    
    [Space(10)] 
    [Tab("Usage Item")] 
    [SerializeField] private int healMultiple;
    [Tab("Curse")]
    [SerializeField] private List<CurseData> curseHave;
    [SerializeField] private GameObject curseUiPrefab;
    [SerializeField] private Transform curseUiParent;
    private bool isDead;

    private void Awake()
    {
        SetStats();
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
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        //ใช้ตอน Player ตาย 
        isDead = true;
    }
    
    public void TakeDamage(int damage)
    {
        CameraManager.Instance.TriggerShake();
        
        if (haveShield)
        {
            haveShield = false;
            return;
        }

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

    public void TakeHealth(int health)
    {
        playerHealth += health + GetComponent<PlayerArtifact>().HealMultiple;
        
        if (playerHealth >= maxHealth + GetComponent<PlayerArtifact>().HealthPoint + playerHealthTemp)
        {
            playerHealth = maxHealth + GetComponent<PlayerArtifact>().HealthPoint;
        }
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
            if (curse.curseActivated)
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
        
    }
}
