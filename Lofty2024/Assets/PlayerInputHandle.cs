using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInputHandle : MonoBehaviour
{
    private PlayerMovementGrid _playerMovementGrid;
    private PlayerArtifact _artifact;
    
    private void Awake()
    {
        _playerMovementGrid = GetComponent<PlayerMovementGrid>();
        _artifact = GetComponent<PlayerArtifact>();
    }

    void Update()
    {
        InventoryHandle();
    }


    #region Player Movement

    public void HandleInput()
    {
        if (GetComponent<PlayerGridBattle>().GetPlayerMode != PlayerMode.Normal)
        {
            return;
        }

        if (_playerMovementGrid.moveRandom)
        {
            MoveRandomKeyboard();
        }
        else
        {
            if (Input.GetKey(KeyCode.W) && !_playerMovementGrid.moveRightBlock)
            {
                _playerMovementGrid.SetTargetPosition(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.S) && !_playerMovementGrid.moveLeftBlock)
            {
                _playerMovementGrid.SetTargetPosition(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.A) && !_playerMovementGrid.moveForwardBlock)
            {
                _playerMovementGrid.SetTargetPosition(Vector3.forward);
            }
            else if (Input.GetKey(KeyCode.D) && !_playerMovementGrid.moveBackwardBlock)
            {
                _playerMovementGrid.SetTargetPosition(Vector3.back);
            }
        }
        
    }

    public void MoveRandomKeyboard()
    {
        bool onLoop = true;
        do
        {
            int randomNumber = Random.Range(0, 40);
            switch (randomNumber)
            {
                case < 10:
                    if (_playerMovementGrid.canRight)
                    {
                        _playerMovementGrid.SetTargetPosition(Vector3.right);
                        onLoop = false;
                    }
                    break;
                case < 20:
                    if (_playerMovementGrid.canLeft)
                    {
                        _playerMovementGrid.SetTargetPosition(Vector3.left);
                        onLoop = false;
                    }
                    break;
                case < 30:
                    if (_playerMovementGrid.canForward)
                    {
                        _playerMovementGrid.SetTargetPosition(Vector3.forward);
                        onLoop = false;
                    }
                    break;
                case < 40:
                    if (_playerMovementGrid.canBackward)
                    {
                        _playerMovementGrid.SetTargetPosition(Vector3.back);
                        onLoop = false;
                    }
                    break;
            }
        } while (onLoop);
        
    }

    #endregion

    #region Player Inventory

    private void InventoryHandle()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _artifact.InventoryAppear();
        }
    }

    #endregion
}
