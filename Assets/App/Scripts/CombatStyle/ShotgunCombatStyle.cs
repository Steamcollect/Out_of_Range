using System.Collections;
using UnityEngine;

public class ShotgunCombatStyle : OverloadCombatStyle
{
    [Header("Combat Settings")]
    [SerializeField] float m_AtkSpdPowerUpAttackCooldown;
    [SerializeField] float m_TimeBetweenClonePowerUp;

    [Space(10)]
    [SerializeField] int m_BulletsPerShot = 8;
    [SerializeField] float m_SpreadAngle = 15f;

    [Header("Combat References")]
    [SerializeField] Transform m_AttackPoint;
    
    [SerializeField] Bullet m_BulletPrefab;
    [SerializeField] Bullet m_StrenghtPowerUpBulletPrefab;

    [Space(10)]
    [SerializeField] RSO_CurrentPowerUp m_CurrentPowerUp;

    public override IEnumerator Attack()
    {
        if (m_CanAttack
            && (m_CurrentState == OverloadWeaponState.CanShoot || m_CurrentState == OverloadWeaponState.CoolBuffed))
        {
            OnAttack?.Invoke();

            if(m_CurrentPowerUp.Get() != null && m_CurrentPowerUp.Get().PowerUpType == PowerUpType.Clone)
            {
                CreateBullets();
                yield return new WaitForSeconds(m_TimeBetweenClonePowerUp);
                CreateBullets();
            }
            else CreateBullets();

            StartCoroutine(AttackCooldown(
                m_CurrentPowerUp.Get() != null
                && m_CurrentPowerUp.Get().PowerUpType == PowerUpType.AttackSpeed ?
                m_AtkSpdPowerUpAttackCooldown : m_AttackCooldown)); m_OnAttackFeedback?.Invoke();


            if (m_CurrentState != OverloadWeaponState.CoolBuffed)
            {
                m_CurentTemperature += m_ShootTemperature;
                if (m_CurentTemperature >= 100)
                    StartCoroutine(OnOverload());
            }

            OnAmmoChange?.Invoke(m_CurentTemperature, 100);

            yield break;
        }
    }

    void CreateBullets()
    {
        for (int i = 0; i < m_BulletsPerShot; i++)
        {
            float yaw;
            if (m_BulletsPerShot == 1)
            {
                yaw = 0f;
            }
            else
            {
                float step = m_SpreadAngle / (m_BulletsPerShot - 1);
                yaw = -m_SpreadAngle / 2f + step * i;
            }

            Quaternion spreadRotation = m_AttackPoint.rotation * Quaternion.Euler(0f, yaw, 0f);

            Bullet bullet;
            if (m_CurrentPowerUp.Get() != null && m_CurrentPowerUp.Get().PowerUpType == PowerUpType.Strenght)
                bullet = PoolManager.Instance.Spawn(m_StrenghtPowerUpBulletPrefab, m_AttackPoint.position, spreadRotation);
            else
                bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, spreadRotation);
            bullet.Setup();
        }
    }
}
