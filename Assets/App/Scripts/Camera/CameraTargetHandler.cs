using MVsToolkit.Dev;
using UnityEngine;

public class CameraTargetHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InterfaceReference<ICameraTarget> m_CameraTarget;

    [Header("Output")]
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;
    
    private Vector3? m_TargetPosition;

    
    private void OnEnable()
    {
        m_AimTarget.Set(transform);
    }
    
    private void OnDisable()
    {
        m_AimTarget.Set(null);
    }
    
    private void Update()
    {
        m_TargetPosition = m_CameraTarget.Value.GetCameraTargetPosition();
        if (m_TargetPosition.HasValue)
        {
            transform.position = m_TargetPosition.Value;
        }
    }
}