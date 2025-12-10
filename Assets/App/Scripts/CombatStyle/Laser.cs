using System;
using System.Collections;
using UnityEngine;

public class Laser : CombatStyle
{
    [Header("Laser Settings")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private LineRenderer m_LaserBeamLine;
    [SerializeField] private float m_LaserRange = 50f;
    [SerializeField] private int m_DamagePerSecond = 10;

    private void Start()
    {
        m_LaserBeamLine.enabled = false;
    }

    public override IEnumerator Attack()
    {
        if (!m_CanAttack) yield break;

        if (m_CurrentMana.Get() >= manaCostPerAttack)
        {
            m_CurrentMana.Set(m_CurrentMana.Get() - (manaCostPerAttack * Time.deltaTime));
        }
        else
        {
            yield break;
        }
        
        m_IsAttacking = true;
        m_LaserBeamLine.enabled = true;

        RaycastHit hit;
        Vector3 endPosition = m_AttackPoint.position + m_AttackPoint.forward * m_LaserRange;

        if (Physics.Raycast(m_AttackPoint.position, m_AttackPoint.forward, out hit, m_LaserRange))
        {
            endPosition = hit.point;

            if (hit.collider.TryGetComponent(out IHealth health))
            {
                // TODO : revoir le système de dégats du laser
                // Les dégâts sont appliqués à chaque frame, en utilisant des int au lieu de float le laser fait soit trop de dégats soit rien du tout.
                health.TakeDamage(Mathf.FloorToInt(m_DamagePerSecond * Time.deltaTime));
            }
        }

        // Update the laser beam line positions
        m_LaserBeamLine.SetPosition(0, m_AttackPoint.position);
        m_LaserBeamLine.SetPosition(1, endPosition);
    }
    
    public override void StopAttack()
    {
        m_IsAttacking = false;
        m_LaserBeamLine.enabled = false;
    }
}
