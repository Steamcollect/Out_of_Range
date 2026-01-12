using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class CombatStyle : MonoBehaviour
{
    protected bool m_CanAttack = true;
    protected bool m_IsAttacking = false;
    
    public Action<float /*current*/, float /*max*/> OnAmmoChange;

    [SerializeField, EnumButtons] protected InputAttackType m_InputAttackType;
    public enum InputAttackType { Auto, SemiAuto}

    public Action OnAttack;
    public Action OnReload;

    [SerializeField] protected UnityEvent m_OnAttackFeedback;
    [SerializeField] protected UnityEvent m_OnReloadFeedback;

    public virtual void AttackStart(InputAction.CallbackContext ctx)
    {
        StartCoroutine(Attack());
    }

    public virtual IEnumerator Attack() { yield break; }
    public virtual void StopAttack()
    {
        m_IsAttacking = false;
    }
    
    public virtual void Reload() { }

    public bool IsAttacking()
    {
        return m_IsAttacking;
    }

    public bool CanAttack()
    {
        return m_CanAttack;
    }

    public InputAttackType GetInputAttackType() => m_InputAttackType;
}