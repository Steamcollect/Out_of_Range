using System.Collections;
using UnityEngine;

public class RangeOverload_CombatStyle : OverloadCombatStyle
{
    [Header("Combat Settings")]
    [SerializeField] float m_AtkSpdPowerUpAttackCooldown;
    [SerializeField] float m_ClonePowerUpBulletSpacing;

    bool strength = false;
    bool clone = false;
    bool atkSpeed = false;

    [Header("Combat References")]
    [SerializeField] Collider m_CollidToIgnore;

    [SerializeField] MeshRenderer m_MeshRenderer;
    [SerializeField] Gradient m_ColorOverTemperature;
    Material m_RendererMat;

    [SerializeField] Transform m_AttackPoint;

    [SerializeField] GameObject m_MuzzleFlashPrefab;

    [Header("Bullets")]
    [SerializeField] Bullet m_BulletPrefab;
    [SerializeField] Bullet m_StrenghtPowerUpBulletPrefab;
    [Space(5)]
    [SerializeField] Bullet m_OverloadBulletPrefab;
    [SerializeField] Bullet m_StrengthPowerUpOverloadBulletPrefab;

    [Space(10)]
    [SerializeField] RSO_PlayerController m_CurrentPowerUp;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        //m_RendererMat = new Material(m_MeshRenderer.material);
        //m_MeshRenderer.material = m_RendererMat;
        //SetRendererColor();
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack 
            && (m_CurrentState == OverloadWeaponState.CanShoot || m_CurrentState == OverloadWeaponState.CoolBuffed))
        {
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

                Vector3 pos = m_AttackPoint.position + m_AttackPoint.transform.right * (spacing * .5f);
                Bullet bullet = PoolManager.Instance.Spawn(bulletPrefab, pos, m_AttackPoint.rotation);
                bullet.Setup().AvoidCollider(m_CollidToIgnore);

                pos = m_AttackPoint.position + -m_AttackPoint.transform.right * (spacing * .5f);
                bullet = PoolManager.Instance.Spawn(bulletPrefab, pos, m_AttackPoint.rotation);
                bullet.Setup().AvoidCollider(m_CollidToIgnore);
            }
            else
            {
                Bullet bullet = PoolManager.Instance.Spawn(bulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
                bullet.Setup().AvoidCollider(m_CollidToIgnore);
            }               

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

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
}