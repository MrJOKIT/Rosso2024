using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera _camera;
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogWarning("No Cam");
                return; 
            }
        }
        Quaternion rotation = _camera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }
}