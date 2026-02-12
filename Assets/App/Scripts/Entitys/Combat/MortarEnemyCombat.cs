using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MortarEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] private int m_GrenadeCount;
    [SerializeField] private float m_TimeBetweenGrenade;
    [Space(5)]
    [SerializeField] private float m_TimeBeforeAttack;
    [SerializeField] private float m_TimeAfterAttack;
    [SerializeField] private float m_TimeBetweenAttacks;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private Grenade m_GrenadePrefab;
    [SerializeField] GameObject m_MuzzleFlashPrefab;

    [Space(5)]
    [SerializeField] private RSO_PlayerController m_Player;

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnShoot;

    public event Action<float /*DelayBeforeShoot*/, float /*DelayAfterShoot*/> OnShootLaunched;
    public event Action OnShootCompleted;

    public override IEnumerator Attack()
    {
        if (!m_CanAttack) yield break;

        m_IsAttacking = true;
        OnShootLaunched?.Invoke(m_TimeBeforeAttack, m_TimeAfterAttack);

        yield return new WaitForSeconds(m_TimeBeforeAttack);

        for (int i = 0; i < m_GrenadeCount; i++)
        {
            Grenade grenade = Instantiate(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            grenade.Setup(m_AttackPoint.position, m_Player.Get().GetTargetPosition());

            grenade.Move();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            m_OnShoot.Invoke();

            yield return new WaitForSeconds(m_TimeBetweenGrenade);
        }

        yield return new WaitForSeconds(m_TimeAfterAttack);
        SetTurnSmoothTime(m_TurnSmoothTime);
        OnShootCompleted?.Invoke();

        yield return new WaitForSeconds(m_TimeBetweenAttacks);
        m_IsAttacking = false;
    }
}