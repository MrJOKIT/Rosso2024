using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int ericCoin;
    [SerializeField] private int flameSoul;

    public int EricCoin
    {
        get { return ericCoin; }
        set { ericCoin = value; }
    }
    public int FlameSoul
    {
        get { return flameSoul; }
        set { flameSoul = value; }
    }
}
