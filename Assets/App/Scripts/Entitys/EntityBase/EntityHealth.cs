using MVsToolkit.Utils;
using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("Settings")]
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    [SerializeField] float invincibilityDelay;
    bool isInvincible = false;

    [Header("References")]
    [SerializeField] protected DamageSFXManager m_DamageSFXManager;
    
    public Action OnTakeDamage, OnDeath;

    //[Header("Input")]
    //[Header("Output")]

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        
        if(currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if(invincibilityDelay > 0)
            {
                isInvincible = true;
                CoroutineUtils.Delay(this, () =>
                {
                    isInvincible = false;
                }, invincibilityDelay);
            }

            m_DamageSFXManager.PlayDamageSFX();
            OnTakeDamage?.Invoke();
        }
    }

    void Die()
    {
        m_DamageSFXManager.PlayDeathSFX();
        OnDeath?.Invoke();
    }

    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;
    
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}