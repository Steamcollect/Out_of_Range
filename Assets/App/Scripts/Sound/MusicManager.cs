using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicManager : MonoBehaviour
{
    private const float k_MenuPhase = 0f;
    private const float k_FightPhase = 1f;
    private const float k_ArenaPhase = 2f;
    private const float k_BossAPhase = 3f;
    private const float k_BossBPhase = 4f;

    [SerializeField] private EventReference m_Music;

    // TODO: Replace these events with one for each phase
    [SerializeField] private RSE_OnFightStarted m_FightStarted;
    [SerializeField] private RSE_OnFightEnded m_FightEnded;
    
    
    private EventInstance m_MusicInstance;

    private void Start()
    {
        m_MusicInstance = RuntimeManager.CreateInstance(m_Music);
        m_MusicInstance.start();
    }

    private void OnDestroy()
    {
        m_MusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        m_MusicInstance.release();
    }

    private void OnEnable()
    {
        //m_FightStarted.Action += SwitchToBattle;
        //m_FightEnded.Action += SwitchToExploration;
    }

    private void OnDisable()
    {
        //m_FightStarted.Action -= SwitchToBattle;
        //m_FightEnded.Action -= SwitchToExploration;
    }

    public void Play()
    {
        m_MusicInstance.start();
    }

    public void Stop()
    {
        m_MusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void SwitchToFight()
    {
        m_MusicInstance.setParameterByName("Phase", k_FightPhase);
    }

    private void SwitchToMenu()
    {
        m_MusicInstance.setParameterByName("Phase", k_MenuPhase);
    }
    
    private void SwitchToArena()
    {
        m_MusicInstance.setParameterByName("Phase", k_ArenaPhase);
    }
    
    private void SwitchToBossA()
    {
        m_MusicInstance.setParameterByName("Phase", k_BossAPhase);
    }
    
    private void SwitchToBossB()
    {
        m_MusicInstance.setParameterByName("Phase", k_BossBPhase);
    }
}