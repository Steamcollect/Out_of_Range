using System;
using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("HEALTH")]
    [SerializeField] protected int m_MaxHealth;
    [SerializeField] protected int m_CurrentHealth;

    [SerializeField] bool m_ClampDamage;
    [SerializeField, ShowIf("m_ClampDamage", true)] int m_MaxDamage;

    [Header("INVINCIBILITY")]
    [SerializeField] protected float m_InvincibilityRegainDuration;

    [SerializeField, ReadOnly] protected bool m_IsInvincible;

    [Header("REFERENCES")]
    [SerializeField] protected UnityEvent m_OnTakeDamageFeedback;
    [SerializeField] protected UnityEvent m_OnDeathFeedback;

    protected float m_CurrentInvincibilityTimer;

    public Action OnTakeDamage, OnDeath;

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (m_IsInvincible) return;

        if (m_ClampDamage) damage = Mathf.Clamp(damage, 0, m_MaxDamage);

        GainInvincibility(m_InvincibilityRegainDuration);

        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            m_OnTakeDamageFeedback.Invoke();
            OnTakeDamage?.Invoke();
        }
    }

    public void TakeHealth(int health)
    {
        m_CurrentHealth += health;

        if (m_CurrentHealth > m_MaxHealth)
            m_CurrentHealth = m_MaxHealth;

        OnTakeDamage?.Invoke();
    }

    private void Die()
    {
        m_OnDeathFeedback.Invoke();
        OnDeath?.Invoke();
    }

    public void GainInvincibility()
    {
        m_IsInvincible = true;
    }
    
    public void GainInvincibility(float duration)
    {
        StartCoroutine(OnInvincibilityGain(duration));
    }

    private IEnumerator OnInvincibilityGain(float duration)
    {
        m_IsInvincible = true;
        yield return new WaitForSeconds(duration);
        m_IsInvincible = false;
    }

    public int GetMaxHealth()
    {
        return m_MaxHealth;
    }

    public int GetCurrentHealth()
    {
        return m_CurrentHealth;
    }

    public float GetHealthPercentage()
    {
        return (float)m_CurrentHealth / m_MaxHealth;
    }
}