using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public class Player : MonoBehaviour, ITakeDamage
{
    [SerializeField] private bool haveShield;
    [SerializeField] private int playerHealth;
    [SerializeField] private List<CurseData> curseHave;
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
    }
    
    public void TakeDamage(int damage)
    {
        if (haveShield)
        {
            haveShield = false;
            return;
        }
        playerHealth -= damage;
    }
    
    public void AddCurseStatus(CurseType curseType, int turnTime)
    {
        curseHave.Add(new CurseData(curseType,turnTime));
    }
    
    public void CurseHandle()
    {
        if (curseHave == null)
        {
            return;
        }
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
}
