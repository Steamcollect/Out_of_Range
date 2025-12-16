using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : RegularSingleton<AudioManager>
{
    [Title("AUDIO")]
    [SerializeField] private SSO_FMODBus m_MasterBus;
    [SerializeField] private SSO_FMODBus m_MusicBus;
    [SerializeField] private SSO_FMODBus m_SFXBus;
    
    [Title("REFERENCES")]
    [SerializeField] private SSO_UniversalSettings m_MasterVolumeSetting;
    [SerializeField] private SSO_UniversalSettings m_MusicVolumeSetting;
    [SerializeField] private SSO_UniversalSettings m_SFXVolumeSetting;

    protected override void Awake()
    {
        base.Awake();

        InitializeAudio();
    }

    private void InitializeAudio()
    {
        // AUDIO
        m_MasterBus.Bus.setVolume(m_MasterVolumeSetting.CurrentFloat);
        m_MusicBus.Bus.setVolume(m_MusicVolumeSetting.CurrentFloat);
        m_SFXBus.Bus.setVolume(m_SFXVolumeSetting.CurrentFloat);
    }

    public void VolumeSetMaster(float volume)
    {
        m_MasterBus.Bus.setVolume(volume);
    }

    public void VolumeSetMusic(float volume)
    {
        m_MusicBus.Bus.setVolume(volume);
    }
    
    public void VolumeSetSFX(float volume)
    {
        m_SFXBus.Bus.setVolume(volume);
    }
}