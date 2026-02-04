using UnityEngine;

public sealed class HandlerVirtualMouse : MonoBehaviour
{
        [Header("References")]
        [SerializeField] private CustomVirtualMouse m_CustomVirtualMouse;
        [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;
        
        private void OnEnable()
        {
            m_CurrentInputDeviceType.OnChanged += HandleChangeDevice;
            HandleChangeDevice(m_CurrentInputDeviceType.Value);
        }

        private void HandleChangeDevice(InputDeviceType deviceType)
        {
            m_CustomVirtualMouse.enabled = deviceType == InputDeviceType.Gamepad;
        }

        private void OnDisable()
        {
            m_CurrentInputDeviceType.OnChanged -= HandleChangeDevice;
        }
}