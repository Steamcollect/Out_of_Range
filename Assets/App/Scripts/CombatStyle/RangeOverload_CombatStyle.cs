using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RangeOverload_CombatStyle : CombatStyle
{
    [SerializeField, MVsToolkit.Dev.ReadOnly] float m_CurentTemperature;
    [SerializeField, MVsToolkit.Dev.ReadOnly] RangeOverloadWeaponState m_CurrentState;

    [Space(10)]
    [SerializeField, Tooltip("Delay between attacks")] float m_AttackCooldown;

    [Space(5)]
    [SerializeField, Tooltip("Temperature donnée par shoot")] float m_ShootTemperature;
    [SerializeField, Tooltip("Delay de bloquage avant de pouvoir recharger quand l'arme est surchauffé")] float m_StunDelayOnOverload;

    [Space(10)]
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_DefaultCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_OverloadCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand input raté")] float m_NerfCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand input réussis")] float m_BuffCoolsPerSec;

    [Space(5)]
    public Vector2 RangeToReset;
    public Vector2 RangeToBuff;
    public Vector2 RangeToNerf;

    [Space(10)]
    [SerializeField] MeshRenderer m_MeshRenderer;
    [SerializeField] Gradient m_ColorOverTemperature;

    [SerializeField] Transform m_AttackPoint;

    [SerializeField] GameObject m_MuzzleFlashPrefab;
    [SerializeField] Bullet m_BulletPrefab;

    [Space(10)]
    [SerializeField] UnityEvent m_OnAttackFeedback;
    [SerializeField] UnityEvent m_OnReloadFeedback;

    Material m_RendererMat;

    [Header("Input")]
    [SerializeField] InputActionReference m_HandleCoolsInput;
    [SerializeField] InputActionReference m_HandleCoolsSkillInput;

    //[Header("Output")]
    public Action OnOverloadBuff, OnOverloadNerf, OnOverloadReset;

    private void OnEnable()
    {
        m_HandleCoolsInput.action.Enable();
        m_HandleCoolsSkillInput.action.Enable();

        m_HandleCoolsInput.action.started += Cool;
        m_HandleCoolsSkillInput.action.started += TryBuffOnCool;
    }

    private void OnDisable()
    {
        m_HandleCoolsInput.action.started -= Cool;
        m_HandleCoolsSkillInput.action.started -= TryBuffOnCool;
    }

    private void Start()
    {
        m_RendererMat = new Material(m_MeshRenderer.material);
        m_MeshRenderer.material = m_RendererMat;
        SetRendererColor();
    }

    private void Update()
    {
        switch (m_CurrentState)
        {
            case RangeOverloadWeaponState.DefaultCool:
                HandleCool(m_DefaultCoolsPerSec);
                break;
            case RangeOverloadWeaponState.OverloadCool:
                HandleCool(m_DefaultCoolsPerSec);
                break;

            case RangeOverloadWeaponState.CoolBuffed:
                HandleCool(m_BuffCoolsPerSec);
                break;
            case RangeOverloadWeaponState.CoolNerfed:
                HandleCool(m_NerfCoolsPerSec);
                break;
        }
    }

    void HandleCool(float coolSpeed)
    {
        m_CurentTemperature -= coolSpeed * Time.deltaTime;

        if (m_CurentTemperature <= 0)
        {
            m_CurentTemperature = 0;
            m_CurrentState = RangeOverloadWeaponState.CanShoot;
        }

        OnAmmoChange?.Invoke(m_CurentTemperature, 100);
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack 
            && (m_CurrentState == RangeOverloadWeaponState.CanShoot || m_CurrentState == RangeOverloadWeaponState.CoolBuffed))
        {
            OnAttack?.Invoke();
            
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            StartCoroutine(AttackCooldown());
            
            m_OnAttackFeedback?.Invoke();

            if(m_CurrentState != RangeOverloadWeaponState.CoolBuffed)
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

    IEnumerator OnOverload()
    {
        m_CurentTemperature = 100;
        m_CurrentState = RangeOverloadWeaponState.Overload;

        yield return new WaitForSeconds(m_StunDelayOnOverload);

        m_CurrentState = RangeOverloadWeaponState.OverloadCool;
    }

    void Cool(InputAction.CallbackContext ctx)
    {
        switch (m_CurrentState)
        {
            case RangeOverloadWeaponState.CanShoot:
                m_CurrentState = RangeOverloadWeaponState.DefaultCool;
                OnReload?.Invoke();
                break;
        }
    }

    void TryBuffOnCool(InputAction.CallbackContext ctx)
    {
        if (m_CurrentState != RangeOverloadWeaponState.OverloadCool) return;

        if (m_CurentTemperature.InRange(RangeToBuff))
        {
            m_CurrentState = RangeOverloadWeaponState.CoolBuffed;
            m_CurentTemperature = 100;

            OnOverloadBuff?.Invoke();
        }
        else if (m_CurentTemperature.InRange(RangeToReset))
        {
            m_CurrentState = RangeOverloadWeaponState.CanShoot;
            m_CurentTemperature = 0;
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);

            OnOverloadReset?.Invoke();
        }
        else if (m_CurentTemperature.InRange(RangeToNerf))
        {
            m_CurrentState = RangeOverloadWeaponState.CoolNerfed;
            m_CurentTemperature = 100;

            OnOverloadNerf?.Invoke();
        }
    }

    private IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    private void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature * .001f);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }

    public RangeOverloadWeaponState GetState()
    {
        return m_CurrentState;
    }

    private void OnValidate()
    {
        RangeToReset.x = Mathf.Clamp(RangeToReset.x, 0, 100);
        RangeToReset.y = Mathf.Clamp(RangeToReset.y, 0, 100);

        RangeToBuff.x = Mathf.Clamp(RangeToBuff.x, 0, 100);
        RangeToBuff.y = Mathf.Clamp(RangeToBuff.y, 0, 100);

        RangeToNerf.x = Mathf.Clamp(RangeToNerf.x, 0, 100);
        RangeToNerf.y = Mathf.Clamp(RangeToNerf.y, 0, 100);
    }
}

public enum RangeOverloadWeaponState
{
    CanShoot = 0,
    Overload = 1,
    DefaultCool = 2,
    OverloadCool = 3,
    CoolBuffed = 4,
    CoolNerfed = 5
}