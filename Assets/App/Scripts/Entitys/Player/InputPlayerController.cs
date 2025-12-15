using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference m_DashIa;
    [SerializeField] private InputActionReference m_MoveIa;
    [SerializeField] private InputActionReference m_PrimaryAttackIa;
    [SerializeField] private InputActionReference m_SecondaryAttackIa;
    
    public Vector2 GetMoveDirection()
    {
        Vector2 input = m_MoveIa.action.ReadValue<Vector2>();
        return input.normalized;
    }
    
    public bool IsPrimaryAttackPressed()
    {
        return m_PrimaryAttackIa.action.IsPressed();
    }
    
    public bool IsSecondaryAttackPressed()
    {
        return m_SecondaryAttackIa.action.IsPressed();
    }
    
    public event Action<InputAction.CallbackContext> OnInputDashPressed;

    private void OnEnable()
    {
        m_DashIa.action.Enable();
        m_MoveIa.action.Enable();
        m_PrimaryAttackIa.action.Enable();
        m_SecondaryAttackIa.action.Enable();

        m_DashIa.action.started += Callback_OnDashPressed;
    }
    
    private void Callback_OnDashPressed(InputAction.CallbackContext context) => OnInputDashPressed?.Invoke(context);
    
    private void OnDisable()
    {
        m_DashIa.action.started -= Callback_OnDashPressed;
        
        m_SecondaryAttackIa.action.Disable();
        m_PrimaryAttackIa.action.Disable();
        m_DashIa.action.Disable();
        m_MoveIa.action.Disable();
    }
    
}