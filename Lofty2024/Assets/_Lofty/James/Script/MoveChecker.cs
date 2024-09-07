using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveChecker : MonoBehaviour
{
    public LayerMask gridLayer;
    public void SetMover()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down,out hit, 10f,gridLayer))
        {
            GridMover gm = hit.collider.GetComponent<GridMover>();
            if (gm != null)
            {
                switch (gm.gridState)
                {
                    case GridState.Empty:
                        gm.gridState = GridState.OnMove;
                        break;
                    case GridState.OnEnemy:
                        gm.ActiveEnemy();
                        break;
                }
            }
        }
    }
}
