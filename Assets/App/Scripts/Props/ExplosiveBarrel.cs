using System.Collections;
using MoreMountains.Feedbacks;
using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class ExplosiveBarrel : MonoBehaviour, ITargetable
{
    [Header("Settings")]
    [SerializeField] private float m_ExplosionRadius;
    [SerializeField] private float m_LoadingDuration;
    [SerializeField] private float m_ExplosionDuration;
    [SerializeField] private float m_SpawnDuration = 2.0f;
    [SerializeField] private int m_Damage;

    [Header("References")]
    [SerializeField] private GameObject m_BarrelMesh;
    [SerializeField] private VisualEffect m_LoadingEffect;
    [SerializeField] private VisualEffect m_ExplosionEffect;
    [SerializeField] private LayerMask mask;
    [SerializeField] Collider[] m_Collids;

    public UnityEvent OnLoading;
    public UnityEvent OnExplode;

    private bool m_IsExploding;
    private bool m_IsInvincible = true;
    private readonly Collider[] m_ResultsBuffer = new Collider[32];

    private void Start()
    {
        m_LoadingEffect.gameObject.SetActive(false);
        m_ExplosionEffect.gameObject.SetActive(false);

        m_BarrelMesh.SetActive(true);

        m_IsInvincible = true;
        this.Delay(() => m_IsInvincible = false, m_SpawnDuration);
    }

    public void Explode()
    {
        if (m_IsInvincible == true) return;

        if(m_IsExploding) return;

        m_IsExploding = true;

        StartCoroutine(ExplosionVFX());
    }

    private void InflictDamage()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, m_ExplosionRadius, m_ResultsBuffer, mask);

        for (int i = 0; i < size; i++)
        {
            if(m_ResultsBuffer[i].TryGetComponent(out HurtBox hurtBox))
            {
                hurtBox.TakeDamage(m_Damage);
            }
        }
    }

    public void Loading()
    {
        OnLoading.Invoke();
        m_LoadingEffect.gameObject.SetActive(true);
        m_LoadingEffect.Play();
    }

    public void Explosion()
    {
        m_LoadingEffect.Stop();
        foreach (Collider collid in m_Collids)
        {
            collid.enabled = false;
        }
        m_LoadingEffect.gameObject.SetActive(false);

        OnExplode.Invoke();
        InflictDamage();
        m_ExplosionEffect.gameObject.SetActive(true);
        m_ExplosionEffect.Play();

        Destroy(m_BarrelMesh);
    }

    public IEnumerator ExplosionVFX()
    {
        Loading();
        yield return new WaitForSeconds(m_LoadingDuration);
        Explosion();
        yield return new WaitForSeconds(m_ExplosionDuration);
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }

    public Vector3 GetTargetPosition()
    {
        return transform.position + Vector3.up;
    }

    public Vector3 GetTargetIndicatorPosition()
    {
        return transform.position + Vector3.up * 1.5f;
    }
}
