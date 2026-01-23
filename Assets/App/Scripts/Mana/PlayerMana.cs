using System;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int m_MaxMana = 100;

    private int m_CurrentMana;
        
    public event Action OnManaChanged;
        
    public int CurrentMana => m_CurrentMana;
    public int MaxMana => m_MaxMana;
        
    public void Add(int amount)
    {
        m_CurrentMana = Mathf.Max(m_MaxMana, m_CurrentMana + amount);
        OnManaChanged?.Invoke();
    }
        
    public void Remove(int amount)
    {
        m_CurrentMana = Mathf.Max(0, m_CurrentMana - amount);
        OnManaChanged?.Invoke();
    }

}