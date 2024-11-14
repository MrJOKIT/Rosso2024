using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerMovementGrid playerMovementGrid;


    public void Attack()
    {
        playerMovementGrid.AttackEnemy();
    }
}
