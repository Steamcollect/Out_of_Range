using System;
using UnityEngine;
using UnityEngine.VFX;

public class Grenade : MonoBehaviour
{
    [SerializeField] int m_Damage;
    [SerializeField] float explosionRadius;
    [SerializeField] GameObject radiusVisualizer;
    [SerializeField] private VisualEffect explosionEffect;
    private GameObject radius;

    public int Damage
    {
        get => m_Damage;
        set => m_Damage = value;
    }

    public void ShowExplosionRadius(Vector3 position)
    {
        if (radiusVisualizer != null)
        {
            radius = Instantiate(radiusVisualizer, position, Quaternion.identity);
            radius.transform.localScale = Vector3.one * explosionRadius * 2f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Explode();
    }
    
    private void Explode()
    {
        Destroy(radius);
        // Peut etre mettre une Pool ici mdrr
        Instantiate(explosionEffect.gameObject, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.gameObject.TryGetComponent(out HurtBox hurtBox) && !nearbyObject.CompareTag("Player"))
            {
                hurtBox.TakeDamage(m_Damage);
            }
        }
        Destroy(gameObject);
    }
}
