using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class RangeReloadingWeaponSFXManager : MonoBehaviour
{
    [FormerlySerializedAs("m_FireSFX")] [SerializeField] private EventReference m_FireSfx;
    [FormerlySerializedAs("m_ReloadSFX")] [SerializeField] private EventReference m_ReloadSfx;

    [SerializeField] private float m_FirePitchVariance = 0.2f;
    [SerializeField] private float m_ReloadVolume = 1.0f;
    [SerializeField] private float m_FireVolume = 1.0f;
    private EventInstance m_FireSfxInstance;
    private EventInstance m_ReloadSfxInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!m_FireSfx.IsNull)
        {
            m_FireSfxInstance = RuntimeManager.CreateInstance(m_FireSfx);
            m_FireSfxInstance.setVolume(m_FireVolume);
        }

        if (!m_ReloadSfx.IsNull)
        {
            m_ReloadSfxInstance = RuntimeManager.CreateInstance(m_ReloadSfx);
            m_ReloadSfxInstance.setVolume(m_ReloadVolume);
        }
    }

    [Button("Attack SFX")]
    public void PlayAttackSfx()
    {
        if (!m_FireSfxInstance.isValid()) return;

        m_FireSfxInstance.stop(STOP_MODE.IMMEDIATE);
        m_FireSfxInstance.setPitch(Random.value * m_FirePitchVariance + (1 - m_FirePitchVariance / 2));
        m_FireSfxInstance.start();
    }

    [Button("Reload SFX")]
    public void PlayReloadSfx()
    {
        if (!m_ReloadSfxInstance.isValid()) return;

        m_ReloadSfxInstance.stop(STOP_MODE.IMMEDIATE);
        m_ReloadSfxInstance.start();
    }
}