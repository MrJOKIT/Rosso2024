using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHearth : MonoBehaviour
{
    public GameObject halfHearth;
    public GameObject fullHearth;
    private void Start()
    {
        PlayerHealth.instance.AddHearth(halfHearth,fullHearth);
    }
}
