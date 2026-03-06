using System;
using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargetLookVisual : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float m_RenderRotationTime;
    [Header("References")]
    [SerializeField] private GameObject m_RenderContainer;
    [Space]
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;
    [SerializeField] InputActionReference m_LookAtInput;
    Vector3 m_RenderRotationVelocity;

    private void Update()
    {
        m_RenderContainer.SetActive(m_CurrentInputDeviceType.Get() == InputDeviceType.Gamepad
                && m_LookAtInput.action.ReadValue<Vector2>().sqrMagnitude > .1f);
    }

    public void RotateToward(Vector3 target)
    {        
        m_RenderContainer.transform.LookAtSmoothDamp(target, ref m_RenderRotationVelocity, m_RenderRotationTime);
    }
}