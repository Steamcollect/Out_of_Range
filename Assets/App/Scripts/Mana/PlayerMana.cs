using System;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMana : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int m_MaxMana = 100;
    [SerializeField, ReadOnly] int m_CurrentMana;
        
    public event Action OnManaChanged;
    public UnityEvent OnManaCollected;
        
    public int CurrentMana => m_CurrentMana;
    public int MaxMana => m_MaxMana;
        
    public void Add(int amount)
    {
        m_CurrentMana = Mathf.Clamp(m_CurrentMana + amount, 0, m_MaxMana);
        OnManaCollected?.Invoke();
        OnManaChanged?.Invoke();
    }
        
    public void Remove(int amount)
    {
        m_CurrentMana = Mathf.Clamp(m_CurrentMana - amount, 0, m_MaxMana);
        OnManaChanged?.Invoke();
    }

    public void SetToMax()
    {
        m_CurrentMana = m_MaxMana;
        OnManaChanged.Invoke();
    }
}