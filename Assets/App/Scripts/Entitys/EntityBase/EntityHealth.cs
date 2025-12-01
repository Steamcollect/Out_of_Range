using MVsToolkit.Utils;
using System;
using System.Collections;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("HEALTH")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;

    [Header("INVINCIBILITY")]
    [SerializeField] protected float m_InvincibilityRegainDuration = 5.0f;
    [SerializeField] protected bool m_IsInvincible = false;

    [Header("REFERENCES")]
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
            m_DamageSFXManager.PlayDamageSFX();
            OnTakeDamage?.Invoke();
        }
    }

    void Die()
    {
        m_DamageSFXManager.PlayDeathSFX();
        OnDeath?.Invoke();
    }

    public void GainInvincibility(float duration)
    {
        StartCoroutine(OnInvincibilityGain(duration));
    }

    private IEnumerator OnInvincibilityGain(float duration)
    {
        // Feedback gain d'invicibilité
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
        // Feedback perte d'invincibilité
    }

    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;
    
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}