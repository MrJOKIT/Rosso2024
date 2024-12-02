using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveChecker : MonoBehaviour
{
    public LayerMask gridLayer;
    public GridState gridCheck;
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
                        TutorialManager.Instance.ActiveTutorial(TutorialName.HowToAttack);
                        gm.ActiveEnemy();
                        break;
                }

                gridCheck = gm.gridState;
            }
        }
    }
}
