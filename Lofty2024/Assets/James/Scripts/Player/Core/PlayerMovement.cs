using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float smoothInputSpeed = 0.2f;
    public bool canMove = true;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;

    [Header("Ref")] 
    public Transform spriteTransform;
    public Animator _animator;
    private PlayerControl playerInputControl;
    private InputAction playerMove;
    private Rigidbody rb;
    private CharacterController _controller;
    private PlayerCombat _playerCombat;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        //_animator = GetComponent<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();
        playerInputControl = new PlayerControl();
        //playerSprite = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        playerMove = playerInputControl.Player.Move;
        playerMove.Enable();
        rb = GetComponent<Rigidbody>();
    }
    private void OnDisable()
    {
        playerMove.Disable();
    }

    private void Update()
    {
        moveDirection = playerMove.ReadValue<Vector3>();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }
        PlayerMove();
    }

    private void PlayerMove()
    {
        currentInputVector =
            Vector3.SmoothDamp(currentInputVector, moveDirection, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 move = new Vector3(currentInputVector.x, 0,currentInputVector.z);
        _controller.Move(move * Time.deltaTime * playerSpeed);
        //transform.position += moveDirection * speed * Time.fixedDeltaTime;
        _animator.SetFloat("X",moveDirection.x);
        _animator.SetFloat("Z",moveDirection.z);
        if (moveDirection.x < 0)
        {
            spriteTransform.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (moveDirection.x > 0)
        {
            spriteTransform.GetComponent<SpriteRenderer>().flipX = false;
        }

    }

    
}
