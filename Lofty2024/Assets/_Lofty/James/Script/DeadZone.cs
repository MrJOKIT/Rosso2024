using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public List<Enemy> enemyInZone;
    public bool onEnemy;
    private float timeCounter;
    private void Update()
    {
        if (onEnemy)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > 0.5f)
            {
                foreach (Enemy enemy in enemyInZone)
                {
                    enemy.TakeDamage(999);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            onEnemy = true;
            enemyInZone.Add(other.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            onEnemy = false;
            timeCounter = 0;
            enemyInZone.Remove(other.GetComponent<Enemy>());
        }
    }
}
