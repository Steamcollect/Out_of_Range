using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class DeviceManager : RegularSingleton<DeviceManager>
{
    [Header("Output")]
    [SerializeField] private RSE_OnInputDeviceChanged m_OnInputDeviceChanged;

    private InputDeviceType m_CurrentDeviceType;

    private void OnEnable() => InputSystem.onActionChange += CheckControlChange;
    private void OnDisable() => InputSystem.onActionChange -= CheckControlChange;

    private void CheckControlChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed) return;
    
        if (obj is not InputAction action) return;
    
        InputDevice device = action.activeControl?.device;
        if (device == null) return;
    
        InputDeviceType newType = device switch
        {
            Gamepad => InputDeviceType.Gamepad,
            Mouse mouse when mouse.name.Contains("VirtualMouse") => m_CurrentDeviceType,
            Keyboard or Mouse => InputDeviceType.KeyboardMouse,
            _ => m_CurrentDeviceType
        };

        if (newType == m_CurrentDeviceType) return;
        
        m_OnInputDeviceChanged.Call(newType);
        m_CurrentDeviceType = newType;
    }
}