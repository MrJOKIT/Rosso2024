using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverCheckerHost : MonoBehaviour
{
    public List<MoveChecker> checkerOwn;

    public void CheckMove()
    {
        foreach (MoveChecker mc in checkerOwn)
        {
            mc.SetMover();
        }
    }
}
