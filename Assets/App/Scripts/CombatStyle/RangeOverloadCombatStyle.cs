using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RangeOverloadCombatStyle : CombatStyle
{
    [FormerlySerializedAs("maxTemperature")]
    [Header("Settings")]
    [SerializeField] private float m_MaxTemperature;

    [FormerlySerializedAs("shootTemperature")] [SerializeField] private float m_ShootTemperature;
    [FormerlySerializedAs("temperatureLostPerSec")] [SerializeField] private float m_TemperatureLostPerSec;

    [FormerlySerializedAs("attackCooldown")] [Space(5)] [SerializeField] private float m_AttackCooldown;

    [FormerlySerializedAs("overloadCooldown")] [SerializeField] private float m_OverloadCooldown;
    [FormerlySerializedAs("overloadRecorverySpeed")] [SerializeField] private float m_OverloadRecorverySpeed;
    [FormerlySerializedAs("timeToCoolsAfterShoot")] [SerializeField] private float m_TimeToCoolsAfterShoot;

    [FormerlySerializedAs("bulletDamage")] [Space(10)] [SerializeField] private int m_BulletDamage;

    [FormerlySerializedAs("bulletSpeed")] [SerializeField] private float m_BulletSpeed;
    [FormerlySerializedAs("knockBackForce")] [SerializeField] private float m_KnockBackForce;

    [FormerlySerializedAs("meshRenderer")]
    [Header("Visual")]
    [SerializeField] private MeshRenderer m_MeshRenderer;

    [FormerlySerializedAs("colorOverTemperature")] [SerializeField] private Gradient m_ColorOverTemperature;

    [FormerlySerializedAs("attackPoint")]
    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;

    [SerializeField] private GameObject m_MuzzleFlashPrefab;

    [FormerlySerializedAs("m_SFXManager")] [SerializeField] private RangeReloadingWeaponSFXManager m_SfxManager;
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

    public override void Attack()
    {
        if (m_CanAttack && !m_IsOverload)
        {
            m_OnAttackFeedback?.Invoke();
            OnAttack?.Invoke();
            m_CoolsTimer = 0;

            Bullet bullet = BulletManager.S_Instance.GetBullet();
            bullet.transform.position = m_AttackPoint.position;
            bullet.transform.up = m_AttackPoint.forward;
            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);
            bullet.Setup(m_BulletDamage, m_BulletSpeed)
                .SetKnockback(m_KnockBackForce);

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