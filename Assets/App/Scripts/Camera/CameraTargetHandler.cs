using System;
using MVsToolkit.Dev;
using UnityEngine;

public class CameraTargetHandler : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private float m_FreshRate = 0.1f;
    
    [Header("References")]
    [SerializeField] private InterfaceReference<ICameraTarget> m_CameraTargetAutoFocus;
    [SerializeField] private InterfaceReference<ICameraTarget> m_CameraTargetFreeLook;

    [Header("Input")]
    [SerializeField] private RSO_CameraTargetType m_CameraTargetType;
    
    [Header("Output")]
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;


    private ICameraTarget m_CameraTargetRunning;
    private Vector3? m_TargetPosition;

    private float m_InternalTimer;
    private Vector3 m_Velocity;

    private void Awake()
    {
        HandleCameraTypeChange(CameraTargetType.AutoFocus);
    }

    private void OnEnable()
    {
        m_AimTarget.Set(transform);
        m_CameraTargetType.OnChanged += HandleCameraTypeChange;
    }

    private void HandleCameraTypeChange(CameraTargetType obj)
    {
        switch (obj)
        {
            case CameraTargetType.AutoFocus:
                m_CameraTargetRunning = m_CameraTargetAutoFocus.Value;
                break;
            case CameraTargetType.FreeLook:
                m_CameraTargetRunning = m_CameraTargetFreeLook.Value;
                break;
            default:
                throw new System.NotImplementedException("CameraTargetType not implemented: " + obj);
        }
    }

    private void OnDisable()
    {
        m_AimTarget.Set(null);
        m_CameraTargetType.OnChanged -= HandleCameraTypeChange;
    }
    
    private void Update()
    {
        m_InternalTimer += Time.deltaTime;
        if (m_InternalTimer >= m_FreshRate)
        {
            m_InternalTimer = 0f;
            m_TargetPosition = m_CameraTargetRunning.GetCameraTargetPosition();
        }
        UpdateTargetPosition();
    }
    
    private void UpdateTargetPosition()
    {
        transform.position = Vector3.SmoothDamp(transform.position, m_TargetPosition ?? transform.position, ref m_Velocity, m_FreshRate);
    }
}