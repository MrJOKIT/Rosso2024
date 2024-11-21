using System;
using System.Collections;
using EditorAttributes;
using TransitionsPlus;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public enum CameraChangingState
{
    OnIdle,
    OnChanging,
    OnZooming,
}

public enum ZoomType
{
    ZoomIn,
    ZoomOut,
}

public class CameraManager : Singeleton<CameraManager>
{
    [Tab("Shake Setting")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;
    public float dampingSpeed = 1.0f;
    private Vector3 initialPosition;
    public bool onShake;

    [Space(20)] [Header("Changing Camera Setting")]
    public CameraChangingState changingState;
    public bool onTopdown;
    [Space(20)] 
    [Tab("Camera Setting")] 
    public Camera cameraToZoom;
    [Space(20)]
    [Header("Zoom Setting")]
    public ZoomType zoomType;
    public float zoomSpeed = 10f;
    public float minZoom = 2f;
    public float maxZoom = 5f;
    [Space(20)]
    [Header("Follow Setting")]
    public Transform target;
    public Vector3 offset = new Vector3(1.5f, 2, 1.5f);
    public float smoothSpeed = 0.125f;
    
    void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        switch (changingState)
        {
            case CameraChangingState.OnZooming:
                if (zoomType == ZoomType.ZoomIn)
                {
                    Zoom(+1 * zoomSpeed * Time.deltaTime);
                }
                else
                {
                    Zoom(-1 * zoomSpeed * Time.deltaTime);
                }
                
                break;
        }
    }
    
    private void LateUpdate()
    {
        if (onShake)
        {
            return;
        }
        if (target == null) return;

        
        if (onTopdown)
        {
            if (zoomType == ZoomType.ZoomIn)
            {
                Vector3 desiredPosition = target.position; 
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
            else
            {
                Vector3 desiredPosition = target.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
            
        }
        else
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    public void TriggerShake()
    {
        if (onShake)
        {
            return;
        }
        StartCoroutine(Shake());
    }

    public void ChangeCameraView()
    {
        if (changingState == CameraChangingState.OnChanging)
        {
            return;
        }
        if (onTopdown)
        {
            GetComponent<Animator>().SetBool("TopDown",false);
            onTopdown = false;
        }
        else
        {
            GetComponent<Animator>().SetBool("TopDown",true);
            onTopdown = true;
        }

        changingState = CameraChangingState.OnChanging;

    }

    public void ChangeSuccess()
    {
        changingState = CameraChangingState.OnIdle;
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
    
    public void SetCameraTarget(Transform newTarget)
    {
        target = newTarget;
        //initialPosition = newTarget.localPosition;
    }

    
    
    [ContextMenu("Focus Zoom")]
    public void FocusZoom()
    {
        zoomType = ZoomType.ZoomIn;
        if (TurnManager.Instance.turnData[0].isPlayer)
        {
            SetCameraTarget(TurnManager.Instance.turnData[0].unitTransform.GetComponent<Player>().focusTransform);
        }
        else
        {
            SetCameraTarget(TurnManager.Instance.turnData[0].unitTransform.GetComponent<Enemy>().focusTransform);
        }
        
        changingState = CameraChangingState.OnZooming;
    }

    [ContextMenu("UnFocus Zoom")]
    public void UnFocusZoom()
    {
        zoomType = ZoomType.ZoomOut;
        SetCameraTarget(GameManager.Instance.currentRoomPos);
        changingState = CameraChangingState.OnZooming;

    }

    [ContextMenu("Test Zoom")]
    public void TestZoom()
    {
        if (zoomType == ZoomType.ZoomIn)
        {
            zoomType = ZoomType.ZoomOut;
        }
        else
        {
            zoomType = ZoomType.ZoomIn;
        }

        changingState = CameraChangingState.OnZooming;
    }
    
    private void Zoom(float increment)
    {
        if (cameraToZoom != null)
        {
            // Adjust the field of view for perspective cameras
            if (cameraToZoom.orthographic)
            {
                cameraToZoom.orthographicSize = Mathf.Clamp(cameraToZoom.orthographicSize - increment, minZoom, maxZoom);
            }
            else
            {
                cameraToZoom.fieldOfView = Mathf.Clamp(cameraToZoom.fieldOfView - increment, minZoom, maxZoom);
            }


            if (zoomType == ZoomType.ZoomIn)
            {
                if (Mathf.Approximately(cameraToZoom.orthographicSize, minZoom))
                {
                    changingState = CameraChangingState.OnIdle;
                }
            }
            else
            {
                if (Mathf.Approximately(cameraToZoom.orthographicSize, maxZoom))
                {
                    changingState = CameraChangingState.OnIdle;
                }
            }
        }
    }
    
}