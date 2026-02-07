using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : EntityCombat
{
    [SerializeField, ReadOnly] CombatStyle m_PrimaryCombatStyle, m_SecondaryCombatStyle;

    [SerializeField] private InputPlayerController m_InputPlayerController;
    [Space(10)]
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;

    [Space(10)]
    [SerializeField] PlayerAnimationVisual m_RotationVisual;

    public Action OnPrimaryCombatStyleChange, OnSecondaryCombatStyleChange;

    private void OnEnable()
    {
        m_InputPlayerController.PrimaryAttackIa.action.started += OnPrimaryAttackStart;
        m_InputPlayerController.PrimaryAttackIa.action.canceled += OnPrimaryAttackCanceled;

        m_InputPlayerController.SecondaryAttackIa.action.started += OnSecondaryAttackStart;
        m_InputPlayerController.SecondaryAttackIa.action.canceled += OnSecondaryAttackCanceled;
    }

    void OnDisable()
    {
        m_InputPlayerController.PrimaryAttackIa.action.started -= OnPrimaryAttackStart;
        m_InputPlayerController.PrimaryAttackIa.action.canceled -= OnPrimaryAttackCanceled;

        m_InputPlayerController.SecondaryAttackIa.action.started -= OnSecondaryAttackStart;
        m_InputPlayerController.SecondaryAttackIa.action.canceled -= OnSecondaryAttackCanceled;
    }

    private void Update()
    {
        Vector3 targetPosition = m_AimTarget.Get().position;
        Vector3 currentPos = transform.position;

        Vector3 direction = targetPosition - currentPos;

        direction.y = 0;

        if(direction.sqrMagnitude > 0.01f)
        {
            LookAt(m_AimTarget.Get().position, LookAtAxis.Horizontal);
        }
        
        if (m_InputPlayerController.IsPrimaryAttackPressed() 
            && m_PrimaryCombatStyle != null 
            && m_PrimaryCombatStyle.GetInputAttackType() == CombatStyle.InputAttackType.Auto)
        {
            StartCoroutine(m_PrimaryCombatStyle.Attack());
        }
        
        if (m_InputPlayerController.IsSecondaryAttackPressed()
            && m_SecondaryCombatStyle != null
            && m_SecondaryCombatStyle.GetInputAttackType() == CombatStyle.InputAttackType.Auto)
        {
            StartCoroutine(m_SecondaryCombatStyle.Attack());
        }
    }

    void OnPrimaryAttackStart(InputAction.CallbackContext ctx) => m_PrimaryCombatStyle?.AttackStart();
    void OnPrimaryAttackCanceled(InputAction.CallbackContext ctx) => m_PrimaryCombatStyle?.AttackEnd();
    void OnSecondaryAttackStart(InputAction.CallbackContext ctx) => m_SecondaryCombatStyle?.AttackStart();
    void OnSecondaryAttackCanceled(InputAction.CallbackContext ctx) => m_SecondaryCombatStyle?.AttackEnd();

    public void SetPrimaryCombatStyle(CombatStyle newStyle)
    {
        if (newStyle == null) return;

        m_PrimaryCombatStyle = newStyle;
        OnPrimaryCombatStyleChange?.Invoke();
    }

    public void SetSecondaryCombatStyle(CombatStyle newStyle)
    {
        if (newStyle == null) return;

        m_SecondaryCombatStyle = newStyle;
        OnSecondaryCombatStyleChange?.Invoke();
    }

    public CombatStyle GetPrimaryCombatStyle()
    {
        return m_PrimaryCombatStyle;
    }
    public CombatStyle GetSecondaryCombatStyle()
    {
        return m_SecondaryCombatStyle;
    }

    public override void LookAt(Vector3 targetPos, LookAtAxis lookAtAxis = LookAtAxis.Both)
    {
        if (!m_CanLookAt) return;

        m_RotationVisual.RotateToward(targetPos);
    }
}