using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

public enum DamageTo
{
    Player,
    Enemy,
    All,
}
public class SkillAction : MonoBehaviour
{

    public DamageTo damageTo;
    [Space(10)]
    [SerializeField] private AttackType skillAttackType;
    [SerializeField] private CurseType curseType;
    [SerializeField] private int damageCount;
    [SerializeField] private int knockBackRange = 1;
    [SerializeField] private GameObject vfx;
    [Space(10)] 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private List<Transform> checkPosition;
    
    [Header("Optional")] 
    [Tooltip("If enable optional checker is active")]public bool optional;
    [SerializeField] private AttackType optionalAttackType;
    [SerializeField] private CurseType optionalCurseType;
    [SerializeField] private int optionalDamageCount;
    [SerializeField] private int optionalKnockBackRange;
    [SerializeField] private List<Transform> optionalChecker;

    public void SetSkillAction(AttackType skillAttackType,CurseType curseType,int damage,int knockBackRange)
    {
        this.skillAttackType = skillAttackType;
        this.curseType = curseType;
        damageCount = damage;
        this.knockBackRange = knockBackRange;
    }

    public void SetSkillActionOptional(AttackType optionalAttackType,CurseType optionalCurseType,int optionalDamage,int optionalKnockBackRange)
    {
        this.optionalAttackType = optionalAttackType;
        this.optionalCurseType = optionalCurseType;
        this.optionalDamageCount = optionalDamage;
        this.optionalKnockBackRange = optionalKnockBackRange;

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
                    if (damageTo == DamageTo.Enemy)
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
                                enemy.GetComponent<EnemyMovementGrid>().KnockBack(transform,knockBackRange);
                                break;
                            case AttackType.EffectiveAttack:
                                enemy.AddCurseStatus(curseType,1);
                                break;
                        }
                    }
                    else if (damageTo == DamageTo.Player)
                    {
                        Player player = enemies.GetComponent<Player>();
                        switch (skillAttackType)
                        {
                            case AttackType.NormalAttack:
                                player.TakeDamage(damageCount);
                                break;
                            case AttackType.SpecialAttack:
                                player.TakeDamage(damageCount * 2);
                                break;
                            case AttackType.KnockBackAttack:
                                player.TakeDamage(damageCount);
                                if (player.PlayerHealth > 0)
                                {
                                    player.GetComponent<Player>().PlayerKnockBack(transform,knockBackRange);
                                }
                                break;
                            case AttackType.EffectiveAttack:
                                player.AddCurseStatus(curseType,1);
                                break;
                        }
                    }
                    else
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
                                enemy.GetComponent<EnemyMovementGrid>().KnockBack(transform,knockBackRange);
                                break;
                            case AttackType.EffectiveAttack:
                                enemy.AddCurseStatus(curseType,1);
                                break;
                        }
                        
                        Player player = enemies.GetComponent<Player>();
                        switch (skillAttackType)
                        {
                            case AttackType.NormalAttack:
                                player.TakeDamage(damageCount);
                                break;
                            case AttackType.SpecialAttack:
                                player.TakeDamage(damageCount * 2);
                                break;
                            case AttackType.KnockBackAttack:
                                player.TakeDamage(damageCount);
                                if (player.PlayerHealth > 0)
                                {
                                    player.GetComponent<Player>().PlayerKnockBack(transform,knockBackRange);
                                }
                                break;
                            case AttackType.EffectiveAttack:
                                player.AddCurseStatus(curseType,1);
                                break;
                        }
                    }
                    Instantiate(vfx, checkPoint.position,Quaternion.identity);
                }
                catch (Exception a)
                {
                    Debug.Log($"No enemy component {a}");
                }
            }
        }

        if (optional)
        {
            foreach (Transform checkPoint in optionalChecker)
            {
                Instantiate(vfx, checkPoint.position,Quaternion.identity);
                Collider[] enemyColliders = Physics.OverlapSphere(checkPoint.position, 0.2f, enemyLayer);
                foreach (Collider enemies in enemyColliders)
                {
                    try
                    {
                        if (damageTo == DamageTo.Enemy)
                        {
                            Enemy enemy = enemies.GetComponent<Enemy>();
                            switch (skillAttackType)
                            {
                                case AttackType.NormalAttack:
                                    enemy.TakeDamage(optionalDamageCount);
                                    break;
                                case AttackType.SpecialAttack:
                                    enemy.TakeDamage(optionalDamageCount * 2);
                                    break;
                                case AttackType.KnockBackAttack:
                                    enemy.TakeDamage(optionalDamageCount);
                                    enemy.GetComponent<EnemyMovementGrid>().KnockBack(transform,optionalKnockBackRange);
                                    break;
                                case AttackType.EffectiveAttack:
                                    enemy.AddCurseStatus(optionalCurseType,1);
                                    break;
                            }
                        }
                        else if (damageTo == DamageTo.Player)
                        {
                            Player player = enemies.GetComponent<Player>();
                            switch (skillAttackType)
                            {
                                case AttackType.NormalAttack:
                                    player.TakeDamage(optionalDamageCount);
                                    break;
                                case AttackType.SpecialAttack:
                                    player.TakeDamage(optionalDamageCount * 2);
                                    break;
                                case AttackType.KnockBackAttack:
                                    player.TakeDamage(optionalDamageCount);
                                    player.GetComponent<EnemyMovementGrid>().KnockBack(transform,optionalKnockBackRange);
                                    break;
                                case AttackType.EffectiveAttack:
                                    player.AddCurseStatus(optionalCurseType,1);
                                    break;
                            }
                        }
                        else
                        {
                            Enemy enemy = enemies.GetComponent<Enemy>();
                            switch (skillAttackType)
                            {
                                case AttackType.NormalAttack:
                                    enemy.TakeDamage(optionalDamageCount);
                                    break;
                                case AttackType.SpecialAttack:
                                    enemy.TakeDamage(optionalDamageCount * 2);
                                    break;
                                case AttackType.KnockBackAttack:
                                    enemy.TakeDamage(optionalDamageCount);
                                    enemy.GetComponent<EnemyMovementGrid>().KnockBack(transform,optionalKnockBackRange);
                                    break;
                                case AttackType.EffectiveAttack:
                                    enemy.AddCurseStatus(optionalCurseType,1);
                                    break;
                            }
                            Player player = enemies.GetComponent<Player>();
                            switch (skillAttackType)
                            {
                                case AttackType.NormalAttack:
                                    player.TakeDamage(optionalDamageCount);
                                    break;
                                case AttackType.SpecialAttack:
                                    player.TakeDamage(optionalDamageCount * 2);
                                    break;
                                case AttackType.KnockBackAttack:
                                    player.TakeDamage(optionalDamageCount);
                                    player.GetComponent<EnemyMovementGrid>().KnockBack(transform,optionalKnockBackRange);
                                    break;
                                case AttackType.EffectiveAttack:
                                    player.AddCurseStatus(optionalCurseType,1);
                                    break;
                            }
                        }
                        
                    }
                    catch (Exception a)
                    {
                        Debug.Log($"No enemy component {a}");
                    }
                }
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        foreach (Transform colliderCheck in checkPosition)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(colliderCheck.position,0.2f);
        }

        foreach (Transform colliderCheck in optionalChecker)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(colliderCheck.position,0.2f);
        }
    }
}
