using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class RangeOverloadCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] float maxTemperature;
    [SerializeField] float shootTemperature;
    [SerializeField] float temperatureLostPerSec;
    float curentTemperature;

    [Space(10)]
    [SerializeField] float attackCooldown;
    [SerializeField] float overloadCooldown;
    [SerializeField] float timeToCoolsAfterShoot;

    [Space(10)]
    [SerializeField] int bulletDamage;
    [SerializeField] float bulletSpeed;
    [SerializeField] float knockBackForce;

    bool canAttack = true;
    bool isOverload = false;

    float coolsTimer;

    [Header("References")]
    [SerializeField] Transform attackPoint;

    [SerializeField] private RangeReloadingWeaponSFXManager m_SFXManager;

    //[Header("Input")]
    //[Header("Output")]

    private void Update()
    {
        coolsTimer += Time.deltaTime;

        if(coolsTimer > timeToCoolsAfterShoot && !isOverload)
        {
            Mathf.Clamp(curentTemperature -= temperatureLostPerSec * Time.deltaTime, 0, maxTemperature);
            OnAmmoChange?.Invoke(curentTemperature, maxTemperature);
        }
    }

    public override void Attack()
    {
        if (canAttack && !isOverload)
        {
            coolsTimer = 0;

            Bullet bullet = BulletManager.Instance.GetBullet();
            bullet.transform.position = attackPoint.position;
            bullet.transform.up = attackPoint.forward;

            bullet.Setup(bulletDamage, bulletSpeed)
                .SetKnockback(knockBackForce);

            StartCoroutine(AttackCooldown());

            if (m_SFXManager)
                m_SFXManager.PlayAttackSFX();

            curentTemperature += shootTemperature;
            if(curentTemperature >= maxTemperature)
            {
                curentTemperature = maxTemperature;
                Overload();
            }

            OnAmmoChange?.Invoke(curentTemperature, maxTemperature);
        }
    }

    public void Overload()
    {
        if (!isOverload)
        {
            OnReload?.Invoke();

            if (m_SFXManager)
                m_SFXManager.PlayReloadSFX();
            StartCoroutine(OverloadCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator OverloadCooldown()
    {
        isOverload = true;
        yield return new WaitForSeconds(overloadCooldown);
        isOverload = false;
    }
}