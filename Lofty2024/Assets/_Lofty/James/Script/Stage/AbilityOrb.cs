using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityOrb : MonoBehaviour
{
    [SerializeField] private AbilityType _abilityType;
    [SerializeField] private GameObject uiCanvas;
    private bool onPlayer;
    private PlayerAbility _player;
    public List<SpriteRenderer> iconImage;
    public Sprite pawn;
    public Sprite rook;
    public Sprite knight;
    public Sprite bishop;
    public Sprite queen;

    private void Start()
    {
        GameManager.Instance.currentRoomPos.GetComponent<RoomManager>().AddItemInRoom(gameObject);
    }

    public void SetOrbAbility(AbilityType abilityType)
    {
        _abilityType = abilityType;
        switch (abilityType)
        {
            case AbilityType.Pawn:
                foreach (SpriteRenderer iconImage in this.iconImage)
                {
                    iconImage.sprite = pawn; 
                }
                break;
            case AbilityType.Rook:
                foreach (SpriteRenderer iconImage in this.iconImage)
                {
                    iconImage.sprite = rook;
                }
                break;
            case AbilityType.Knight:
                foreach (SpriteRenderer iconImage in this.iconImage)
                {
                    iconImage.sprite = knight;
                }
                break;
            case AbilityType.Bishop:
                foreach (SpriteRenderer iconImage in this.iconImage)
                {
                    iconImage.sprite = bishop;
                }
                break;
            case AbilityType.Queen:
                foreach (SpriteRenderer iconImage in this.iconImage)
                {
                    iconImage.sprite = queen;
                }
                break;
        }
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
