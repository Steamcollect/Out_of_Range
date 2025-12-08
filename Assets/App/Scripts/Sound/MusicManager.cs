using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicManager : MonoBehaviour
{
    private const float k_ExplorationPhase = 0f;
    private const float k_BattlePhase = 1f;

    [SerializeField] private EventReference m_Music;

    [SerializeField] private RSE_OnFightStarted m_FightStarted;
    [SerializeField] private RSE_OnFightEnded m_FightEnded;
    private EventInstance m_MusicInstance;

    private void Start()
    {
        m_MusicInstance = RuntimeManager.CreateInstance(m_Music);
        SwitchToExploration();
        m_MusicInstance.start();
    }

    private void OnEnable()
    {
        m_FightStarted.Action += SwitchToBattle;
        m_FightEnded.Action += SwitchToExploration;
    }

    private void OnDisable()
    {
        m_FightStarted.Action -= SwitchToBattle;
        m_FightEnded.Action -= SwitchToExploration;
    }

    public void Play()
    {
        m_MusicInstance.start();
    }

    public void Stop()
    {
        m_MusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void SwitchToBattle()
    {
        m_MusicInstance.setParameterByName("Phase", k_BattlePhase);
    }

    private void SwitchToExploration()
    {
        m_MusicInstance.setParameterByName("Phase", k_ExplorationPhase);
    }
}