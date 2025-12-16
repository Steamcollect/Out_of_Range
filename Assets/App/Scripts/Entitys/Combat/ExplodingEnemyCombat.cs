using System.Collections;
using UnityEngine;

public class ExplodingEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] int m_Damage;
    [SerializeField] float m_ExplosionRadius;

    [SerializeField] LayerMask m_ExplosionMask;

    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public override IEnumerator Attack()
    {
        foreach (Collider hit in Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_ExplosionMask))
            if (hit.TryGetComponent(out HurtBox hurtBox))
                hurtBox.TakeDamage(m_Damage);

        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }
}