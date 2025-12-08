using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class RangeReloadingCombatStyle : CombatStyle
{
    [FormerlySerializedAs("maxBulletCount")]
    [Header("Settings")]
    [SerializeField] private int m_MaxBulletCount;

    [FormerlySerializedAs("bulletDamage")] [Space(10)] [SerializeField] private int m_BulletDamage;

    [FormerlySerializedAs("bulletSpeed")] [SerializeField] private float m_BulletSpeed;
    [FormerlySerializedAs("knockBackForce")] [SerializeField] private float m_KnockBackForce;

    [FormerlySerializedAs("attackCooldown")] [Space(10)] [SerializeField] private float m_AttackCooldown;

    [FormerlySerializedAs("reloadCooldown")] [SerializeField] private float m_ReloadCooldown;

    [FormerlySerializedAs("attackPoint")]
    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;

    [SerializeField] private GameObject m_MuzzleFlashPrefab;

    [FormerlySerializedAs("m_SFXManager")] [SerializeField] private RangeReloadingWeaponSFXManager m_SfxManager;

    private int m_CurrentBulletCount;

    private bool m_IsReloading;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(.1f);
        m_CurrentBulletCount = m_MaxBulletCount;
    }

    public override void Attack()
    {
        if (m_CanAttack && !m_IsReloading)
        {
            if (m_CurrentBulletCount > 0)
            {
                OnAttack?.Invoke();

                m_CurrentBulletCount--;
                OnAmmoChange?.Invoke(m_CurrentBulletCount, m_MaxBulletCount);
                GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
                Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);
                Bullet bullet = BulletManager.S_Instance.GetBullet();
                bullet.transform.position = m_AttackPoint.position;
                bullet.transform.up = m_AttackPoint.forward;

                bullet.Setup(m_BulletDamage, m_BulletSpeed)
                    .SetKnockback(m_KnockBackForce);

                StartCoroutine(AttackCooldown());

                if (m_SfxManager)
                    m_SfxManager.PlayAttackSfx();
            }
            else
            {
                Reload();
            }
        }
    }

    public override void Reload()
    {
        if (!m_IsReloading)
        {
            OnReload?.Invoke();

            if (m_SfxManager)
                m_SfxManager.PlayReloadSfx();
            StartCoroutine(ReloadCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    private IEnumerator ReloadCooldown()
    {
        m_IsReloading = true;
        yield return new WaitForSeconds(m_ReloadCooldown);
        m_CurrentBulletCount = m_MaxBulletCount;
        OnAmmoChange?.Invoke(m_CurrentBulletCount, m_MaxBulletCount);
        m_IsReloading = false;
    }
}