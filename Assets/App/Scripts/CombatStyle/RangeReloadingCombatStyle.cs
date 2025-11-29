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
    
    [Space(10)]
    [SerializeField] float attackCooldown;
    [SerializeField] float reloadCooldown;

    bool canAttack = true;
    bool isReloading = false;

    [Header("References")]
    [SerializeField] Transform attackPoint;

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
                currentBulletCount--;
                OnAMMOChange?.Invoke(currentBulletCount, maxBulletCount);

                Bullet bullet = BulletManager.Instance.GetBullet();
                bullet.transform.position = attackPoint.position;
                bullet.transform.up = attackPoint.forward;

                bullet.Setup(bulletDamage, bulletSpeed);

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
        OnAMMOChange?.Invoke(currentBulletCount, maxBulletCount);
        isReloading = false;
    }
}