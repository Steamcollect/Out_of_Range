using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RangeOverloadCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] private float m_MaxTemperature;

    [SerializeField] private float m_ShootTemperature;
    [SerializeField] private float m_TemperatureLostPerSec;

    [Space(5)] 
    [SerializeField] private float m_AttackCooldown;

    [SerializeField] private float m_OverloadCooldown;
    [SerializeField] private float m_OverloadRecorverySpeed;
    [SerializeField] private float m_TimeToCoolsAfterShoot;

    [Space(10)] 
    [SerializeField] private int m_BulletDamage;

    [SerializeField] private float m_BulletSpeed;
    [SerializeField] private float m_KnockBackForce;

    [Header("Visual")]
    [SerializeField] private MeshRenderer m_MeshRenderer;

    [SerializeField] private Gradient m_ColorOverTemperature;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;

    [SerializeField] private GameObject m_MuzzleFlashPrefab;
    [SerializeField] private Bullet m_BulletPrefab;

    [SerializeField] private RangeReloadingWeaponSFXManager m_SfxManager;
    [SerializeField] private UnityEvent m_OnAttackFeedback;

    private float m_CoolsTimer;
    private float m_CurentTemperature;

    private bool m_IsOverload;

    private Material m_RendererMat;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_RendererMat = new Material(m_MeshRenderer.material);
        m_MeshRenderer.material = m_RendererMat;
        SetRendererColor();
    }

    private void Update()
    {
        m_CoolsTimer += Time.deltaTime;

        if (m_CoolsTimer > m_TimeToCoolsAfterShoot && !m_IsOverload)
        {
            m_CurentTemperature =
                Mathf.Clamp(m_CurentTemperature - m_TemperatureLostPerSec * Time.deltaTime, 0, m_MaxTemperature);
            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, m_MaxTemperature);
        }
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack && !m_IsOverload)
        {
            m_OnAttackFeedback?.Invoke();
            OnAttack?.Invoke();
            m_CoolsTimer = 0;
            
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            StartCoroutine(AttackCooldown());

            if (m_SfxManager)
                m_SfxManager.PlayAttackSfx();

            m_CurentTemperature += m_ShootTemperature;
            if (m_CurentTemperature >= m_MaxTemperature)
            {
                m_CurentTemperature = m_MaxTemperature;
                Overload();
            }

            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, m_MaxTemperature);

            yield break;
        }
    }

    public void Overload()
    {
        if (!m_IsOverload)
        {
            OnReload?.Invoke();

            if (m_SfxManager)
                m_SfxManager.PlayReloadSfx();
            StartCoroutine(OverloadCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    private IEnumerator OverloadCooldown()
    {
        m_IsOverload = true;
        yield return new WaitForSeconds(m_OverloadCooldown);

        while (m_CurentTemperature > 0)
        {
            m_CurentTemperature -= m_OverloadRecorverySpeed * Time.deltaTime;
            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, m_MaxTemperature);
            yield return null;
        }

        m_CurentTemperature = 0;

        m_IsOverload = false;
    }

    private void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature / m_MaxTemperature);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }
}