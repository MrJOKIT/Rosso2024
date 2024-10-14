using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPassiveBar : MonoBehaviour
{
    [Header("Passive")] 
    [Range(0,1)][SerializeField] private float redEnergy = 0.5f;
    private float redMaxEnergy = 1;
    [Range(0,1)][SerializeField] private float blueEnergy;

    [Header("Ref")] 
    public Image icon;
    public Image redBar;
    public Slider iconSlider;
    public Animator animator;

    private void Awake()
    {
        redEnergy = 0.5f;
    }

    private void Update()
    {
        EnergyCalculate();
    }

    public void IncreaseRedEnergy(float count)
    {
        redEnergy += count;
    }

    public void DecreaseRedEnergy(float count)
    {
        redEnergy -= count;
    }

    private void EnergyCalculate()
    {
        blueEnergy = redMaxEnergy - redEnergy;

        redBar.fillAmount = redEnergy;
        iconSlider.value = blueEnergy;

        if (redEnergy >= 0.5f)
        {
            animator.SetBool("IsRed",false);
        }
        else
        {
            animator.SetBool("IsRed",true);
        }
    }
}
