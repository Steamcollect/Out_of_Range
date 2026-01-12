using System.Collections;
using UnityEngine;

public class RangeOverload_CombatStyle : OverloadCombatStyle
{
    [Header("Combat Settings")]
    [SerializeField] MeshRenderer m_MeshRenderer;
    [SerializeField] Gradient m_ColorOverTemperature;

    [SerializeField] Transform m_AttackPoint;

    [SerializeField] GameObject m_MuzzleFlashPrefab;
    [SerializeField] Bullet m_BulletPrefab;

    Material m_RendererMat;

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
            
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            StartCoroutine(AttackCooldown());
            
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