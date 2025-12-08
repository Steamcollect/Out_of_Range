using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private CinemachineCamera m_FightCamera;
    [SerializeField] private CinemachineCamera m_ExplorationCamera;
    [SerializeField] private RSE_OnFightEnded m_OnFightEnded;
    [SerializeField] private RSE_OnFightStarted m_OnFightStarted;

    [SerializeField] private RSO_PlayerCameraController m_PlayerCamera;
    [SerializeField] private Camera m_Camera;

    [SerializeField] private float m_DelayBeforeSwitchingToExploration = 5f;
    private float m_Timer;

    private void Start()
    {
        m_PlayerCamera.Set(this);
        m_ExplorationCamera.Follow = m_PlayerController.Get().transform;
        SetFightParameters();
    }

    // TODO: Utiliser le RSE_OnFightEnded pour switcher la caméra.
    private void Update()
    {
        return; // On reste tout le temps sur la cam de fight

        if (m_ExplorationCamera.isActiveAndEnabled) return;

        m_Timer += Time.deltaTime;

        if (m_Timer > m_DelayBeforeSwitchingToExploration) SetExplorationParameters();
    }

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

    private void SetFightParameters()
    {
        if (m_FightCamera.isActiveAndEnabled) return;

        m_Timer = 0f;

        m_ExplorationCamera.enabled = false;
        m_FightCamera.enabled = true;
    }

    private void SetExplorationParameters()
    {
        return; // On ne switch jamais en mode exploration pour l'instant

        m_FightCamera.enabled = false;
        m_ExplorationCamera.enabled = true;
    }

    public Camera GetCamera()
    {
        return m_Camera;
    }
}