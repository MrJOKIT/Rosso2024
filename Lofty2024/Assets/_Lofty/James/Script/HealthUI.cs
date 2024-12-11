using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public GameObject hearthUI;
    public Material tempHealthMaterial;
    
    public void ActiveHearth(bool active)
    {
        hearthUI.SetActive(active);
    }

    public void ChangeToTemp()
    {
        hearthUI.GetComponent<Image>().material = tempHealthMaterial;
    }
}
