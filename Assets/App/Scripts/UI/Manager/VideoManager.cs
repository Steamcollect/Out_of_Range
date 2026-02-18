using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VideoManager : RegularSingleton<VideoManager>
{
    [Title("REFERENCES")]
    [SerializeField] private SSO_UniversalSettings m_Resolution;
    [SerializeField] private SSO_UniversalSettings m_WindowMode;
    [SerializeField] private SSO_UniversalSettings m_VSync;

    private List<Resolution> m_Resolutions;

    protected override void Awake()
    {
        base.Awake();
        InitializeResolutions();

        m_Resolution.OnEnumChanged += SetResolution;
        m_WindowMode.OnEnumChanged += SetWindowMode;
        m_VSync.OnEnumChanged += SetVSync;

        m_Resolution.LoadSavedValue();
        m_WindowMode.LoadSavedValue();
        m_VSync.LoadSavedValue();
    }

    private void OnDestroy()
    {
        m_Resolution.OnEnumChanged -= SetResolution;
        m_WindowMode.OnEnumChanged -= SetWindowMode;
        m_VSync.OnEnumChanged -= SetVSync;
    }

    private void InitializeResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        m_Resolutions = new List<Resolution>();

        double currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        for(int i = 0; i < allResolutions.Length; i++)
        {
            if(!m_Resolutions.Any(r => r.width == allResolutions[i].width && r.height == allResolutions[i].height))
            {
                m_Resolutions.Add(allResolutions[i]);
            }
        }

        List<string> options = new List<string>();

        for (int i = 0; i < m_Resolutions.Count; i++)
        {
            string option = m_Resolutions[i].width + " x " + m_Resolutions[i].height;
            options.Add(option);

            if (m_Resolutions[i].width == Screen.width &&
                m_Resolutions[i].height == Screen.height)
            {
                m_Resolution.SetNewEnumValue(i);
            }
        }

        m_Resolution.EnumOptions = options.ToArray();
    }

    private void SetResolution(int index)
    {
        if (index < 0 || index >= m_Resolutions.Count) return;

        Resolution resolution = m_Resolutions[index];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    private void SetWindowMode(int index)
    {
        switch (index)
        {
            case 0:
                WindowWindowed();
                break;
            case 1:
                WindowBorderless();
                break;
            case 2:
                WindowFullscreen();
                break;
        }
    }

    public void SetVSync(int index)
    {
        QualitySettings.vSyncCount = index;
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