using System.Collections;
using UnityEngine;

public class RangeReloadingCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] int maxBulletCount;
    int currentBulletCount;

    [Space(10)]
    [SerializeField] int bulletDamage;
    [SerializeField] float bulletSpeed;
    [SerializeField] float knockBackForce;

    [Space(10)]
    [SerializeField] float attackCooldown;
    [SerializeField] float reloadCooldown;

    bool isReloading = false;

    [Header("References")]
    [SerializeField] Transform attackPoint;
    [SerializeField] private GameObject m_MuzzleFlashPrefab;

    [SerializeField] private RangeReloadingWeaponSFXManager m_SFXManager;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(.1f);
        currentBulletCount = maxBulletCount;
    }

    public override void Attack()
    {
        if(canAttack && !isReloading)
        {
            if(currentBulletCount > 0)
            {
                OnAttack?.Invoke();

                currentBulletCount--;
                OnAmmoChange?.Invoke(currentBulletCount, maxBulletCount);
                var muzzleVFX = Instantiate(m_MuzzleFlashPrefab, attackPoint);
                Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);
                Bullet bullet = BulletManager.Instance.GetBullet();
                bullet.transform.position = attackPoint.position;
                bullet.transform.up = attackPoint.forward;

                bullet.Setup(bulletDamage, bulletSpeed)
                    .SetKnockback(knockBackForce);

                StartCoroutine(AttackCooldown());
                
                if(m_SFXManager)
                    m_SFXManager.PlayAttackSFX();
            }
            else
            {
                Reload();
            }
        }
    }

    public override void Reload()
    {
        if (!isReloading)
        {
            OnReload?.Invoke();

            if(m_SFXManager)
                m_SFXManager.PlayReloadSFX();
            StartCoroutine(ReloadCooldown());         
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator ReloadCooldown()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadCooldown);
        currentBulletCount = maxBulletCount;
        OnAmmoChange?.Invoke(currentBulletCount, maxBulletCount);
        isReloading = false;
    }
}