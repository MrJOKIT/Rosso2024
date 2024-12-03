using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClickMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement between grid points
    public Vector3 gridSize = new Vector3(1f, 1f, 1f); // Size of each grid cell
    public LayerMask gridLayerMask; // Layer mask to identify the grid cells

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        MoveToTarget();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayerMask))
            {
                Vector3 clickedPosition = hit.point;
                SetTargetPosition(clickedPosition);
            }
        }
    }

    void SetTargetPosition(Vector3 clickedPosition)
    {
        // Round the clicked position to the nearest grid point
        float gridX = Mathf.Round(clickedPosition.x / gridSize.x) * gridSize.x;
        float gridZ = Mathf.Round(clickedPosition.z / gridSize.z) * gridSize.z;

        targetPosition = new Vector3(gridX, transform.position.y, gridZ);
        isMoving = true;
    }

    void MoveToTarget()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }
}
