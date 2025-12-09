using System.Collections;
using UnityEngine;

public class ExplodingEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] int m_Damage;
    [SerializeField] float m_ExplosionRadius;

    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public override IEnumerator Attack()
    {
        foreach (Collider hit in Physics.OverlapSphere(transform.position, m_ExplosionRadius))
            if (hit.TryGetComponentInChildrens(out IHealth health))
            {
                health.TakeDamage(m_Damage);
            }

        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }
}