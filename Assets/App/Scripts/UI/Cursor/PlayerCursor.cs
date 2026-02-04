using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerCursor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_RotationSpeed;
    
    [Header("References")]
    [SerializeField] VirtualMouseInput m_VirtualMouseInput;
    [SerializeField] Transform m_CursorImg;

    [Header("Input")]
    [SerializeField] InputActionReference m_MousePositionIA;
    [SerializeField] RSE_OnInputDeviceChanged m_OnInputDeviceChanged;

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }


    private void Update()
    {
        transform.position = m_MousePositionIA.action.ReadValue<Vector2>();
        m_CursorImg.Rotate(Vector3.forward * (m_RotationSpeed * Time.deltaTime));
    }
}