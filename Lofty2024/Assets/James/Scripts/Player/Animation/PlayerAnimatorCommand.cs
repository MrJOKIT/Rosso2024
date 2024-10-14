using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorCommand : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    
    public void UnActiveMove()
    {
        playerMovement.canMove = false;
    }

    public void ActiveMove()
    {
        playerMovement.canMove = true;
    }
    
}
