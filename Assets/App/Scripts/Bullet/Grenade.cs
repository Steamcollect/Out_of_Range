using System;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] int m_Damage;
    [SerializeField] float explosionRadius;
    [SerializeField] GameObject radiusVisualizer;
    private GameObject explosionEffect;

    public int Damage
    {
        get => m_Damage;
        set => m_Damage = value;
    }

    public void ShowExplosionRadius(Vector3 position)
    {
        if (radiusVisualizer != null)
        {
            explosionEffect = Instantiate(radiusVisualizer, position, Quaternion.identity);
            explosionEffect.transform.localScale = Vector3.one * explosionRadius * 2f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Explode();
    }
    
    private void Explode()
    {
        Destroy(explosionEffect);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.gameObject.TryGetComponent(out HurtBox hurtBox))
            {
                hurtBox.TakeDamage(m_Damage);
            }
        }
        Destroy(gameObject);
    }
}
