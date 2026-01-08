using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCombat : EntityCombat
{
    [ShowInInspector] private CombatStyle m_PrimaryCombatStyle, m_SecondaryCombatStyle;

    [SerializeField] private InputPlayerController m_InputPlayerController;
    [Space(10)]
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;

    public Action OnPrimaryCombatStyleChange, OnSecondaryCombatStyleChange;

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
    
    public void SetPrimaryCombatStyle(CombatStyle newStyle)
    {
        if(m_PrimaryCombatStyle != null)
            m_InputPlayerController.PrimaryAttackIa.action.started -= m_PrimaryCombatStyle.AttackStart;
        
        m_PrimaryCombatStyle = newStyle;
        OnPrimaryCombatStyleChange?.Invoke();
        m_InputPlayerController.PrimaryAttackIa.action.started += m_PrimaryCombatStyle.AttackStart;
    }

    public void SetSecondaryCombatStyle(CombatStyle newStyle)
    {
        if (newStyle == null) return;

        if (m_SecondaryCombatStyle != null)
            m_InputPlayerController.SecondaryAttackIa.action.started -= m_SecondaryCombatStyle.AttackStart;

        m_SecondaryCombatStyle = newStyle;
        OnSecondaryCombatStyleChange?.Invoke();
        m_InputPlayerController.SecondaryAttackIa.action.started += m_SecondaryCombatStyle.AttackStart;
    }

    public CombatStyle GetPrimaryCombatStyle()
    {
        return m_PrimaryCombatStyle;
    }
    public CombatStyle GetSecondaryCombatStyle()
    {
        return m_SecondaryCombatStyle;
    }
}