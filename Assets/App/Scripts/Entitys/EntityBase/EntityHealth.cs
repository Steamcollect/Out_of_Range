using MVsToolkit.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("HEALTH")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;

    [Header("INVINCIBILITY")]
    [SerializeField] protected float m_InvincibilityRegainDuration;
    [SerializeField] protected bool m_IsInvincible = false;

    [Header("REFERENCES")]
    [SerializeField] protected UnityEvent m_OnDeathFeedback;
    [SerializeField] protected DamageSFXManager m_DamageSFXManager;
    
    public Action OnTakeDamage, OnDeath;

    protected float m_CurrentInvincibilityTimer;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (m_IsInvincible) return;

        Debug.Log("Entity gain invincibility for " + m_InvincibilityRegainDuration + " seconds.");
        GainInvincibility(m_InvincibilityRegainDuration);

        currentHealth -= damage;
        
        if(currentHealth <= 0)
        {
            Die();
        }
        else
        {
            m_DamageSFXManager?.PlayDamageSFX();
            OnTakeDamage?.Invoke();
        }
    }

    public void TakeHealth(int health)
    {
        currentHealth += health;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        OnTakeDamage?.Invoke();
    }

    void Die()
    {
        m_DamageSFXManager?.PlayDeathSFX();
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

        while(m_CurrentInvincibilityTimer < duration)
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

    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;
    
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}