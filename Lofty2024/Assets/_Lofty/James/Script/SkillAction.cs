using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAction : MonoBehaviour
{
    [SerializeField] private AttackType skillAttackType;
    [SerializeField] private CurseType curseType;
    [SerializeField] private int damageCount;
    [Space(10)] 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private List<Transform> checkPosition;

    public void SetSkillAction(AttackType skillAttackType,CurseType curseType,int damage)
    {
        this.skillAttackType = skillAttackType;
        this.curseType = curseType;
        damageCount = damage;
    }

    public void ActiveSkill()
    {
        foreach (Transform checkPoint in checkPosition)
        {
            Collider[] enemyColliders = Physics.OverlapSphere(checkPoint.position, 0.2f, enemyLayer);
            foreach (Collider enemies in enemyColliders)
            {
                try
                {
                    Enemy enemy = enemies.GetComponent<Enemy>();
                    switch (skillAttackType)
                    {
                        case AttackType.NormalAttack:
                            enemy.TakeDamage(damageCount);
                            break;
                        case AttackType.SpecialAttack:
                            enemy.TakeDamage(damageCount * 2);
                            break;
                        case AttackType.KnockBackAttack:
                            enemy.TakeDamage(damageCount);
                            enemy.GetComponent<EnemyMovementGrid>().KnockBack(transform,1);
                            break;
                        case AttackType.EffectiveAttack:
                            enemy.AddCurseStatus(curseType,1);
                            break;
                    }
                }
                catch (Exception a)
                {
                    Debug.Log($"No enemy component {a}");
                }
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        foreach (Transform colliderCheck in checkPosition)
        {
            Gizmos.DrawWireSphere(colliderCheck.position,0.2f);
        }
    }
}
