using System;
using MVsToolkit.Dev;
using UnityEngine;

public sealed class CameraTargetHandler : MonoBehaviour
{
    //[Header("Settings")]
    [Header("References")]
    [SerializeField] private InterfaceReference<ICameraTarget> m_CameraTargetAutoFocus;
    [SerializeField] private InterfaceReference<ICameraTarget> m_CameraTargetFreeLook;
    [Space]
    [SerializeField] private CameraTargetVisual m_TargetFocusVisual;
    
    [Header("Input")]
    [SerializeField] private RSO_CameraTargetType m_CameraTargetType;
    [Header("References")]
    [SerializeField] private RSO_PlayerController m_PlayerController;
    
    [Header("Output")]
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;
    
    private ICameraTarget m_CameraTargetRunning;
    private Vector3? m_TargetPosition;
    bool m_IsTargettingSomething = false;

    private void Awake()
    {
        HandleCameraTypeChange(CameraTargetType.AutoFocus);
    }

    private void OnEnable()
    {
        m_AimTarget.Set(transform);
        m_CameraTargetType.OnChanged += HandleCameraTypeChange;
    }

    private void OnDisable()
    {
        m_AimTarget.Set(null);
        m_CameraTargetType.OnChanged -= HandleCameraTypeChange;
    }

    private void Start() => UpdateCameraTarget();

    public void UpdateCameraTarget()
    {
        transform.position = m_PlayerController.Get().GetTargetPosition();
    }    

    private void HandleCameraTypeChange(CameraTargetType type)
    {
        switch (type)
        {
            case CameraTargetType.AutoFocus:
                m_CameraTargetRunning = m_CameraTargetAutoFocus.Value;
                break;
            case CameraTargetType.FreeLook:
                m_CameraTargetRunning = m_CameraTargetFreeLook.Value;
                break;
            default:
                throw new NotImplementedException("CameraTargetType not implemented: " + type);
        }
    }
    
    private void Update()
    {
        m_TargetPosition = m_CameraTargetRunning.GetCameraTargetPosition(ref m_IsTargettingSomething);
        UpdateTargetPosition();
    }
    
    private void UpdateTargetPosition()
    {
        transform.position = m_TargetPosition ?? transform.position;
        m_TargetFocusVisual.HandleCameraTarget(m_CameraTargetRunning.GetCameraTarget());
    }

    public Vector3? GetTargetPosition()
    {
        return m_TargetPosition;
    }

    public bool IsTargetingSomething()
    {
        return m_IsTargettingSomething;
    }
}