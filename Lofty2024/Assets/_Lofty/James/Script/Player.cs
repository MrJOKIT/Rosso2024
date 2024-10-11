using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using VInspector;

public class Player : MonoBehaviour, ITakeDamage
{
    [Tab("Player")]
    [SerializeField] private bool haveShield;
    [SerializeField] private int maxHealth;
    [SerializeField] private int playerHealth;
    [Space(10)] 
    public List<GameObject> healthUI;
    [Tab("Curse")]
    [SerializeField] private List<CurseData> curseHave;
    [SerializeField] private GameObject curseUiPrefab;
    [SerializeField] private Transform curseUiParent;
    private bool isDead;
    

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

    private void UpdateHealthUI()
    {
        switch (playerHealth)
        {
            case 0:
                healthUI[0].SetActive(false);
                healthUI[1].SetActive(false);
                healthUI[2].SetActive(false);
                break;
            case 1:
                healthUI[0].SetActive(true);
                healthUI[1].SetActive(false);
                healthUI[2].SetActive(false);
                break;
            case 2:
                healthUI[0].SetActive(true); 
                healthUI[1].SetActive(true);
                healthUI[2].SetActive(false);
                break;
            case 3:
                healthUI[0].SetActive(true);
                healthUI[1].SetActive(true);
                healthUI[2].SetActive(true);
                break;
        }
    }
    
    public void TakeDamage(int damage)
    {
        CameraManager.Instance.TriggerShake();
        
        if (haveShield)
        {
            haveShield = false;
            return;
        }
        playerHealth -= damage;
        
        UpdateHealthUI();
    }

    public void TakeHealth(int health)
    {
        if (playerHealth >= maxHealth)
        {
            return;
        }

        playerHealth += health;
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
}
