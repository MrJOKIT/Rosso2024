using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

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
    
    public void AttackPlayer()
    {
        enemyHost.targetTransform.GetComponent<Player>().TakeDamage(enemyHost.enemyData.damage);
        VisualEffectManager.Instance.CallEffect(effectName,enemyHost.targetTransform);
    }

    private void Update()
    {
        if (enemyHost.targetTransform.position.x < transform.position.x)
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
        }
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
