using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDelay = 5f;
    [SerializeField] private int m_Damage = 50;

    [Header("References")]
    [SerializeField] private GameObject barrelModel;
    [SerializeField] private VisualEffect loadingEffect;
    [SerializeField] private VisualEffect explosionVFX;

    public UnityEvent onExplode;

    private void Start()
    {
        loadingEffect.gameObject.SetActive(false);
        explosionVFX.gameObject.SetActive(false);

        barrelModel.SetActive(true);
    }
    
    public LayerMask mask;
    public void Explode()
    {
        StopAllCoroutines();
        StartCoroutine(ExplosionVFX());
    }

    private void InflictDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, mask);

        foreach (Collider collider in colliders)
        {
            if(collider.TryGetComponent(out HurtBox hurtBox))
            {
                hurtBox.TakeDamage(m_Damage);
            }
        }
    }

    public IEnumerator ExplosionVFX()
    {
        loadingEffect.gameObject.SetActive(true);
        loadingEffect.Play();
        yield return new WaitForSeconds(explosionDelay);
        loadingEffect.Stop();
        loadingEffect.gameObject.SetActive(false);

        barrelModel.SetActive(false);

        onExplode.Invoke();
        InflictDamage();
        explosionVFX.gameObject.SetActive(true);
        explosionVFX.Play();

        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
