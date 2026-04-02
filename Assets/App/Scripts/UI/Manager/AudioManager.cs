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

    [Title("FADING PARAMETERS")]
    private float m_MusicVolumeFadeStartValue;
    private float m_MusicVolumeFadeDuration;
    private float m_MusicVolumeFadeElapsedTime;
    private bool m_IsMusicVolumeFading;

    protected override void Awake()
    {
        base.Awake();

        InitializeAudio();

        m_MasterVolumeSetting.OnFloatChanged += VolumeSetMaster;
        m_MusicVolumeSetting.OnFloatChanged += VolumeSetMusic;

        m_MasterVolumeSetting.LoadSavedValue();
        m_MusicVolumeSetting.LoadSavedValue();
    }

    private void Update()
    {
        if (m_IsMusicVolumeFading)
        {
            m_MusicVolumeFadeElapsedTime += Time.deltaTime;

            if (m_MusicVolumeFadeElapsedTime >= m_MusicVolumeFadeDuration)
            {
                m_MusicBus.Bus.setVolume(0f);
                m_IsMusicVolumeFading = false;
            }
            else
            {
                float progress = m_MusicVolumeFadeElapsedTime / m_MusicVolumeFadeDuration;
                float currentVolume = Mathf.Lerp(m_MusicVolumeFadeStartValue, 0f, progress);
                m_MusicBus.Bus.setVolume(currentVolume);
            }
        }
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

    public void FadeMusicVolume(float fadeDurationInSeconds)
    {
        m_MusicBus.Bus.getVolume(out float currentVolume);
        m_MusicVolumeFadeStartValue = currentVolume;
        m_MusicVolumeFadeDuration = fadeDurationInSeconds;
        m_MusicVolumeFadeElapsedTime = 0f;
        m_IsMusicVolumeFading = true;
    }
}