using UnityEngine;

public sealed class TutorialPanelDynamic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_TutorialPanelGamepad;
    [SerializeField] private GameObject m_TutorialPanelKeyboardMouse;
    [Space]
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;
        
    private void OnEnable()
    {
        m_CurrentInputDeviceType.OnChanged += HandleChangeDevice;
        HandleChangeDevice(m_CurrentInputDeviceType.Value);
    }

    private void HandleChangeDevice(InputDeviceType value)
    {
        switch (value)
        {
            case InputDeviceType.KeyboardMouse:
                m_TutorialPanelGamepad.SetActive(false);
                m_TutorialPanelKeyboardMouse.SetActive(true);
                break;
            case InputDeviceType.Gamepad:
                m_TutorialPanelGamepad.SetActive(true);
                m_TutorialPanelKeyboardMouse.SetActive(false);
                break;
            default:
                m_TutorialPanelGamepad.SetActive(false);
                m_TutorialPanelKeyboardMouse.SetActive(false);
                break;
        }
    }
        
    private void OnDisable()
    {
        m_CurrentInputDeviceType.OnChanged -= HandleChangeDevice;
    }
}