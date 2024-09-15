using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public enum BlessType
{
    Empty,
    Protect,
    AttackBoost1,
    AttackBoost2,
    AttackBoost3,
}
[Serializable]
public class BlessData
{
    [ReadOnly] public BlessType blessType;
    [ReadOnly] public int blessTurn;

    public BlessData(BlessType blessType,int blessTime)
    {
        this.blessType = blessType;
        this.blessTurn = blessTime;
    }
}
public class Player : MonoBehaviour, ITakeDamage
{
    [SerializeField] private int playerHealth;
    [SerializeField] private List<BlessData> blessHave;
    [SerializeField] private List<CurseData> curseHave;
    private void Update()
    {
        PlayerDeadHandle();
        BlessHandle();
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
        playerHealth -= damage;
    }

    public void AddBlessStatus(BlessType blessType,int turnTime)
    {
        blessHave.Add(new BlessData(blessType,turnTime));
    }
    
    public void AddCurseStatus(CurseType curseType, int turnTime)
    {
        curseHave.Add(new CurseData(curseType,turnTime));
    }

    private void BlessHandle()
    {
        if (blessHave == null)
        {
            return;
        }

        foreach (BlessData bless in blessHave)
        {
            switch (bless.blessType)
            {
                case BlessType.Protect:
                    break;
                case BlessType.AttackBoost1:
                    break;
                case BlessType.AttackBoost2:
                    break;
                case BlessType.AttackBoost3:
                    break;
            }
        }
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
