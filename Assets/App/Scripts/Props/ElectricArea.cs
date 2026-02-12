using MVsToolkit.Dev;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class ElectricArea : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool m_IsLethalDamage = false;
    [SerializeField, ShowIf("m_IsLethalDamage", false)] int m_Damage = 1;

    [Space(10)]
    [SerializeField] float m_SafeTime = 1;
    [SerializeField] float m_WarningTime = 1;
    [SerializeField] float m_DamageTime = 1;

    float m_Timer = 0;

    [Space(10)]
    [SerializeField, EnumButtons] ElectricAreaState m_StartingState;
    [SerializeField, ReadOnly] ElectricAreaState m_CurrentState;

    enum ElectricAreaState { Safe, Warning, Damage }

    [Header("References")]
    [SerializeField] GameObject m_WarningGO;
    [SerializeField] GameObject m_DamageGO;

    List<IHealth> m_HealthsInside = new();

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_CurrentState = m_StartingState;

        switch (m_CurrentState)
        {
            case ElectricAreaState.Safe:
                SetAsWarning();
                break;

            case ElectricAreaState.Warning:
                SetAsDamage();
                break;

            case ElectricAreaState.Damage:
                SetAsSafe();
                break;
        }
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;

        switch (m_CurrentState)
        {
            case ElectricAreaState.Safe:
                if(m_Timer >= m_SafeTime)
                {
                    m_Timer = 0;
                    m_CurrentState = ElectricAreaState.Warning;
                    SetAsWarning();
                }
                break;

            case ElectricAreaState.Warning:
                if(m_Timer >= m_WarningTime)
                {
                    m_Timer = 0;
                    m_CurrentState = ElectricAreaState.Damage;
                    SetAsDamage();
                }
                break;

            case ElectricAreaState.Damage:
                if(m_Timer >= m_DamageTime)
                {
                    m_Timer = 0;
                    m_CurrentState = ElectricAreaState.Safe;
                    SetAsSafe();
                }
                break;
        }
    }

    void SetAsSafe()
    {
        m_WarningGO.SetActive(false);
        m_DamageGO.SetActive(false);
    }

    void SetAsWarning()
    {
        m_WarningGO.SetActive(true);
        m_DamageGO.SetActive(false);
    }

    void SetAsDamage()
    {
        ApplyDamage();

        m_WarningGO.SetActive(false);
        m_DamageGO.SetActive(true);
    }

    void ApplyDamage()
    {
        foreach (var health in m_HealthsInside)
        {
            if (m_IsLethalDamage) health.Die();
            else health.TakeDamage(m_Damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IHealth health))
        {
            m_HealthsInside.Add(health);

            if(m_CurrentState == ElectricAreaState.Damage)
            {
                if (m_IsLethalDamage) health.Die();
                else health.TakeDamage(m_Damage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out IHealth health))
        {
            m_HealthsInside.Remove(health);
        }
    }
}