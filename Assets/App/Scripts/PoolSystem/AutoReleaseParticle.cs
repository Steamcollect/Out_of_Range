using UnityEngine;

public class AutoReleaseParticle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem m_ParticleSystem;

    private PooledObject m_PoolTicket;

    private void OnEnable()
    {
        m_ParticleSystem.Play();
    }

    private void OnParticleSystemStopped()
    {
        if(m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();
        m_PoolTicket?.Release();
    }
}