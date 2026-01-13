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

    
    private void Start() => TeleportCamera();

    private void TeleportCamera()
    {
        var updateMode = CinemachineBrain.GetActiveBrain(0).UpdateMethod;
        CinemachineBrain.GetActiveBrain(0).UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;
        
        Vector3 from = m_TargetGroupManager.GetTargetPosition();
        m_CameraTargetHandler.UpdateCameraTarget();
        m_TargetGroupManager.UpdateTargetGroup();
        Vector3 to = m_TargetGroupManager.GetTargetPosition();
        
        if ((to - from).sqrMagnitude < 0.001f) return;
        
        m_CinemachineCamera.PreviousStateIsValid = false;
        m_CinemachineCamera.OnTargetObjectWarped(m_TargetGroupManager.transform, to - from);
        
        CinemachineBrain.GetActiveBrain(0).ManualUpdate();
        
        CinemachineBrain.GetActiveBrain(0).UpdateMethod = updateMode;
        
    }

    private void OnEnable()
    {
        m_PlayerCamera.Set(this);
        m_PlayerController.Get().OnPlayerTeleported += TeleportCamera;
    }

    private void OnDisable()
    {
        m_PlayerController.Get().OnPlayerTeleported -= TeleportCamera;
        m_PlayerCamera.Set(null);
    }

    public Camera GetCamera() => m_Camera;
}