using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CheckerData
{
    public bool dontStop;
    public bool checkSucces;
    public List<MoveChecker> checkerOwn;
}
public class MoverCheckerHost : MonoBehaviour
{
    [Space(10)]
    [Header("Check Direction")]
    public List<CheckerData> checkerData;

    public void CheckMove()
    {
        foreach (CheckerData checker in checkerData)
        {
            if (checker.checkSucces)
            {
                continue;
            }
            foreach (MoveChecker mc in checker.checkerOwn.ToList())
            {
                mc.SetMover();
                if (checker.dontStop == false)
                {
                    if (mc.gridCheck == GridState.OnEnemy || mc.gridCheck == GridState.OnObstacle || mc.gridCheck == GridState.Empty)
                    {
                        checker.checkSucces = true;
                        break;
                    }
                }
                
            }
        }
    }
}
