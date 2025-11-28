using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private CinemachineCamera m_FightCamera;
    [SerializeField] private CinemachineCamera m_ExplorationCamera;
    [SerializeField] private RSE_OnFightEnded m_OnFightEnded;
    [SerializeField] private RSE_OnFightStarted m_OnFightStarted;

    [SerializeField] RSO_PlayerCameraController m_PlayerCamera;
    [SerializeField] Camera m_Camera;

    [SerializeField] private float m_DelayBeforeSwitchingToExploration = 5f;
    private float m_Timer = 0f;
    
    private void OnEnable()
    {
        m_OnFightEnded.Action += SetExplorationParameters;
        m_OnFightStarted.Action += SetFightParameters;
    }
    
    private void OnDisable()
    {
        m_OnFightEnded.Action -= SetExplorationParameters;
        m_OnFightStarted.Action -= SetFightParameters;
    }

    private void Start()
    {
        m_PlayerCamera.Set(this);
        m_ExplorationCamera.Follow = m_PlayerController.Get().transform;
        SetExplorationParameters();
    }

    // TODO: Utiliser le RSE_OnFightEnded pour switcher la caméra.
    private void Update()
    {
        if (m_ExplorationCamera.isActiveAndEnabled) return;

        m_Timer += Time.deltaTime;
        
        if (m_Timer > m_DelayBeforeSwitchingToExploration)
        {
            SetExplorationParameters();
        }
    }

    private void SetFightParameters()
    {
        if (m_FightCamera.isActiveAndEnabled) return;

        m_Timer = 0f;
        
        m_ExplorationCamera.enabled = false;
        m_FightCamera.enabled = true;
    }
    
    private void SetExplorationParameters()
    {
        m_FightCamera.enabled = false;
        m_ExplorationCamera.enabled = true;
    }

    public Camera GetCamera() => m_Camera;
}