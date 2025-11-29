using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour, IHealth
{
    [Header("Settings")]
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

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

    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;
}