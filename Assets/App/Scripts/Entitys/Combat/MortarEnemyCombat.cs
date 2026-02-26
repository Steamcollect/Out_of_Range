using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MortarEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] float m_RandomTargetRadius;

    [SerializeField] private int m_GrenadeCount;
    [SerializeField] float m_AtkSpeedGrenadesCount;
    [SerializeField] private float m_TimeBetweenGrenades;
    [SerializeField] float m_AtkSpeedTimeBetweenGrenades;
    [Space(5)]
    [SerializeField] private float m_TimeBeforeAttack;
    [SerializeField] private float m_TimeAfterAttack;
    [SerializeField] private float m_TimeBetweenAttacks;

    [Space(5)]
    [SerializeField] float m_StrenghtGrenadeSpeedMult = 1.5f;
    [SerializeField] float m_StrenghtExplosionRadiusMult = 1.5f;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private Grenade m_GrenadePrefab;
    [SerializeField] GameObject m_MuzzleFlashPrefab;
    [Space(5)]
    [SerializeField] BossEnemyController m_Controller;

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

        for (int i = 0; i < (m_Controller.HaveAtkSpeedPowerUp ? m_AtkSpeedGrenadesCount : m_GrenadeCount); i++)
        {
            LaunchGrenade();
            if (m_Controller.HaveClonePowerUp) LaunchGrenade();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            m_OnShoot.Invoke();

            yield return new WaitForSeconds(m_Controller.HaveAtkSpeedPowerUp ? m_AtkSpeedTimeBetweenGrenades : m_TimeBetweenGrenades);
        }

        yield return new WaitForSeconds(m_TimeAfterAttack);
        SetTurnSmoothTime(m_TurnSmoothTime);
        OnShootCompleted?.Invoke();

        yield return new WaitForSeconds(m_TimeBetweenAttacks);
        m_IsAttacking = false;
    }

    void LaunchGrenade()
    {
        Vector3 targetPos = m_Player.Get().GetTargetPosition();
        Vector2 rnd = UnityEngine.Random.insideUnitCircle * m_RandomTargetRadius;
        targetPos.x += rnd.x;
        targetPos.z += rnd.y;

        Grenade grenade = PoolManager.Instance.Spawn(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        
        // Grenade grenade = Instantiate(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        grenade.Setup(m_AttackPoint.position, targetPos);

        if (m_Controller.HaveStrenghtPowerUp) grenade.Move(m_StrenghtGrenadeSpeedMult, m_StrenghtExplosionRadiusMult);
        else grenade.Move();
    }
}