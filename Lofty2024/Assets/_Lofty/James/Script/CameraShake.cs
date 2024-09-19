using System.Collections;
using UnityEngine;

public class CameraShake : Singeleton<CameraShake>
{
    public float shakeDuration = 0.5f;
    
    public float shakeMagnitude = 0.2f;
    
    public float dampingSpeed = 1.0f;
    
    private Vector3 initialPosition;

    public bool onShake;
    
    void Start()
    {
        initialPosition = transform.localPosition;
    }

    public void TriggerShake()
    {
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
            transform.localPosition = initialPosition + randomOffset;

            // Increment elapsed time
            elapsed += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }
        
        while (elapsed > 0)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, dampingSpeed * Time.deltaTime);
            elapsed -= Time.deltaTime;

            yield return null;
        }
        
        transform.localPosition = initialPosition;
        onShake = false;
    }
}