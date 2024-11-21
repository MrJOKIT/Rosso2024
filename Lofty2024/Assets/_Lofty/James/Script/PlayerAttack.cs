using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Player player;
    public PlayerMovementGrid playerMovementGrid;
    public TextRevealer textRevealer;


    public void Attack()
    {
        playerMovementGrid.AttackEnemy();
    }

    public void Dead()
    {
        textRevealer.Reveal();
    }

    public void Focus()
    {
        CameraManager.Instance.FocusZoom();
    }

    public void UnFocus()
    {
        CameraManager.Instance.UnFocusZoom();
    }
}
