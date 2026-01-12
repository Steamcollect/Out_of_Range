using System.Collections;
using UnityEngine;

public class ShotgunCombatStyle : OverloadCombatStyle
{
    [SerializeField] int m_BulletsPerShot = 8;
    [SerializeField] float m_SpreadAngle = 15f;

    [Space(10)]
    [SerializeField] Transform m_AttackPoint;
    
    [SerializeField] Bullet m_BulletPrefab;

    public override IEnumerator Attack()
    {
        if (m_CanAttack
            && (m_CurrentState == OverloadWeaponState.CanShoot || m_CurrentState == OverloadWeaponState.CoolBuffed))
        {
            OnAttack?.Invoke();

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
                Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, spreadRotation);

                bullet.Setup();
            }

            StartCoroutine(AttackCooldown());
            m_OnAttackFeedback?.Invoke();

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
}
