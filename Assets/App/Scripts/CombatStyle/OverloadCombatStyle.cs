using System;
using System.Collections;
using MVsToolkit.Dev;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class OverloadCombatStyle : CombatStyle
{
    [FoldoutGroup("Overload Settings")]
    [SerializeField, FoldoutGroup("Overload Settings"), MVsToolkit.Dev.ReadOnly] protected float m_CurentTemperature;
    [SerializeField, FoldoutGroup("Overload Settings"), MVsToolkit.Dev.ReadOnly] protected OverloadWeaponState m_CurrentState;

    [Space(10)]
    [SerializeField, FoldoutGroup("Overload Settings")] protected float m_ShootTemperature;
    [SerializeField, FoldoutGroup("Overload Settings"), Tooltip("Delay de bloquage avant de pouvoir recharger quand l'arme est surchauffé")] protected float m_StunDelayOnOverload;

    [Space(10)]
    [SerializeField, FoldoutGroup("Overload Settings"), Tooltip("Refroidissement par seconde quand maximum pas atteint")] protected float m_DefaultCoolsPerSec;
    [SerializeField, FoldoutGroup("Overload Settings"), Tooltip("Refroidissement par seconde quand maximum pas atteint")] protected float m_OverloadCoolsPerSec;
    [SerializeField, FoldoutGroup("Overload Settings"), Tooltip("Refroidissement par seconde quand input raté")] protected float m_NerfCoolsPerSec;
    [SerializeField, FoldoutGroup("Overload Settings"), Tooltip("Refroidissement par seconde quand input réussis")] protected float m_BuffCoolsPerSec;

    [Space(5)]
    [SerializeField, FoldoutGroup("Overload Settings")] protected Vector2 RangeToReset;
    [SerializeField, FoldoutGroup("Overload Settings")] protected Vector2 RangeToBuff;
    [SerializeField, FoldoutGroup("Overload Settings")] protected Vector2 RangeToNerf;

    [SerializeField, FoldoutGroup("Overload Settings"), DrawInRect("DrawReloadRect")] int drawReloadRect;

    [Space(10)]
    [SerializeField, FoldoutGroup("Overload Settings")] protected float m_TimeBeforeAutoCool;
    
    [Space(10)]
    [SerializeField] protected float m_AttackCooldown;

    protected float m_AutoCoolTimer;

    //[Header("References")]
    [Header("Input")]
    [SerializeField] protected InputActionReference m_HandleCoolsInput;
    [SerializeField] protected InputActionReference m_HandleCoolsSkillInput;

    [Header("Output")]
    [Space(10)]
    [SerializeField] protected UnityEvent m_OnAttackFeedback;
    [SerializeField] protected UnityEvent m_OnReloadFeedback;

    public Action OnOverloadStart, OnOverloadEnd;
    public Action<OverloadWeaponState> OnOverloadStateChange;

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

    private void Update()
    {
        switch (m_CurrentState)
        {
            case OverloadWeaponState.CanShoot:
                m_AutoCoolTimer += Time.deltaTime;

                if(m_AutoCoolTimer >= m_TimeBeforeAutoCool)
                    HandleCool(m_DefaultCoolsPerSec);
                break;

            case OverloadWeaponState.DefaultCool:
                HandleCool(m_DefaultCoolsPerSec);
                break;
            case OverloadWeaponState.OverloadCool:
                HandleCool(m_OverloadCoolsPerSec);
                break;

            case OverloadWeaponState.CoolBuffed:
                HandleCool(m_BuffCoolsPerSec);
                break;
            case OverloadWeaponState.CoolNerfed:
                HandleCool(m_NerfCoolsPerSec);
                break;
        }
    }

    protected virtual void HandleCool(float coolSpeed)
    {
        m_CurentTemperature -= coolSpeed * Time.deltaTime;

        if (m_CurentTemperature <= 0)
        {
            m_CurentTemperature = 0;
            if (m_CurrentState == OverloadWeaponState.OverloadCool)
                OnOverloadEnd?.Invoke();

            m_CurrentState = OverloadWeaponState.CanShoot;
        }

        OnAmmoChange?.Invoke(m_CurentTemperature, 100);
    }

    protected virtual IEnumerator OnOverload()
    {
        m_CurentTemperature = 100;
        m_CurrentState = OverloadWeaponState.Overload;
        OnOverloadStart?.Invoke();

        yield return new WaitForSeconds(m_StunDelayOnOverload);

        m_CurrentState = OverloadWeaponState.OverloadCool;
    }

    protected virtual void Cool(InputAction.CallbackContext ctx)
    {
        switch (m_CurrentState)
        {
            case OverloadWeaponState.CanShoot:
                m_CurrentState = OverloadWeaponState.DefaultCool;
                OnReload?.Invoke();
                break;
        }
    }

    protected virtual void TryBuffOnCool(InputAction.CallbackContext ctx)
    {
        if (m_CurrentState != OverloadWeaponState.OverloadCool) return;

        if (m_CurentTemperature.InRange(RangeToBuff))
        {
            m_CurrentState = OverloadWeaponState.CoolBuffed;
            m_CurentTemperature = 100;
        }
        else if (m_CurentTemperature.InRange(RangeToReset))
        {
            m_CurrentState = OverloadWeaponState.CanShoot;
            m_CurentTemperature = 0;
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);
        }
        else if (m_CurentTemperature.InRange(RangeToNerf))
        {
            m_CurrentState = OverloadWeaponState.CoolNerfed;
            m_CurentTemperature = 100;
        }
        else return;

        OnOverloadStateChange.Invoke(m_CurrentState);
        OnOverloadEnd?.Invoke();
    }

    protected virtual IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        m_AutoCoolTimer = 0;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
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

    #region Getter
    public float GetCurrentTemperature() => m_CurentTemperature;
    public OverloadWeaponState GetState() => m_CurrentState;
    public Vector2 GetRangeToBuff() => RangeToBuff;
    public Vector2 GetRangeToReset() => RangeToReset;
    #endregion

#if UNITY_EDITOR
    void DrawReloadRect(Rect rect)
    {
        Rect sliderRect = new Rect(rect.x, rect.y + 12.5f, rect.width, 5);
        float unit = sliderRect.width / 100f;

        EditorGUI.DrawRect(sliderRect, Color.grey);

        // Nerf range
        float nerfStart = sliderRect.x + RangeToNerf.x * unit;
        float nerfWidth = Mathf.Max(0f, (RangeToNerf.y - RangeToNerf.x) * unit);
        EditorGUI.DrawRect(new Rect(nerfStart, sliderRect.y, nerfWidth, sliderRect.height), Color.red);

        // Reset range
        float resetStart = sliderRect.x + RangeToReset.x * unit;
        float resetWidth = Mathf.Max(0f, (RangeToReset.y - RangeToReset.x) * unit);
        EditorGUI.DrawRect(new Rect(resetStart, sliderRect.y, resetWidth, sliderRect.height), Color.blue);

        // Buff range
        float buffStart = sliderRect.x + RangeToBuff.x * unit;
        float buffWidth = Mathf.Max(0f, (RangeToBuff.y - RangeToBuff.x) * unit);
        EditorGUI.DrawRect(new Rect(buffStart, sliderRect.y, buffWidth, sliderRect.height), Color.yellow);
    }
#endif
}

public enum OverloadWeaponState
{
    CanShoot = 0,
    Overload = 1,
    DefaultCool = 2,
    OverloadCool = 3,
    CoolBuffed = 4,
    CoolNerfed = 5
}