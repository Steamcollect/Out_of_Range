using System;
using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMana : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int m_MaxMana = 100;
    [SerializeField, ReadOnly] int m_CurrentMana;

    [SerializeField] int m_ManaGivenPerSec = 1;

    public event Action OnManaChanged;
    public event Action OnManaAddedOverTime;
    public UnityEvent OnManaCollected;
    
    public int CurrentMana => m_CurrentMana;
    public int MaxMana => m_MaxMana;

    private void Start()
    {
        SetToMax();
        StartCoroutine(ManaOverTime());
    }

    IEnumerator ManaOverTime()
    {
        while (true)
        {
            yield return new WaitUntil(() => m_CurrentMana < m_MaxMana);

            yield return new WaitForSeconds(1);
            m_CurrentMana = Math.Clamp(m_CurrentMana + m_ManaGivenPerSec, 0, m_MaxMana);
            OnManaAddedOverTime.Invoke();
        }
    }

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
        OnManaChanged?.Invoke();
    }
}