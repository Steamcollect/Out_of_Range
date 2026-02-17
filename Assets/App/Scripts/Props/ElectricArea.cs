using MVsToolkit.Dev;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.VFX;

public class ElectricArea : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool m_IsLethalDamage = false;
    [SerializeField, ShowIf("m_IsLethalDamage", false)] int m_Damage = 1;

    [SerializeField] bool m_HandleOnStart = false;

    [Space(10)]
    [SerializeField] float m_SafeTime = 1;
    [SerializeField] float m_WarningTime = 1;
    [SerializeField] float m_DamageTime = 1;

    [Space(10)]
    [SerializeField, EnumButtons] ElectricAreaState m_StartingState;
    [SerializeField, ReadOnly] ElectricAreaState m_CurrentState;

    public enum ElectricAreaState { Safe, Warning, Damage }

    [Header("References")]
    [SerializeField] private VisualEffect m_ElectricAreaVFX;

    List<IHealth> m_HealthsInside = new();

    Coroutine m_CurrentLoop;

    private void Start()
    {
        m_CurrentState = m_StartingState;

        switch (m_CurrentState)
        {
            case ElectricAreaState.Safe:
                OnSetAsSafe();
                break;

            case ElectricAreaState.Warning:
                OnSetAsWarning();
                break;

            case ElectricAreaState.Damage:
                OnSetAsDamage();
                break;
        }

        if(m_HandleOnStart)
            m_CurrentLoop = StartCoroutine(Loop());
    }

    public void HandleLoop()
    {
        SetState(ElectricAreaState.Warning);
        OnSetAsWarning();
        m_CurrentLoop = StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        switch (m_CurrentState)
        {
            case ElectricAreaState.Safe:

                yield return new WaitForSeconds(m_SafeTime);

                m_CurrentState = ElectricAreaState.Warning;
                OnSetAsWarning();
                break;

            case ElectricAreaState.Warning:

                yield return new WaitForSeconds(m_WarningTime);
                
                m_CurrentState = ElectricAreaState.Damage;
                OnSetAsDamage();
                break;

            case ElectricAreaState.Damage:

                if (m_DamageTime == -1) yield break;
                else yield return new WaitForSeconds(m_DamageTime);

                m_CurrentState = ElectricAreaState.Safe;
                OnSetAsSafe();
                break;
        }

        m_CurrentLoop = StartCoroutine(Loop());
    }

    public void OnSetAsSafe()
    {
        if(m_CurrentLoop != null) StopCoroutine(m_CurrentLoop);

        m_ElectricAreaVFX.SetFloat("Intensity", 0);
    }

    public void OnSetAsWarning()
    {
        m_ElectricAreaVFX.SetFloat("Intensity", 0.5f);
    }

    public void OnSetAsDamage()
    {
        ApplyDamage();

        m_ElectricAreaVFX.SetFloat("Intensity", 1);
    }

    public void SetState(ElectricAreaState state) => m_CurrentState = state;

    void ApplyDamage()
    {
        foreach (var health in m_HealthsInside)
        {
            if (m_IsLethalDamage) health.Die();
            else health.TakeDamage(m_Damage);
        }
    }

    public void StopLoop()
    {
        OnSetAsSafe();
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