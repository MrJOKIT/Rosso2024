using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public GameObject hearthUI;

    public void ActiveHearth(bool active)
    {
        hearthUI.SetActive(active);
    }
}
