using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicManager : MonoBehaviour
{
    const float k_MenuPhase = 0f;
    const float k_CalmPhase = 1f;
    const float k_ArenaPhase = 2f;
    const float k_BossAPhase = 3f;
    const float k_BossBPhase = 4f;

    [SerializeField] EventReference m_Music;
    EventInstance m_MusicInstance;

    [Space]
    [SerializeField] RSE_SetMusicToMenu m_SetMusicToMenu;
    [SerializeField] RSE_SetMusicToCalm m_SetMusicToCalm;
    [SerializeField] RSE_SetMusicToArena m_SetMusicToArena;
    [SerializeField] RSE_SetMusicToBossP1 m_SetMusicToBossP1;
    [SerializeField] RSE_SetMusicToBossP2 m_SetMusicToBossP2;

    private void OnEnable()
    {
        m_SetMusicToMenu.Action += SwitchToMenu;
        m_SetMusicToCalm.Action += SwitchToFight;
        m_SetMusicToArena.Action += SwitchToArena;
        m_SetMusicToBossP1.Action += SwitchToBossA;
        m_SetMusicToBossP2.Action += SwitchToBossB;
    }

    private void OnDisable()
    {
        m_SetMusicToMenu.Action -= SwitchToMenu;
        m_SetMusicToCalm.Action -= SwitchToFight;
        m_SetMusicToArena.Action -= SwitchToArena;
        m_SetMusicToBossP1.Action -= SwitchToBossA;
        m_SetMusicToBossP2.Action -= SwitchToBossB;
    }

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

    public void Play()
    {
        m_MusicInstance.start();
    }

    public void Stop()
    {
        m_MusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void SwitchToMenu()
    {
        m_MusicInstance.setParameterByName("Phase", k_MenuPhase);
    }

    private void SwitchToFight()
    {
        m_MusicInstance.setParameterByName("Phase", k_CalmPhase);
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