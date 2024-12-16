using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AbilityOrb : MonoBehaviour
{
    [FormerlySerializedAs("_abilityType")] [SerializeField] private AbilityType abilityType;
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

    public void SetOrbAbility(AbilityType _abilityType)
    {
        abilityType = _abilityType;
        
        foreach (var _iconImage in iconImage)
        {
            _iconImage.sprite = _abilityType switch
            {
                AbilityType.Pawn => pawn,
                AbilityType.Rook => rook,
                AbilityType.Knight => knight,
                AbilityType.Bishop => bishop,
                AbilityType.Queen => queen,
                _ => _iconImage.sprite
            };
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
        _player.swapAbility = abilityType;
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
