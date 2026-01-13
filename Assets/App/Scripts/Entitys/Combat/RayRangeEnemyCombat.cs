using MVsToolkit.Dev;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RayRangeEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] private int m_Damage;
    [Space(5)]
    [SerializeField] private float m_TimeBeforeAttack;
    [SerializeField] private float m_TimeAfterAttack;
    [SerializeField] private float m_TimeBetweenAttacks;
    [Space(5)]
    [SerializeField] private LayerMask m_DetectionMask;
    [SerializeField, TagName] private string m_PlayerTag;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [Space(5)]
    [SerializeField] private RSO_PlayerController m_Player;
    [Header("Output")]
    [SerializeField] private UnityEvent m_OnShoot;

    public event Action<float /*DelayBeforeShoot*/, float /*DelayAfterShoot*/> OnShootLaunched;
    public event Action OnShootCompleted;

    public override IEnumerator Attack()
    {
        SetActiveLookAt(false);
        m_IsAttacking = true;
        OnShootLaunched?.Invoke(m_TimeBeforeAttack, m_TimeAfterAttack);

        yield return new WaitForSeconds(m_TimeBeforeAttack);

        Ray ray = new Ray(m_AttackPoint.position, m_AttackPoint.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.collider.TryGetComponent(out HurtBox hurtBox))
            {
                hurtBox.TakeDamage(m_Damage);
            }
        }

        //Debug.Log("Ray shot from " + m_AttackPoint.position + " towards " + m_Player.Get().GetTargetPosition());
        m_OnShoot.Invoke();

        yield return new WaitForSeconds(m_TimeAfterAttack);

        SetActiveLookAt(true);
        OnShootCompleted?.Invoke();

        yield return new WaitForSeconds(m_TimeBetweenAttacks);
        Debug.Log("IsAttackingFalse");
        m_IsAttacking = false;
    }
}