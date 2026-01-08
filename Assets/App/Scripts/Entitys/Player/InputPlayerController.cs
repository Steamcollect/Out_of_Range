using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference m_DashIa;
    [SerializeField] private InputActionReference m_MoveIa;
    public InputActionReference PrimaryAttackIa;
    public InputActionReference SecondaryAttackIa;
    
    public Vector2 GetMoveDirection()
    {
        Vector2 input = m_MoveIa.action.ReadValue<Vector2>();
        return input.normalized;
    }
    
    public bool IsPrimaryAttackPressed()
    {
        return PrimaryAttackIa.action.IsPressed();
    }
    
    public bool IsSecondaryAttackPressed()
    {
        return SecondaryAttackIa.action.IsPressed();
    }
    
    public event Action<InputAction.CallbackContext> OnInputDashPressed;

    private void OnEnable()
    {
        m_DashIa.action.Enable();
        m_MoveIa.action.Enable();
        PrimaryAttackIa.action.Enable();
        SecondaryAttackIa.action.Enable();

        m_DashIa.action.started += Callback_OnDashPressed;
    }
    
    private void Callback_OnDashPressed(InputAction.CallbackContext context) => OnInputDashPressed?.Invoke(context);
    
    private void OnDisable()
    {
        m_DashIa.action.started -= Callback_OnDashPressed;
        
        SecondaryAttackIa.action.Disable();
        PrimaryAttackIa.action.Disable();
        m_DashIa.action.Disable();
        m_MoveIa.action.Disable();
    }
    
}