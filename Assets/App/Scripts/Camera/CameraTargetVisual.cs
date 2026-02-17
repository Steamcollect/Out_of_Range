using UnityEngine;

public class CameraTargetVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Amplitude = 0.5f;
    [SerializeField] private float m_Frequency = 1f;
    
    [Header("References")]
    [SerializeField] private GameObject m_VisualContainer;
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    public void HandleCameraTarget(ITargetable target)
    {
        m_VisualContainer.SetActive(target != null && m_CurrentInputDeviceType.Get() == InputDeviceType.Gamepad);
        if (target != null)
        {
            m_VisualContainer.transform.position = Vector3.up * (Mathf.Sin(Time.time * m_Frequency) * m_Amplitude) + target.GetTargetIndicatorPosition();
        }
    }
        
}