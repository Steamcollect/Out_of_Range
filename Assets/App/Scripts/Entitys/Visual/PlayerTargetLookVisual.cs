using System;
using MVsToolkit.Utilities;
using UnityEngine;

public class PlayerTargetLookVisual : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float m_RenderRotationTime;
    [Header("References")]
    [SerializeField] private GameObject m_RenderContainer;
    [Space]
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    private Vector3 m_RenderRotationVelocity;

    private void OnEnable()
    {
        m_CurrentInputDeviceType.OnChanged += HandleInputDeviceChange;
        HandleInputDeviceChange(m_CurrentInputDeviceType.Get());
    }
        
    private void OnDisable() => m_CurrentInputDeviceType.OnChanged -= HandleInputDeviceChange;

    private void HandleInputDeviceChange(InputDeviceType type)
    {
        switch (type)
        {
            case InputDeviceType.Gamepad:
                m_RenderContainer.SetActive(true);
                break;
            case InputDeviceType.KeyboardMouse:
                m_RenderContainer.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
        
    public void RotateToward(Vector3 target)
    {
        m_RenderContainer.transform.LookAtSmoothDamp(target, ref m_RenderRotationVelocity, m_RenderRotationTime);
    }
}