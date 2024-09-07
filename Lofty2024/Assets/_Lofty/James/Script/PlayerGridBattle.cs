using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMode
{
    Normal,
    Combat,
}
public class PlayerGridBattle : MonoBehaviour
{
    [SerializeField] private PlayerMode _playerMode;
    private PlayerMode oldMode;

    public PlayerMode GetPlayerMode
    {
        get { return _playerMode; }
    }
    private void Start()
    {
        oldMode = _playerMode;
    }

    private void LateUpdate()
    {
        switch (_playerMode)
        {
            case PlayerMode.Normal:
                if (oldMode == _playerMode)
                {
                    return;
                }
                Debug.Log("Normal");
                GridSpawnManager.Instance.ClearMover();
                oldMode = _playerMode;
                break;
            case PlayerMode.Combat:
                if (oldMode == _playerMode)
                {
                    return;
                }
                Debug.Log("Grid Start");
                oldMode = _playerMode;
                break;
        }
    }
}
