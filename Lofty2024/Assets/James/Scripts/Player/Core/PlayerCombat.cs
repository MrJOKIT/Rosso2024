using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum PlayerClassType
{
    SwordKnight,
    BladeMaster,
    ShootingCaster,
}
public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;
    [SerializeField] private List<PlayerClass> _playerClassesProfile;
    // Sword Knight = 0
    // Blade Master = 1
    // Shooting Caster = 2
    
    [Header("Attack")] 
    [SerializeField] private PlayerClassType _playerClassType;
    [SerializeField] private float playerDamage;
    [SerializeField] private float attackDelay = 1.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    public bool onCombo;
    
    [Header("UI")] 
    public Image playerFace;

    private float attackValue;
    private float attackTimer;
    private bool onAttacking;
    

    [Header("Ref")]
    private PlayerControl playerInputControl;
    private Animator _animator;
    private void OnEnable()
    {
        _animator = GetComponent<PlayerMovement>()._animator;
        playerInputControl.Enable();
        playerInputControl.Player.Fire.performed += Attack;
        //playerInputControl.Player.Fire.canceled += Attack;
        playerInputControl.Player.Dash.performed += Dash;
        //playerInputControl.Player.Dash.canceled += Dash;
    }
    
    private void Awake()
    {
        instance = this;
        playerInputControl = new PlayerControl();
    }
    
    private void OnDisable()
    {
        playerInputControl.Disable();
        playerInputControl.Player.Fire.performed -= Attack;
        //playerInputControl.Player.Fire.canceled -= Attack;
        playerInputControl.Player.Dash.performed -= Dash;
        //playerInputControl.Player.Dash.canceled -= Dash;
    }

    private void Update()
    {
        OnChangeClass();
        if (onAttacking)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer < 0)
                {
                    onAttacking = false;
                }
            }
        }
    }
    private void Attack(InputAction.CallbackContext context)
    {
        if (onAttacking)
        {
            return;
        }
        if (!onCombo)
        {
            _animator.SetTrigger("AttackOne");
        }
        else
        {
            _animator.SetTrigger("AttackTwo");
            onCombo = false;
        }
        onAttacking = true;
        attackTimer = attackDelay;

    }
    

    private void Dash(InputAction.CallbackContext context)
    {
        //Dash
    }

    private void OnChangeClass()
    {
        switch (_playerClassType)
        {
            case PlayerClassType.SwordKnight:
                playerFace.sprite = _playerClassesProfile[0].artwork;
                playerDamage = _playerClassesProfile[0].damage;
                attackDelay = _playerClassesProfile[0].delayAttack;
                break;
            case PlayerClassType.BladeMaster:
                playerFace.sprite = _playerClassesProfile[1].artwork;
                playerDamage = _playerClassesProfile[1].damage;
                attackDelay = _playerClassesProfile[1].delayAttack;
                break;
            case PlayerClassType.ShootingCaster:
                playerFace.sprite = _playerClassesProfile[2].artwork;
                playerDamage = _playerClassesProfile[2].damage;
                attackDelay = _playerClassesProfile[2].delayAttack;
                break;
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position,attackRadius);
    }
}
