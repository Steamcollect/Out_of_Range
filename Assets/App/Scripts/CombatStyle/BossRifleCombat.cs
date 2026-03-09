using MVsToolkit.Dev;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class BossRifleCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] private int m_BulletCount;
    [SerializeField] private float m_BulletCountAtkSpeedPowerUp;
    [SerializeField] private float m_TimeBetweenBullets;
    [SerializeField] private float m_TimeBetweenBulletsAtkSpeedPowerUp;
    [SerializeField] float m_AtkSpeedBulletSpeedMult = 1.5f;
    [SerializeField] float m_CloneBulletSpacing;
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
    [SerializeField] private Bullet m_StrenghtBulletPrefab;
    [SerializeField] GameObject m_MuzzleFlashPrefab;
    [Space(5)]
    [SerializeField] private RSO_PlayerController m_Player;
    [SerializeField] BossEnemyController m_Controller;
    [Space(5)]
    [SerializeField] private VisualEffect m_ShootVFX;

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnShoot;

    public event Action<float /*DelayBeforeShoot*/, float /*DelayAfterShoot*/> OnShootLaunched;
    public event Action OnShootCompleted;

    private void Start()
    {
        m_ShootVFX.gameObject.SetActive(true);
    }

    public override IEnumerator Attack()
    {
        if (!m_CanAttackOnSpawn) yield break;

        SetTurnSmoothTime(m_TurnWhileShooting ? m_TurnSmoothTimeOnShoot : 0f);
        m_IsAttacking = true;
        OnShootLaunched?.Invoke(m_TimeBeforeAttack, m_TimeAfterAttack);

        float bulletCount = m_Controller.HaveAtkSpeedPowerUp ? m_BulletCountAtkSpeedPowerUp : m_BulletCount;

        float loopDuration = bulletCount * (m_Controller.HaveAtkSpeedPowerUp ? m_TimeBetweenBulletsAtkSpeedPowerUp : m_TimeBetweenBullets);

        m_ShootVFX.SetFloat("EffectDuration", m_TimeBeforeAttack + loopDuration);
        m_ShootVFX.SetFloat("SpawnDuration", m_TimeBeforeAttack);
        m_ShootVFX.gameObject.SetActive(true);

        yield return new WaitForSeconds(m_TimeBeforeAttack);

        for (int i = 0; i < (m_Controller.HaveAtkSpeedPowerUp ? m_BulletCountAtkSpeedPowerUp : m_BulletCount); i++)
        {
            if (!m_Controller.HaveClonePowerUp)
            {
                Vector3 position = m_AttackPoint.position;
                Bullet bullet = PoolManager.Instance.Spawn(m_Controller.HaveStrenghtPowerUp ? m_StrenghtBulletPrefab : m_BulletPrefab, position, m_AttackPoint.rotation);
                bullet.Setup(m_Controller.HaveAtkSpeedPowerUp ? m_AtkSpeedBulletSpeedMult : 1);
            }
            else
            {
                Vector3 position = m_AttackPoint.position + m_AttackPoint.right * m_CloneBulletSpacing * .5f;
                Bullet bullet = PoolManager.Instance.Spawn(m_Controller.HaveStrenghtPowerUp ? m_StrenghtBulletPrefab : m_BulletPrefab, position, m_AttackPoint.rotation);
                bullet.Setup(m_Controller.HaveAtkSpeedPowerUp ? m_AtkSpeedBulletSpeedMult : 1);

                position = m_AttackPoint.position + -m_AttackPoint.right * m_CloneBulletSpacing * .5f;
                bullet = PoolManager.Instance.Spawn(m_Controller.HaveStrenghtPowerUp ? m_StrenghtBulletPrefab : m_BulletPrefab, position, m_AttackPoint.rotation);
                bullet.Setup(m_Controller.HaveAtkSpeedPowerUp ? m_AtkSpeedBulletSpeedMult : 1);
            }


                GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            m_OnShoot.Invoke();

            yield return new WaitForSeconds(m_Controller.HaveAtkSpeedPowerUp ? m_TimeBetweenBulletsAtkSpeedPowerUp : m_TimeBetweenBullets);
        }

        yield return new WaitForSeconds(m_TimeAfterAttack);
        SetTurnSmoothTime(m_TurnSmoothTime);
        OnShootCompleted?.Invoke();

        yield return new WaitForSeconds(m_TimeBetweenAttacks);
        m_ShootVFX.gameObject.SetActive(false);
        m_IsAttacking = false;
    }
}
