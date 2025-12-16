using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_ExplosionRadius;
    [SerializeField] private float m_LoadingDuration;
    [SerializeField] private float m_ExplosionDuration;
    [SerializeField] private int m_Damage;

    [Header("References")]
    [SerializeField] private GameObject m_BarrelMesh;
    [SerializeField] private VisualEffect m_LoadingEffect;
    [SerializeField] private VisualEffect m_ExplosionEffect;
    [SerializeField] private LayerMask mask;

    public UnityEvent OnLoading;
    public UnityEvent OnExplode;

    private bool m_IsExploding;

    private void Start()
    {
        m_LoadingEffect.gameObject.SetActive(false);
        m_ExplosionEffect.gameObject.SetActive(false);

        m_BarrelMesh.SetActive(true);
    }

    public void Explode()
    {
        if(m_IsExploding) return;

        m_IsExploding = true;

        StartCoroutine(ExplosionVFX());
    }

    private void InflictDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, mask);

        foreach (Collider collider in colliders)
        {
            if(collider.TryGetComponent(out HurtBox hurtBox))
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
}
