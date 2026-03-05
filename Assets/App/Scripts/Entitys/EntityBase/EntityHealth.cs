using System;
using System.Collections;
using MVsToolkit.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("HEALTH")]
    [SerializeField] protected float m_MaxHealth;
    [SerializeField] protected float m_CurrentHealth;

    [SerializeField] bool m_ClampDamage;
    [SerializeField, ShowIf("m_ClampDamage", true)] int m_MaxDamage;

    [Header("INVINCIBILITY")]
    [SerializeField] protected float m_InvincibilityRegainDuration;

    [SerializeField, ReadOnly] protected bool m_IsInvincible;
    protected float m_CurrentInvincibilityDuration = 0;

    Coroutine m_InvincibilityCoroutine;

    public float dieDuration = 0.0f;

    [Header("REFERENCES")]
    [SerializeField] protected UnityEvent m_OnTakeDamageFeedback;
    [SerializeField] protected UnityEvent m_OnDeathFeedback;
    [SerializeField] protected UnityEvent m_OnHealFeedback;
    [SerializeField] private GameObject m_ExplosionFX;
    protected float m_CurrentInvincibilityTimer;

    public Action OnTakeDamage, OnHeal, OnDeath;

    private bool IsDying = false;

    private void FakeTakeDamage()
    {
        OnTakeDamage?.Invoke(); 
        m_OnTakeDamageFeedback?.Invoke();
    }

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    [Button("Take Damage")]
    public void TakeDamage(float damage = 1)
    {
        if (m_IsInvincible) return;

        if (m_ClampDamage) damage = Mathf.Clamp(damage, 0, m_MaxDamage);

        if(m_InvincibilityRegainDuration > 0) GainInvincibility(m_InvincibilityRegainDuration);

        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            OnTakeDamage?.Invoke();
            m_OnTakeDamageFeedback.Invoke();
        }
    }

    [Button]
    public void Heal(float health)
    {
        if (m_CurrentHealth == m_MaxHealth) return;

        m_CurrentHealth += health;

        if (m_CurrentHealth > m_MaxHealth)
            m_CurrentHealth = m_MaxHealth;

        OnHeal?.Invoke();
        m_OnHealFeedback?.Invoke();
    }

    public void Die()
    {
        if (IsDying) return;

        IsDying = true;

        m_OnDeathFeedback.Invoke();

        this.Delay(() => {
            if (m_ExplosionFX != null) Instantiate(m_ExplosionFX, transform.position, Quaternion.identity);
            OnDeath?.Invoke();
        }, dieDuration);
    }

    public void GainInvincibility()
    {
        m_IsInvincible = true;
    }
    
    public void GainInvincibility(float duration)
    {
        if (m_CurrentInvincibilityDuration > duration) return;

        if(m_InvincibilityCoroutine != null) StopCoroutine(m_InvincibilityCoroutine);
        m_InvincibilityCoroutine = StartCoroutine(OnInvincibilityGain(duration));
    }

    private IEnumerator OnInvincibilityGain(float duration)
    {
        m_IsInvincible = true;
        m_CurrentInvincibilityDuration = duration;

        while(m_CurrentInvincibilityDuration > 0)
        {
            m_CurrentInvincibilityDuration -= Time.deltaTime;
            yield return null;
        }

        m_CurrentInvincibilityDuration = 0;
        m_IsInvincible = false;
    }

    public float GetMaxHealth()
    {
        return m_MaxHealth;
    }

    public float GetCurrentHealth()
    {
        return m_CurrentHealth;
    }

    public float GetHealthPercentage()
    {
        return (float)m_CurrentHealth / m_MaxHealth;
    }
}