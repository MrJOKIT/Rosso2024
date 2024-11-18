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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CameraManager.Instance.ChangeCameraView();
        }
    }


    #region Player Movement

    public void HandleInput()
    {
        if (GetComponent<PlayerGridBattle>().GetPlayerMode != PlayerMode.Normal ||
            GameManager.Instance.GetComponent<SceneLoading>().loadSucces == false)
        {
            return;
        }

        if (_playerMovementGrid.moveRandom)
        {
            MoveRandomKeyboard();
        }
        else
        {
            if (Input.GetKey(KeyCode.W) && !_playerMovementGrid.rightMoveBlock)
            {
                _playerMovementGrid.SetTargetPosition(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.S) && !_playerMovementGrid.leftMoveBlock)
            {
                _playerMovementGrid.SetTargetPosition(Vector3.left);
            }
            else if (Input.GetKey(KeyCode.A) && !_playerMovementGrid.forwardMoveBlock)
            {
                _playerMovementGrid.SetTargetPosition(Vector3.forward);
            }
            else if (Input.GetKey(KeyCode.D) && !_playerMovementGrid.backwardMoveBlock)
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
            int randomNumber = Random.Range(0, 60);
            switch (randomNumber)
            {
                case < 10:
                    if (!_playerMovementGrid.rightMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.Right);
                        onLoop = false;
                    }
                    break;
                case < 20:
                    if (!_playerMovementGrid.leftMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.Left);
                        onLoop = false;
                    }
                    break;
                case < 30:
                    if (!_playerMovementGrid.forwardMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.Forward);
                        onLoop = false;
                    }
                    break;
                case < 40:
                    if (!_playerMovementGrid.backwardMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.Backward);
                        onLoop = false;
                    }
                    break;
                case < 50:
                    if (!_playerMovementGrid.forwardLeftMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.ForwardLeft);
                        onLoop = false;
                    }
                    break;
                case < 60:
                    if (!_playerMovementGrid.forwardRightMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.ForwardRight);
                        onLoop = false;
                    }
                    break;
                case < 70:
                    if (!_playerMovementGrid.backwardLeftMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.BackwardLeft);
                        onLoop = false;
                    }
                    break;
                case < 80:
                    if (!_playerMovementGrid.backwardRightMoveBlock)
                    {
                        _playerMovementGrid.SetPlayerMoveDirection(PlayerMoveDirection.BackwardRight);
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
