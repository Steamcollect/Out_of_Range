using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("HEALTH")]
    [SerializeField] protected int m_MaxHealth;

    [SerializeField] protected int m_CurrentHealth;

    [Header("INVINCIBILITY")]
    [SerializeField] protected float m_InvincibilityRegainDuration;

    [SerializeField] protected bool m_IsInvincible;

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

        //Debug.Log("Entity gain invincibility for " + m_InvincibilityRegainDuration + " seconds.");
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

    public void GainInvincibility(float duration)
    {
        StartCoroutine(OnInvincibilityGain(duration));
    }

    private IEnumerator OnInvincibilityGain(float duration)
    {
        // Feedback gain d'invicibilit�
        m_IsInvincible = true;
        m_CurrentInvincibilityTimer = 0;

        while (m_CurrentInvincibilityTimer < duration)
        {
            m_CurrentInvincibilityTimer += Time.deltaTime;

            yield return null;
        }

        LoseInvincibility();
    }

    private void LoseInvincibility()
    {
        m_IsInvincible = false;
        // Feedback perte d'invincibilit�
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