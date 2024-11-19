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
    public Transform cameraTrans;
    public Enemy enemyHost;
    public EffectName effectName;
    
    void Start()
    {
        initialPosition = cameraTrans.localPosition;
    }
    public void AttackPlayer()
    {
        enemyHost.targetTransform.GetComponent<Player>().TakeDamage(enemyHost.enemyData.damage);
        VisualEffectManager.Instance.CallEffect(effectName,enemyHost.targetTransform);
        enemyHost.EndTurn();
        if (onShake)
        {
            return;
        }

        StartCoroutine(Shake());
    }
    
    IEnumerator Shake()
    {
        onShake = true;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Create a random shake offset
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;

            // Apply the shake to the camera position
            cameraTrans.localPosition = initialPosition + randomOffset;

            // Increment elapsed time
            elapsed += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }
        
        while (elapsed > 0)
        {
            cameraTrans.localPosition = Vector3.Lerp(cameraTrans.localPosition, initialPosition, dampingSpeed * Time.deltaTime);
            elapsed -= Time.deltaTime;

            yield return null;
        }
        
        cameraTrans.localPosition = initialPosition;
        onShake = false;
    }
}
