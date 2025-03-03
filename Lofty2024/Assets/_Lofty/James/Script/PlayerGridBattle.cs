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

    [Space(10)] [Header("UI")] public List<GameObject> activeUI;

    public PlayerMode GetPlayerMode
    {
        get { return _playerMode; }
    }
    private void Start()
    {
        switch (_playerMode)
        {
            case PlayerMode.Normal:
                oldMode = _playerMode;
                AppearUI(false);
                break;
            case PlayerMode.Combat: 
                GetComponent<PlayerSkillHandle>().ResetSkillPoint();
                oldMode = _playerMode;
                AppearUI(true);
                break;
        }
        oldMode = _playerMode;
    }

    private void LateUpdate()
    {
        _playerMode = TurnManager.Instance.currentRoomClear ? PlayerMode.Normal : PlayerMode.Combat;
        if (_playerMode == PlayerMode.Combat)
        {
            AppearUI(GetComponent<PlayerMovementGrid>().onTurn);
        }
        if (oldMode == _playerMode)
        {
            return;
        }
        switch (_playerMode)
        {
            case PlayerMode.Normal:
                // if (oldMode == _playerMode)
                // {
                //     return;
                // }
                Debug.Log("Normal");
                GridSpawnManager.Instance.ClearMover();
                AppearUI(false);
                oldMode = _playerMode;
                break;
            case PlayerMode.Combat:
                // if (oldMode == _playerMode)
                // {
                //     return;
                // }
                Debug.Log("Grid Start");
                GetComponent<PlayerSkillHandle>().ResetSkillPoint();
                AppearUI(true);
                oldMode = _playerMode;
                break;
        }
    }

    private void AppearUI(bool appear)
    {
        foreach (GameObject ui in activeUI)
        {
            ui.SetActive(appear);
        }
    }
}
