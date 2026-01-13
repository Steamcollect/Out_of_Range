using System;
using MVsToolkit.Dev;
using UnityEngine;

public class CameraTargetHandler : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private float m_FreshRate = 0.1f;
    
    [Header("References")]
    [SerializeField] private InterfaceReference<ICameraTarget> m_CameraTarget;

    [Header("References")]
    [SerializeField] private RSO_PlayerController m_PlayerController;
    
    [Header("Output")]
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;
    
    private Vector3? m_TargetPosition;

    private float m_InternalTimer;
    

    private void Start() => UpdateCameraTarget();

    private void OnEnable()
    {
        m_AimTarget.Set(transform);
    }
    
    public void UpdateCameraTarget()
    {
        transform.position = m_PlayerController.Get().GetTargetPosition();
    }
    
    private void OnDisable()
    {
        m_AimTarget.Set(null);
    }
    
    private void Update()
    {
        m_InternalTimer += Time.deltaTime;
        if (m_InternalTimer < m_FreshRate) return;
        m_InternalTimer = 0f;
        
        m_TargetPosition = m_CameraTarget.Value.GetCameraTargetPosition();
        if (m_TargetPosition.HasValue)
        {
            transform.position = m_TargetPosition.Value;
        }
    }
}