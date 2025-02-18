using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnDeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            gameObject.SetActive(false);
        }
    }
}
