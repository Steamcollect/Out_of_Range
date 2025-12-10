using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : RegularSingleton<SettingsManager>
{
    [Title("AUDIO")]
    [SerializeField] private string m_MasterBusName;
    [SerializeField] private string m_MusicBusName;
    [SerializeField] private string m_EffectsBusName;

    [Title("GRAPHICS")]
    [SerializeField] private CustomDropdown m_ResolutionsDropdown;

    private List<Resolution> m_Resolutions;
    private List<string> m_Options = new List<string>();

    private FMOD.Studio.Bus m_MasterBus;
    private FMOD.Studio.Bus m_MusicBus;
    private FMOD.Studio.Bus m_EffectsBus;

    private void Start()
    {
        // AUDIO
        //m_MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/+" + m_MasterBusName);
        //m_MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/+" + m_MusicBusName);
        //m_EffectsBus = FMODUnity.RuntimeManager.GetBus("bus:/+" + m_EffectsBusName);

        m_MasterBus.setVolume(PlayerPrefs.GetFloat("MasterVolume" + "Slider"));
        m_MasterBus.setVolume(PlayerPrefs.GetFloat("MusicVolume" + "Slider"));
        m_MasterBus.setVolume(PlayerPrefs.GetFloat("EffectsVolume" + "Slider"));

        // RESOLUTIONS

        if (m_ResolutionsDropdown == null) return;

        m_ResolutionsDropdown.DropdownItems.RemoveRange(0, m_ResolutionsDropdown.DropdownItems.Count);
        m_Resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().Reverse().ToList();

        int currentResolutionIndex = 0;
        for (int i = 0; i < m_Resolutions.Count; i++)
        {
            string option = m_Resolutions[i].width + " x " + m_Resolutions[i].height;
            m_Options.Add(option);

            if (m_Resolutions[i].width == Screen.currentResolution.width && m_Resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                m_ResolutionsDropdown.SelectedIndex = currentResolutionIndex;
                m_ResolutionsDropdown.Index = currentResolutionIndex;
            }

            m_ResolutionsDropdown.CreateNewOption(m_Options[i]);
            CustomDropdown.Item item = m_ResolutionsDropdown.DropdownItems[i];
            item.OnItemSelection = new UnityEvent();
            item.OnItemSelection.AddListener(UpdateResolution);
        }

        m_ResolutionsDropdown.SetupDropdown();
    }

    public void UpdateResolution()
    {
        SetResolution(m_ResolutionsDropdown.Index);
        StartCoroutine(FixResolution());
    }

    IEnumerator FixResolution()
    {
        yield return new WaitForSeconds(0.1f);
        SetResolution(m_ResolutionsDropdown.Index);
        StopCoroutine(FixResolution());
    }

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(m_Resolutions[resolutionIndex].width, m_Resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    public void VolumeSetMaster(float volume)
    {
        m_MasterBus.setVolume(volume);
    }

    public void VolumeSetMusic(float volume)
    {
        m_MusicBus.setVolume(volume);
    }

    public void VolumeSetEffects(float volume)
    {
        m_EffectsBus.setVolume(volume);
    }

    public void AnisotropicFilteringEnable()
    {
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
    }

    public void AnisotropicFilteringDisable()
    {
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
    }

    public void AntiAliasingSet(int index)
    {
        // 0, 2, 4, 8 - Zero means off
        QualitySettings.antiAliasing = index;
    }

    public void VsyncSet(int index)
    {
        // 0, 1 - Zero means off
        QualitySettings.vSyncCount = index;
    }

    public void ShadowResolutionSet(int index)
    {
        if (index == 3)
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
        else if (index == 2)
            QualitySettings.shadowResolution = ShadowResolution.High;
        else if (index == 1)
            QualitySettings.shadowResolution = ShadowResolution.Medium;
        else if (index == 0)
            QualitySettings.shadowResolution = ShadowResolution.Low;
    }

    public void ShadowsSet(int index)
    {
        if (index == 0)
            QualitySettings.shadows = ShadowQuality.Disable;
        else if (index == 1)
            QualitySettings.shadows = ShadowQuality.All;
    }

    public void ShadowsCascasedSet(int index)
    {
        //0 = No, 2 = Two, 4 = Four
        QualitySettings.shadowCascades = index;
    }

    public void TextureSet(int index)
    {
        // 0 = Full, 4 = Eight Resolution
        QualitySettings.globalTextureMipmapLimit = index;
    }

    public void SoftParticleSet(int index)
    {
        if (index == 0)
            QualitySettings.softParticles = false;
        else if (index == 1)
            QualitySettings.softParticles = true;
    }

    public void ReflectionSet(int index)
    {
        if (index == 0)
            QualitySettings.realtimeReflectionProbes = false;
        else if (index == 1)
            QualitySettings.realtimeReflectionProbes = true;
    }

    public void SetOverallQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void WindowFullscreen()
    {
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    public void WindowBorderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void WindowWindowed()
    {
        Screen.fullScreen = false;
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }
}