using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using TMPro;
using UnityEngine;

public class GameCurrency : MonoBehaviour
{
    [Header("GUI")] 
    public TextMeshProUGUI flameSoulText;
    public TextMeshProUGUI ericCoinText;
    public float defaultTextSpeed = 10;
    private float textSpeed;
    
    [Space(10)]
    [Header("Currency")]
    [SerializeField] private int maxFlameSoul;
    [SerializeField] private float flameSoulMultiple;
    private float flameSoul;
    [SerializeField] private int maxEricCoin;
    [SerializeField] private float ericCoinMultiple;
    private float ericCoin;
    
    public int FlameSoul => maxFlameSoul;
    public int EricCoin => maxEricCoin;

    private void Awake()
    {
        flameSoulText.text = flameSoul.ToString();
        ericCoinText.text = ericCoin.ToString();
    }

    private void LateUpdate()
    {
        CurrencyInterfaceUpdate();
    }
    private void CurrencyInterfaceUpdate()
    {
        if (flameSoul < maxFlameSoul)
        {
            textSpeed = maxFlameSoul - flameSoul;
            if (textSpeed < defaultTextSpeed)
            {
                textSpeed = defaultTextSpeed;
            }
            
            flameSoul += textSpeed * Time.deltaTime;
            flameSoulText.text = Convert.ToInt32(flameSoul).ToString();
            if (flameSoul > maxFlameSoul)
            {
                flameSoul = maxFlameSoul;
            }
        }
        else if (flameSoul > maxFlameSoul)
        {
            textSpeed = flameSoul - maxFlameSoul;
            if (textSpeed < defaultTextSpeed)
            {
                textSpeed = defaultTextSpeed;
            }
            
            flameSoul -= textSpeed * Time.deltaTime;
            flameSoulText.text = Convert.ToInt32(flameSoul).ToString();
            if (flameSoul < maxFlameSoul)
            {
                flameSoul = maxFlameSoul;
            }
        }

        if (ericCoin < maxEricCoin) 
        {
            textSpeed = maxEricCoin - ericCoin;
            if (textSpeed < defaultTextSpeed)
            {
                textSpeed = defaultTextSpeed;
            }
            
            ericCoin += textSpeed * Time.deltaTime;
            ericCoinText.text = Convert.ToInt32(ericCoin).ToString();
            if (ericCoin > maxEricCoin)
            {
                ericCoin = maxEricCoin;
            }
        }
        else if (ericCoin > maxEricCoin)
        {
            textSpeed = ericCoin - maxEricCoin;
            if (textSpeed < defaultTextSpeed)
            {
                textSpeed = defaultTextSpeed;
            }
            
            ericCoin -= textSpeed * Time.deltaTime;
            ericCoinText.text = Convert.ToInt32(ericCoin).ToString();
            if (ericCoin < maxEricCoin)
            {
                ericCoin = maxEricCoin;
            }
        }
    }
    
    [Button("Increase Eric Coin")]
    public void IncreaseEricCoin(int count)
    {
        maxEricCoin += count;
    }
    [Button("Decrease Eric Coin")]
    public void DecreaseEricCoin(int count)
    {
        maxEricCoin -= count;
        if (maxEricCoin < 0)
        {
            maxEricCoin = 0;
        }
    }
    [Button("Increase Flame Soul")]
    public void IncreaseFlameSoul(int count)
    {
        maxFlameSoul += count;
    }
    [Button("Decrease Flame Soul")]
    public void DecreaseFlameSoul(int count)
    {
        maxFlameSoul -= count;
        if (maxFlameSoul < 0)
        {
            maxFlameSoul = 0;
        }
    }

    public void UpgradeMultiple(float ericCoinMultiple,float flameSoulMultiple)
    {
        this.ericCoinMultiple = ericCoinMultiple;
        this.flameSoulMultiple = flameSoulMultiple;
    }
}
