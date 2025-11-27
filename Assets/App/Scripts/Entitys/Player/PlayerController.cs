using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{
    [Space(10)]
    [SerializeField] InputActionReference moveIA;

    [Header("Output")]
    [SerializeField] RSO_PlayerController controller;

    private void Awake()
    {
        controller.Set(this);
        moveIA.action.Enable();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveIA.action.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude <= .1f) return;

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        Vector3 moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        movement.Value.Move(moveDir);
    }
}