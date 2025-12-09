using System;
using System.Collections;
using UnityEngine;

public abstract class CombatStyle : MonoBehaviour
{
    protected bool m_CanAttack = true;
    protected bool m_IsAttacking = false;
    public Action<float /*current*/, float /*max*/> OnAmmoChange;

    public Action OnAttack;
    public Action OnReload;

    public virtual IEnumerator Attack() { yield break; }

    public virtual void Reload()
    {
    }

    public bool IsAttacking()
    {
        return m_IsAttacking;
    }

    public bool CanAttack()
    {
        return m_CanAttack;
    }
}