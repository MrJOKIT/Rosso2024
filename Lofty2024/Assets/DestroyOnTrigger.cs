using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    public string tagName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagName))
        {
            Destroy(gameObject);
        }
    }
}
