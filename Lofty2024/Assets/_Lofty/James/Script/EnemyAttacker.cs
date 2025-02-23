using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class EnemyAttacker : MonoBehaviour
{
    [Tab("Shake Setting")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;
    public float dampingSpeed = 1.0f;
    private Vector3 initialPosition;
    public bool onShake;
    public Enemy enemyHost;
    public EffectName effectName;
    public CurseType curseType;
    [Range(0,1f)]public float cursePercentage;
    
    public void AttackPlayer()
    {
        enemyHost.TargetTransform.GetComponent<Player>().TakeDamage(enemyHost.EnemyData.damage);
        if (curseType != CurseType.Empty)
        {
            var randomNumber = Random.Range(0, 1f);
            if (randomNumber <= cursePercentage)
            {
                enemyHost.TargetTransform.GetComponent<Player>().AddCurseStatus(curseType,1);
            }
        }
        VisualEffectManager.Instance.CallEffect(effectName,enemyHost.TargetTransform,1f);
        TurnManager.Instance.AddLog(enemyHost.EnemyData.enemyName,enemyHost.TargetTransform.GetComponent<Player>().playerName,LogList.Attacked,false);
    }

    private void Update()
    {
        /*if (enemyHost.targetTransform.position.x < transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (enemyHost.targetTransform.position.x > transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (enemyHost.targetTransform.position.z > transform.position.z)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (enemyHost.targetTransform.position.z < transform.position.z)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }*/
    }

    public void Focus()
    {
        CameraManager.Instance.FocusZoom();
    }

    public void UnFocus()
    {
        CameraManager.Instance.UnFocusZoom();
        enemyHost.EndTurn();
        
    }
}
