using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public CurseType curseType;
    public int curseTurn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().AddCurseStatus(curseType,curseTurn);
            gameObject.SetActive(false);
        }
    }
}
