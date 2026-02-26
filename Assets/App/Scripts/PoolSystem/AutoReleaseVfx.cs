using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.VFX;

public class AutoReleaseVfx : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Duration;
    
    [Header("References")]
    [SerializeField] private VisualEffect m_Vfx;

    private PooledObject m_PoolTicket;

    private void OnEnable()
    {
        m_Vfx.Play();
        this.Delay(Release, m_Duration);
    }

    private void Release()
    {
        if(m_PoolTicket == null) m_PoolTicket = GetComponent<PooledObject>();
        m_PoolTicket?.Release();
    }
}