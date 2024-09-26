using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfacePopUp<T> : MonoBehaviour where T : Component
{
    public GameObject popUpCanvas;
    public bool onPlayer;

    private void Awake()
    {
        popUpCanvas.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayer = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popUpCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayer = false;
            popUpCanvas.SetActive(false);
        }
    }
}
