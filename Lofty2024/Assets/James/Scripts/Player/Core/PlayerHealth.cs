using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    [SerializeField] private float playerMaxHealth;
    [SerializeField] private float playerHealth;
    public List<GameObject> hearthIcon;

    private void Awake()
    {
        instance = this;
        playerHealth = playerMaxHealth;
    }
    public void AddHearth(GameObject halfHearth,GameObject fullHearth)
    {
        hearthIcon.Add(fullHearth);
        hearthIcon.Add(halfHearth);
        if (hearthIcon.Count / 2 == playerHealth )
        {
            //hearthIcon.Reverse();
        }
    }
    public void ReceivedDamage(float damage)
    {
        if (playerHealth == 0)
        {
            return;
        }
        playerHealth -= damage;
        var currentHealth = playerHealth * 2;
        hearthIcon[Convert.ToInt32(currentHealth)].SetActive(false);
    }

    public void ReceivedHealth(float health)
    {
        if (playerHealth == playerMaxHealth)
        {
            return;
        }
        var currentHealth = playerHealth * 2;
        hearthIcon[Convert.ToInt32(currentHealth)].SetActive(true);
        playerHealth += health;
    }
    [Button("Test Received Damage 0.5")] 
    private void TestReceivedDamage()
    {
        if (playerHealth == 0)
        {
            return;
        }
        playerHealth -= 0.5f;
        var currentHealth = playerHealth * 2;
        hearthIcon[Convert.ToInt32(currentHealth)].SetActive(false);
    }
    [Button("Test Received Health 0.5")] 
    private void ReceivedHealth()
    {
        if (playerHealth == playerMaxHealth)
        {
            return;
        }
        var currentHealth = playerHealth * 2;
        hearthIcon[Convert.ToInt32(currentHealth)].SetActive(true);
        playerHealth += 0.5f;
    }
}
