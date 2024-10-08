using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityOrb : MonoBehaviour
{
    [SerializeField] private AbilityType _abilityType;
    [SerializeField] private GameObject uiCanvas;
    private bool onPlayer;
    private PlayerAbility _player;

    private void Start()
    {
        GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().AddItemInRoom(gameObject);
    }

    public void SetOrbAbility(AbilityType abilityType)
    {
        _abilityType = abilityType;
    }

    private void LateUpdate()
    {
        if (onPlayer == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeOrb();
        }
    }

    private void TakeOrb()
    {
        _player.swapAbility = _abilityType;
        GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().itemInRoom.Remove(gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.GetComponent<Player>() != null)
        {
            _player = other.GetComponent<PlayerAbility>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            uiCanvas.SetActive(true);
            onPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            uiCanvas.SetActive(false);
            onPlayer = false;
        }
    }
}
