using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : RegularSingleton<AudioManager>
{
    [Title("AUDIO")]
    [SerializeField] private SSO_FMODBus m_MasterBus;
    [SerializeField] private SSO_FMODBus m_MusicBus;
    
    [Title("REFERENCES")]
    [SerializeField] private SSO_UniversalSettings m_MasterVolumeSetting;
    [SerializeField] private SSO_UniversalSettings m_MusicVolumeSetting;

    protected override void Awake()
    {
        base.Awake();

        InitializeAudio();

        m_MasterVolumeSetting.OnFloatChanged += VolumeSetMaster;
        m_MusicVolumeSetting.OnFloatChanged += VolumeSetMusic;

        m_MasterVolumeSetting.LoadSavedValue();
        m_MusicVolumeSetting.LoadSavedValue();
    }

    private void InitializeAudio()
    {
        // AUDIO
        m_MasterBus.Bus.setVolume(m_MasterVolumeSetting.CurrentFloat);
        m_MusicBus.Bus.setVolume(m_MusicVolumeSetting.CurrentFloat);
    }

    public void VolumeSetMaster(float volume)
    {
        m_MasterBus.Bus.setVolume(volume);
    }

    public void VolumeSetMusic(float volume)
    {
        m_MusicBus.Bus.setVolume(volume);
    }
}