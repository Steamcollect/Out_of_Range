using DG.Tweening;
using MVsToolkit.Dev;
using System.Collections;
using UnityEngine;
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
    [SerializeField] private float m_WarningVisualIntensity = 0.5f;

    [Space(10)]
    [SerializeField, EnumButtons] ElectricAreaState m_StartingState;
    [SerializeField, ReadOnly] ElectricAreaState m_CurrentState;

    public enum ElectricAreaState { Safe, Warning, Damage }

    [Header("References")]
    [SerializeField] private VisualEffect m_ElectricAreaVFX;

    IHealth m_PlayerHealth;

    Coroutine m_CurrentLoop;

    private static readonly int IntensityID = Shader.PropertyToID("Intensity");

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

        m_ElectricAreaVFX.SetFloat(IntensityID, 0);
    }

    public void OnSetAsWarning()
    {
        DOTween.To(() => 0f,
               x => m_ElectricAreaVFX.SetFloat(IntensityID, x),
               m_WarningVisualIntensity,
               m_WarningTime)
               .SetEase(Ease.OutQuad);
    }

    public void OnSetAsDamage()
    {
        ApplyDamage();

        m_ElectricAreaVFX.SetFloat(IntensityID, 1);
    }

    public void SetState(ElectricAreaState state) => m_CurrentState = state;

    void ApplyDamage()
    {
        if (m_PlayerHealth == null) return;

        if (m_IsLethalDamage) m_PlayerHealth.Die();
        else m_PlayerHealth.TakeDamage(m_Damage);
    }

    public void StopLoop()
    {
        OnSetAsSafe();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent(out PlayerController player))
            {
                m_PlayerHealth = player.GetHealth();
                player.GetDash().OnDashInvincibilityEnd += ApplyDamage;

                if (m_CurrentState == ElectricAreaState.Damage)
                {
                    if (m_IsLethalDamage) m_PlayerHealth.Die();
                    else m_PlayerHealth.TakeDamage(m_Damage);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                player.GetDash().OnDashInvincibilityEnd -= ApplyDamage;
                m_PlayerHealth = null;
            }
        }
    }
}