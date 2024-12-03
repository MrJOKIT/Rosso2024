using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePlayerChecker : MonoBehaviour
{ 
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("DeadZone"))
        {
            gameObject.SetActive(false);
        }
    }
    
}
