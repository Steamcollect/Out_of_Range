using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera m_Camera;
    [SerializeField] private CinemachineCamera m_CinemachineCamera;
    [Space]
    [SerializeField] private TargetGroupManager m_TargetGroupManager;
    [SerializeField] private CameraTargetHandler m_CameraTargetHandler;
    [Space(10)]
    [SerializeField] private RSO_PlayerController m_PlayerController;
    
    [Header("Output")]
    [SerializeField] private RSO_PlayerCameraController m_PlayerCamera;
    

    private void OnEnable()
    {
        m_PlayerCamera.Set(this);
    }

    private void OnDisable()
    {
        m_PlayerCamera.Set(null);
    }

    public Camera GetCamera() => m_Camera;

    
    private Vector3 m_LastTargetPosition;
    
    public void PreProcessTeleportCamera()
    {
        m_LastTargetPosition = m_TargetGroupManager.GetTargetPosition();
    }
    
    public void TeleportCamera()
    {
        CinemachineBrain activeBrain = CinemachineBrain.GetActiveBrain(0);
        
        CinemachineBrain.UpdateMethods baseUpdateMode = activeBrain.UpdateMethod;
        activeBrain.UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;
        
        m_CameraTargetHandler.UpdateCameraTarget();
        m_TargetGroupManager.UpdateTargetGroup();
        Vector3 targetPosition = m_TargetGroupManager.GetTargetPosition();
        
        m_CinemachineCamera.PreviousStateIsValid = false;
        m_CinemachineCamera.OnTargetObjectWarped(m_TargetGroupManager.transform, targetPosition - m_LastTargetPosition);
        
        activeBrain.ManualUpdate();
        activeBrain.UpdateMethod = baseUpdateMode;
        
    }
}