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
        Debug.Log("Stop");
        if(m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();

        if(m_PoolTicket != null)
        {
            m_PoolTicket.Release();
        }
    }
}