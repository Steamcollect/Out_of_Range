using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int m_Damage = 50;
    [SerializeField] private GameObject explosionEffect;

    private void Start()
    {
        explosionEffect.SetActive(false);
    }
    
    public LayerMask mask;
    public void Explode()
    {
        StartCoroutine(ExplosionVFX());
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, mask);
        
        foreach (Collider collider in colliders)
        {
            collider.TryGetComponent<HurtBox>(out HurtBox hurtBox);
            hurtBox.TakeDamage(m_Damage);
        }
    }

    public IEnumerator ExplosionVFX()
    {
        explosionEffect.transform.localScale = Vector3.zero;
        explosionEffect.SetActive(true);
        float elapsedTime = 0f;
        float duration = 0.5f;
        while (elapsedTime < duration)
        {
            explosionEffect.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * explosionRadius * 2f, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        explosionEffect.transform.localScale = Vector3.one * explosionRadius * 2f;
        
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
