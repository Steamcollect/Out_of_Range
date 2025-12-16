using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera m_CinemachineCamera;
    [SerializeField] private Camera m_Camera;
    [Space(10)]
    [SerializeField] private RSO_PlayerController m_PlayerController;
    
    [Header("Output")]
    [SerializeField] private RSO_PlayerCameraController m_PlayerCamera;
    
    private void OnEnable()
    {
        m_PlayerCamera.Set(this);
        m_CinemachineCamera.ForceCameraPosition(m_PlayerController.Get().GetTargetPosition(),m_CinemachineCamera.transform.rotation);
    }

    private void OnDisable() => m_PlayerCamera.Set(null);

    public Camera GetCamera() => m_Camera;
}