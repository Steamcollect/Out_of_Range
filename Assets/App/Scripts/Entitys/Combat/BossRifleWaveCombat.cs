using System;
using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

public class BossRifleWaveCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] private int m_BulletCount;
    [SerializeField] private float m_BulletCountAtkSpeedPowerUp;
    [SerializeField] float m_AtkSpeedBulletSpeedMult = 1.5f;
    [SerializeField] float m_CloneBulletSpacing;

    [Space(5)]
    [SerializeField] private float m_TimeBeforeAttack;
    [SerializeField] private float m_TimeAfterAttack;
    [SerializeField] private float m_TimeBetweenAttacks;

    [Space(5)]
    [SerializeField] float m_StartAngle;
    [SerializeField] float m_RotationTime;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private Bullet m_BulletPrefab;
    [SerializeField] private Bullet m_StrenghtBulletPrefab;
    [SerializeField] GameObject m_MuzzleFlashPrefab;
    [Space(5)]
    [SerializeField] private RSO_PlayerController m_Player;
    [SerializeField] BossEnemyController m_Controller;

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnShoot;

    public event Action<float /*DelayBeforeShoot*/, float /*DelayAfterShoot*/> OnShootLaunched;
    public event Action OnShootCompleted;

    public override IEnumerator Attack()
    {
        if (!m_CanAttackOnSpawn) yield break;

        StartCoroutine(Rotate());

        m_IsAttacking = true;
        OnShootLaunched?.Invoke(m_TimeBeforeAttack, m_TimeAfterAttack);

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

            yield return new WaitForSeconds(m_RotationTime / (m_Controller.HaveAtkSpeedPowerUp ? m_BulletCountAtkSpeedPowerUp : m_BulletCount));
        }

        yield return new WaitForSeconds(m_TimeAfterAttack);
        SetTurnSmoothTime(m_TurnSmoothTime);
        OnShootCompleted?.Invoke();

        yield return new WaitForSeconds(m_TimeBetweenAttacks);
        m_IsAttacking = false;
    }

    IEnumerator Rotate()
    {
        float t = 0;

        while (t < m_RotationTime)
        {
            t += Time.deltaTime;
            if (t > m_RotationTime) t = m_RotationTime;

            m_HorizontalPivot.localRotation = Quaternion.Euler(
                0,
                Mathf.Lerp(-m_StartAngle, m_StartAngle, t / m_RotationTime),
                0);

            yield return null;
        }

        m_HorizontalPivot.localRotation = Quaternion.Euler(
            0,
            m_StartAngle,
            0);
    }
}