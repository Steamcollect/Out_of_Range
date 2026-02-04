using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public sealed class DeviceManager : RegularSingleton<DeviceManager>
{
    [Header("Output")]
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    private Coroutine m_PendingChangeDeviceCoroutine;
    
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
            Mouse mouse when mouse.name.Contains("VirtualMouse") => m_CurrentInputDeviceType.Value,
            Keyboard or Mouse => InputDeviceType.KeyboardMouse,
            _ => m_CurrentInputDeviceType.Value
        };

        if (newType == m_CurrentInputDeviceType.Value) return;
        
        if (m_PendingChangeDeviceCoroutine != null)
        {
            StopCoroutine(m_PendingChangeDeviceCoroutine);
        }
        m_PendingChangeDeviceCoroutine = StartCoroutine(HandleChangeDeviceCoroutine(newType));
    }
    
    private IEnumerator HandleChangeDeviceCoroutine(InputDeviceType deviceType)
    {
        yield return new WaitForEndOfFrame();
        m_CurrentInputDeviceType.Value = deviceType;
    }
}