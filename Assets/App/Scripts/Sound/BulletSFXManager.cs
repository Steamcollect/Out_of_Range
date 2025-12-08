using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class BulletSFXManager : MonoBehaviour
{
    [FormerlySerializedAs("m_BulletImpactOnWallSFX")] [SerializeField] private EventReference m_BulletImpactOnWallSfx;
    [FormerlySerializedAs("m_BulletImpactOnTargetSFX")] [SerializeField] private EventReference m_BulletImpactOnTargetSfx;

    [SerializeField] private float m_ImpactOnWallVolume = 1.0f;
    [SerializeField] private float m_ImpactOnTargetVolume = 1.0f;

    [SerializeField] private float m_ImpactPitchVariance = 0.2f;
    private EventInstance m_BulletImpactOnTargetSfxInstance;

    private EventInstance m_BulletImpactOnWallSfxInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!m_BulletImpactOnWallSfx.IsNull)
        {
            m_BulletImpactOnWallSfxInstance = RuntimeManager.CreateInstance(m_BulletImpactOnWallSfx);
            m_BulletImpactOnWallSfxInstance.setVolume(m_ImpactOnWallVolume);
        }

        if (!m_BulletImpactOnTargetSfx.IsNull)
        {
            m_BulletImpactOnTargetSfxInstance = RuntimeManager.CreateInstance(m_BulletImpactOnTargetSfx);
            m_BulletImpactOnTargetSfxInstance.setVolume(m_ImpactOnTargetVolume);
        }
    }

    public void PlayImpactOnWall()
    {
        if (m_BulletImpactOnWallSfxInstance.isValid())
        {
            m_BulletImpactOnWallSfxInstance.stop(STOP_MODE.IMMEDIATE);
            m_BulletImpactOnWallSfxInstance.setPitch(Random.value * m_ImpactPitchVariance +
                                                     (1 - m_ImpactPitchVariance / 2));
            m_BulletImpactOnWallSfxInstance.start();
        }
    }

    public void PlayImpactOnTarget()
    {
        if (m_BulletImpactOnTargetSfxInstance.isValid())
        {
            m_BulletImpactOnTargetSfxInstance.stop(STOP_MODE.IMMEDIATE);
            m_BulletImpactOnTargetSfxInstance.setPitch(Random.value * m_ImpactPitchVariance +
                                                       (1 - m_ImpactPitchVariance / 2));
            m_BulletImpactOnTargetSfxInstance.start();
        }
    }
}