using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    public Enemy enemyHost;
    public void AttackPlayer()
    {
        enemyHost.targetTransform.GetComponent<Player>().TakeDamage(enemyHost.enemyData.damage);
    }
}
