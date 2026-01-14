using System;
using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

public class RangeEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] private int m_BulletCount;
    [SerializeField] private float m_TimeBetweenBullets;
    [Space(5)]
    [SerializeField] private float m_TimeBeforeAttack;
    [SerializeField] private float m_TimeAfterAttack;
    [SerializeField] private float m_TimeBetweenAttacks;
    [Space(5)]
    [SerializeField] private bool m_TurnWhileShooting;
    [SerializeField, ShowIf("m_TurnWhileShooting", true)] private float m_TurnSmoothTimeOnShoot;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private Bullet m_BulletPrefab;
    [Space(5)]
    [SerializeField] private RSO_PlayerController m_Player;
    
    [Header("Output")]
    [SerializeField] private UnityEvent m_OnShoot;

    public event Action<float /*DelayBeforeShoot*/, float /*DelayAfterShoot*/> OnShootLaunched;
    public event Action OnShootCompleted;

    public override IEnumerator Attack()
    {
        SetTurnSmoothTime(m_TurnWhileShooting ? m_TurnSmoothTimeOnShoot : 0f);
        m_IsAttacking = true;
        OnShootLaunched?.Invoke(m_TimeBeforeAttack, m_TimeAfterAttack);

        yield return new WaitForSeconds(m_TimeBeforeAttack);
        
        for(int i = 0; i < m_BulletCount; i++)
        {
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();
            
            m_OnShoot.Invoke();

            yield return new WaitForSeconds(m_TimeBetweenBullets);
        }

        yield return new WaitForSeconds(m_TimeAfterAttack);
        SetTurnSmoothTime(m_TurnSmoothTime);
        OnShootCompleted?.Invoke();

        yield return new WaitForSeconds(m_TimeBetweenAttacks);
        m_IsAttacking = false;
    }
}