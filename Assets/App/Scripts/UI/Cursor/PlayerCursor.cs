using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class PlayerCursor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_RotationSpeed;
    
    [Header("References")]
    [SerializeField] GameObject m_Content;
    [SerializeField] Transform m_CursorImg;
    [SerializeField] InputActionReference m_MousePositionIA;
    [Space]
    [SerializeField] RSO_CurrentInputDeviceType m_CurrentInputDeviceType;
    [SerializeField] RSO_CameraTargetType m_CameraTargetType;
    
    private void OnEnable()
    {
        Cursor.visible = false;
        m_CameraTargetType.OnChanged += HandleCursorChange;
        m_CurrentInputDeviceType.OnChanged += HandleCursorChange;
    }

    private void OnDisable()
    {
        m_CameraTargetType.OnChanged -= HandleCursorChange;
        m_CurrentInputDeviceType.OnChanged -= HandleCursorChange;
        Cursor.visible = true;
    }

    
    private void HandleCursorChange(InputDeviceType obj) => HandleCursorChange();

    private void HandleCursorChange(CameraTargetType obj) => HandleCursorChange();

    private void HandleCursorChange()
    {
        if (m_CurrentInputDeviceType.Value == InputDeviceType.KeyboardMouse || m_CameraTargetType.Value == CameraTargetType.FreeLook)
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }
    
    
    private void ShowCursor()
    {
        m_Content.SetActive(true);
    }

    private void HideCursor()
    {
        m_Content.SetActive(false);
    }

    private void Update()
    {
        transform.position = m_MousePositionIA.action.ReadValue<Vector2>();
        m_CursorImg.Rotate(Vector3.forward * (m_RotationSpeed * Time.deltaTime));
    }
}