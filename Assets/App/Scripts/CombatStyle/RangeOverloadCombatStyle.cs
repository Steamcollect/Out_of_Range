using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class RangeOverloadCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] float maxTemperature;
    [SerializeField] float shootTemperature;
    [SerializeField] float temperatureLostPerSec;
    float curentTemperature;

    [Space(5)]
    [SerializeField] float attackCooldown;
    [SerializeField] float overloadCooldown;
    [SerializeField] float overloadRecorverySpeed;
    [SerializeField] float timeToCoolsAfterShoot;

    [Header("Bullet")]
    [SerializeField] private int bulletDamage;
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float knockBackForce;

    bool isOverload = false;

    float coolsTimer;

    [Header("Visual")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Gradient colorOverTemperature;
    Material rendererMat;

    [Header("References")]
    [SerializeField] Transform attackPoint;
    [SerializeField] private GameObject m_MuzzleFlashPrefab;

    [SerializeField] private RangeReloadingWeaponSFXManager m_SFXManager;
    [SerializeField] private UnityEvent m_OnAttackFeedback;
    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        rendererMat = new Material(meshRenderer.material);
        meshRenderer.material = rendererMat;
        SetRendererColor();
    }

    private void Update()
    {
        coolsTimer += Time.deltaTime;

        if(coolsTimer > timeToCoolsAfterShoot && !isOverload)
        {
            curentTemperature = Mathf.Clamp(curentTemperature - temperatureLostPerSec * Time.deltaTime, 0, maxTemperature);
            SetRendererColor();
            OnAmmoChange?.Invoke(curentTemperature, maxTemperature);
        }
    }

    public override void Attack()
    {
        if (canAttack && !isOverload)
        {
            m_OnAttackFeedback?.Invoke();
            OnAttack?.Invoke();
            coolsTimer = 0;

            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, attackPoint.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.transform.up = attackPoint.forward;
            GameObject muzzleVFX = PoolManager.Instance.Spawn(m_MuzzleFlashPrefab, attackPoint.position, attackPoint.rotation);
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

            SetRendererColor();
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

        while(curentTemperature > 0)
        {
            curentTemperature -= overloadRecorverySpeed * Time.deltaTime;
            SetRendererColor();
            OnAmmoChange?.Invoke(curentTemperature, maxTemperature);
            yield return null;
        }
        curentTemperature = 0;

        isOverload = false;
    }

    void SetRendererColor()
    {
        float value = Mathf.Clamp01(curentTemperature / maxTemperature);
        rendererMat.color = colorOverTemperature.Evaluate(value);
    }
}