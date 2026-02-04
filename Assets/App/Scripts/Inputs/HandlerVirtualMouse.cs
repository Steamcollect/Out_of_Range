using UnityEngine;

public sealed class HandlerVirtualMouse : MonoBehaviour
{
        [Header("References")]
        [SerializeField] private CustomVirtualMouse m_CustomVirtualMouse;
        [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;
        [SerializeField] private RSO_CameraTargetType m_CameraTargetType;
        
        private void OnEnable()
        {
            m_CurrentInputDeviceType.OnChanged += HandleChangeDevice;
            m_CameraTargetType.OnChanged += HandleChangeDevice;
            HandleChangeDevice(m_CurrentInputDeviceType.Value);
        }

        private void HandleChangeDevice(CameraTargetType _)
        {
            HandleChangeDevice();
        }

        private void HandleChangeDevice(InputDeviceType _)
        {
            HandleChangeDevice();
        }
        
        private void HandleChangeDevice()
        {
            m_CustomVirtualMouse.enabled = m_CurrentInputDeviceType.Value == InputDeviceType.Gamepad && m_CameraTargetType.Value == CameraTargetType.FreeLook;
        }

        private void OnDisable()
        {
            m_CameraTargetType.OnChanged -= HandleChangeDevice;
            m_CurrentInputDeviceType.OnChanged -= HandleChangeDevice;
        }
}