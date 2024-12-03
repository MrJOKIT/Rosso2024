using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRadiusChecker : MonoBehaviour
{
    public bool playerOnRadius;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnRadius = false;
        }
    }
}
