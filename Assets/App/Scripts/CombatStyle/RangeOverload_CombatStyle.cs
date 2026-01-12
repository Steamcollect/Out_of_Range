using System.Collections;
using UnityEngine;

public class RangeOverload_CombatStyle : OverloadCombatStyle
{
    [Header("Combat Settings")]
    [SerializeField] float m_AtkSpdPowerUpAttackCooldown;
    [SerializeField] float m_ClonePowerUpBulletSpacing;

    [Header("Combat References")]
    [SerializeField] MeshRenderer m_MeshRenderer;
    [SerializeField] Gradient m_ColorOverTemperature;
    Material m_RendererMat;

    [SerializeField] Transform m_AttackPoint;

    [SerializeField] GameObject m_MuzzleFlashPrefab;

    [Header("Bullets")]
    [SerializeField] Bullet m_BulletPrefab;
    [SerializeField] Bullet m_StrenghtPowerUpBulletPrefab;

    [Space(10)]
    [SerializeField] RSO_CurrentPowerUp m_CurrentPowerUp;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_RendererMat = new Material(m_MeshRenderer.material);
        m_MeshRenderer.material = m_RendererMat;
        SetRendererColor();
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack 
            && (m_CurrentState == OverloadWeaponState.CanShoot || m_CurrentState == OverloadWeaponState.CoolBuffed))
        {
            OnAttack?.Invoke();
                        
            Bullet bulletPrefab = m_CurrentPowerUp.Get() != null && m_CurrentPowerUp.Get().PowerUpType == PowerUpType.Strenght ?
                m_StrenghtPowerUpBulletPrefab :
                m_BulletPrefab;
            
            if(m_CurrentPowerUp.Get() != null && m_CurrentPowerUp.Get().PowerUpType == PowerUpType.Clone)
            {
                Vector3 pos = m_AttackPoint.position + m_AttackPoint.transform.right * (m_ClonePowerUpBulletSpacing * .5f);
                Bullet bullet = PoolManager.Instance.Spawn(bulletPrefab, pos, m_AttackPoint.rotation);
                bullet.Setup();
                
                pos = m_AttackPoint.position + -m_AttackPoint.transform.right * (m_ClonePowerUpBulletSpacing * .5f);
                bullet = PoolManager.Instance.Spawn(bulletPrefab, pos, m_AttackPoint.rotation);
                bullet.Setup();
            }
            else
            {
                Bullet bullet = PoolManager.Instance.Spawn(bulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
                bullet.Setup();
            }               

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            StartCoroutine(AttackCooldown(
                m_CurrentPowerUp.Get() != null
                && m_CurrentPowerUp.Get().PowerUpType == PowerUpType.AttackSpeed ?
                m_AtkSpdPowerUpAttackCooldown : m_AttackCooldown));
            
            m_OnAttackFeedback?.Invoke();

            if(m_CurrentState != OverloadWeaponState.CoolBuffed)
            {
                m_CurentTemperature += m_ShootTemperature;
                if (m_CurentTemperature >= 100)
                    StartCoroutine(OnOverload());
            }

            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);

            yield break;
        }
    }

    private void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature * .001f);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }
}