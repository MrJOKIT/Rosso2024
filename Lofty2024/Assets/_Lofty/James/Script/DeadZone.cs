using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Player playerInZone;
    public List<Enemy> enemyInZone;
    public bool onPlayer;
    public bool onEnemy;
    private float timeCounter;
    private void Update()
    {
        if (onEnemy)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > 0.5f)
            {
                foreach (Enemy enemy in enemyInZone.ToList())
                {
                    enemy.TakeDamage(999);
                    enemyInZone.Remove(enemy);
                }

                timeCounter = 0;
                onEnemy = false;
            }
        }
        else if (onPlayer)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > 0.5f)
            {
                playerInZone.TakeDamage(999);
                playerInZone = null;

                timeCounter = 0;
                onPlayer = false;
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

        if (other.CompareTag("Player"))
        {
            onPlayer = true;
            playerInZone = other.GetComponent<Player>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            onEnemy = false;
            timeCounter = 0;
        }
    }
}
