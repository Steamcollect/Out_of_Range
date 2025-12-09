using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;

public class RangeEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] int m_BulletCount;
    [SerializeField] float m_TimeBetweenBullets;

    [Space(10)]
    [SerializeField] float m_TimeBeforeAttack;
    [SerializeField] float m_TimeAfterAttack;

    [Space(10)]
    [SerializeField] bool m_TurnWhileShooting;

    [SerializeField, ShowIf("m_TurnWhileShooting", true)] float turnSmoothTimeOnShoot;
    bool isShooting = false;

    [Header("References")]
    [SerializeField] Transform m_AttackPoint;
    [SerializeField] Bullet m_BulletPrefab;

    [Space(5)]
    [SerializeField] RSO_PlayerController m_Player;

    //[Header("Input")]
    //[Header("Output")]

    void FixedUpdate()
    {
        if (m_TurnWhileShooting && isShooting)
            LookAt(m_Player.Get().GetTargetPosition(), LookAtAxis.Both, turnSmoothTimeOnShoot);
    }

    public override IEnumerator Attack()
    {
        yield return new WaitForSeconds(m_TimeBeforeAttack);

        int bulletsFired = 0;

        isShooting = true;
        while (bulletsFired < m_BulletCount)
        {
            bulletsFired++;

            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, Quaternion.identity);
            bullet.transform.up = m_AttackPoint.forward;
            bullet.Setup();

            yield return new WaitForSeconds(m_TimeBetweenBullets);
        }

        yield return new WaitForSeconds(m_TimeAfterAttack);
        isShooting = false;
    }
}