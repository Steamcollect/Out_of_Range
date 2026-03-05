using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;

public class RangeOverloadCombatStyle : OverloadCombatStyle
{
    [Header("Combat Settings")]
    [SerializeField] float m_AtkSpdPowerUpAttackCooldown;
    [SerializeField] float m_ClonePowerUpBulletSpacing;

    [SerializeField] bool m_CanShoot = true;

    bool strength = false;
    bool clone = false;
    bool atkSpeed = false;

    [Header("Combat References")]
    [SerializeField] PlayerArms m_Arms;

    [Space]
    [SerializeField] Collider m_CollidToIgnore;

    [SerializeField] MeshRenderer m_MeshRenderer;
    [SerializeField] Gradient m_ColorOverTemperature;
    Material m_RendererMat;

    [SerializeField] GameObject m_MuzzleFlashPrefab;

    [Header("Bullets")]
    [SerializeField] Bullet m_BulletPrefab;
    [SerializeField] Bullet m_StrenghtPowerUpBulletPrefab;
    [Space(5)]
    [SerializeField] Bullet m_OverloadBulletPrefab;
    [SerializeField] Bullet m_StrengthPowerUpOverloadBulletPrefab;

    [Space(10)]
    [SerializeField] RSO_PlayerController m_CurrentPowerUp;

    [Header("Input")]
    [SerializeField] RSE_SetRifleCanShoot m_SetCanShoot;

    //[Header("Output")]

    public override void OnEnable()
    {
        base.OnEnable();
        m_SetCanShoot.Action += SetCanShoot;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        m_SetCanShoot.Action -= SetCanShoot;
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack && m_CanShoot
            && (m_CurrentState == OverloadWeaponState.CanShoot || m_CurrentState == OverloadWeaponState.CoolBuffed))
        {
            Transform attackPoint = m_Arms.LeftArmAttack();

            OnAttack?.Invoke();
   
            strength = m_CurrentPowerUp.Value.GetPowerUp().ContainPowerUp(PowerUpType.Strength);
            clone = m_CurrentPowerUp.Value.GetPowerUp().ContainPowerUp(PowerUpType.Clone);
            atkSpeed = m_CurrentPowerUp.Value.GetPowerUp().ContainPowerUp(PowerUpType.AttackSpeed);

            Bullet bulletPrefab = m_CurrentPowerUp.Get() != null && strength ?
                (m_CurrentState == OverloadWeaponState.CoolBuffed ?
                    m_StrengthPowerUpOverloadBulletPrefab : m_StrenghtPowerUpBulletPrefab) :
                (m_CurrentState == OverloadWeaponState.CoolBuffed ?
                    m_OverloadBulletPrefab : m_BulletPrefab);

            if(m_CurrentPowerUp.Get() != null && clone)
            {
                float spacing = strength ? m_ClonePowerUpBulletSpacing * 2f : 1;

                Vector3 pos = attackPoint.position + attackPoint.transform.right * (spacing * .5f);
                Bullet bullet = PoolManager.Instance.Spawn(bulletPrefab, pos, attackPoint.rotation);
                bullet.Setup().AvoidCollider(m_CollidToIgnore);

                pos = attackPoint.position + -attackPoint.transform.right * (spacing * .5f);
                bullet = PoolManager.Instance.Spawn(bulletPrefab, pos, attackPoint.rotation);
                bullet.Setup().AvoidCollider(m_CollidToIgnore);
            }
            else
            {
                Bullet bullet = PoolManager.Instance.Spawn(bulletPrefab, attackPoint.position, attackPoint.rotation);
                bullet.Setup().AvoidCollider(m_CollidToIgnore);
            }

            PoolManager.Instance.Spawn(m_MuzzleFlashPrefab, attackPoint.position, attackPoint.rotation);

            StartCoroutine(AttackCooldown(
                m_CurrentPowerUp.Get() != null
                && atkSpeed ?
                m_AtkSpdPowerUpAttackCooldown : m_AttackCooldown));
            
            m_OnAttackFeedback?.Invoke();

            OnShootHeat();
            //SetRendererColor();

            yield break;
        }
    }

    private void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature * .001f);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }

    public void SetCanShoot(bool canShoot)
    {
        m_CanShoot = canShoot;
    }
}