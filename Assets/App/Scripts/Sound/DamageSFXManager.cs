using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class DamageSFXManager : MonoBehaviour
{
    [FormerlySerializedAs("m_DamageSFX")] [SerializeField] private EventReference m_DamageSfx;
    [FormerlySerializedAs("m_DeathSFX")] [SerializeField] private EventReference m_DeathSfx;

    [SerializeField] private float m_DamageVolume = 1.0f;
    [SerializeField] private float m_DeathVolume = 1.0f;

    [SerializeField] private float m_DamagePitchVariance = 0.2f;

    private EventInstance m_DamageSfxInstance;
    private EventInstance m_DeathSfxInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!m_DamageSfx.IsNull)
        {
            m_DamageSfxInstance = RuntimeManager.CreateInstance(m_DamageSfx);
            m_DamageSfxInstance.setVolume(m_DamageVolume);
        }

        if (!m_DeathSfx.IsNull)
        {
            m_DeathSfxInstance = RuntimeManager.CreateInstance(m_DeathSfx);
            m_DeathSfxInstance.setVolume(m_DeathVolume);
        }
    }

    public void PlayDamageSfx()
    {
        if (m_DamageSfxInstance.isValid())
        {
            m_DamageSfxInstance.stop(STOP_MODE.IMMEDIATE);
            m_DamageSfxInstance.setPitch(Random.value * m_DamagePitchVariance + (1 - m_DamagePitchVariance / 2));
            m_DamageSfxInstance.start();
        }
    }

    public void PlayDeathSfx()
    {
        if (m_DeathSfxInstance.isValid())
        {
            m_DeathSfxInstance.stop(STOP_MODE.IMMEDIATE);
            m_DeathSfxInstance.start();
        }
    }
}