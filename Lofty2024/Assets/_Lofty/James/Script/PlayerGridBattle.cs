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
                Debug.Log("Normal");
                oldMode = _playerMode;
                AppearUI(false);
                break;
            case PlayerMode.Combat:
                Debug.Log("Grid Start");
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
        
        if (oldMode == _playerMode)
        {
            return;
        }
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
                AppearUI(false);
                break;
            case PlayerMode.Combat:
                if (oldMode == _playerMode)
                {
                    return;
                }
                Debug.Log("Grid Start");
                GetComponent<PlayerSkillHandle>().ResetSkillPoint();
                oldMode = _playerMode;
                AppearUI(true);
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
