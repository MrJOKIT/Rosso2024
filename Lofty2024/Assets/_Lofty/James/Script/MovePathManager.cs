using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePathManager : MonoBehaviour
{
    [Header("Forward")]
    public GameObject forwardPath;
    [Header("Forward Left")]
    public GameObject forwardLeftPath;
    [Header("Forward Right")]
    public GameObject forwardRightPath;
    [Header("Backward")]
    public GameObject backwardPath;
    [Header("Backward Left")]
    public GameObject backwardLeftPath;
    [Header("Backward Right")]
    public GameObject backwardRightPath;
    [Header("Left")]
    public GameObject leftPath;
    [Header("Right")]
    public GameObject rightPath;

    public void SetPath(PlayerMoveDirection direction)
    {
        /*switch (direction)
        {
            case PlayerMoveDirection.Forward:
                forwardPath.SetActive(false);
                break;
            case PlayerMoveDirection.ForwardLeft:
                forwardLeftPath.SetActive(false);
                break;
            case PlayerMoveDirection.ForwardRight:
                forwardRightPath.SetActive(false);
                break;
            case PlayerMoveDirection.Backward:
                backwardPath.SetActive(false);
                break;
            case PlayerMoveDirection.BackwardLeft:
                backwardLeftPath.SetActive(false);
                break;
            case PlayerMoveDirection.BackwardRight:
                backwardRightPath.SetActive(false);
                break;
            case PlayerMoveDirection.Left:
                leftPath.SetActive(false);
                break;
            case PlayerMoveDirection.Right:
                rightPath.SetActive(false);
                break;
        }*/
    }
}
