using System;
using MVsToolkit.Dev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPowerUp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_HandleTime = 10;
    [SerializeField] int m_MaxInventorySize = 2;

    [Space(10)]
    [SerializeField, ReadOnly] bool m_IsHandlingPowerUps = false;
    [SerializeField, ReadOnly] List<SSO_PowerUp> m_PowerUps = new();

    [Header("References")]
    [SerializeField] RSO_CurrentPowerUp m_Script;
    
    Coroutine m_HandleCouroutine;

    [Header("Input")]
    [SerializeField] InputActionReference m_HandleIA;
    
    //[Header("Output")]
    public Action<List<SSO_PowerUp>> OnListChanged;

    private void Awake()
    {
        m_Script.Set(this);
    }

    private void OnEnable()
    {
        m_HandleIA.action.started += Handle;
    }

    private void OnDisable()
    {
        m_HandleIA.action.started -= Handle;
    }

    private void Start()
    {
        m_HandleIA.action.Enable();
    }

    void Handle(InputAction.CallbackContext ctx)
    {
        if (m_PowerUps.Count == 0 || m_IsHandlingPowerUps) return;

        if(m_HandleCouroutine !=  null) StopCoroutine(m_HandleCouroutine);
        m_HandleCouroutine = StartCoroutine(HandleTime());
    }

    IEnumerator HandleTime()
    {
        m_IsHandlingPowerUps = true;
        yield return new WaitForSeconds(m_HandleTime);
     
        Clear();
        m_IsHandlingPowerUps = false;
    }

    public void Add(SSO_PowerUp powerUp)
    {
        if (m_PowerUps.Count >= m_MaxInventorySize || m_PowerUps.Contains(powerUp)) return;

        m_PowerUps.Add(powerUp);
        OnListChanged?.Invoke(m_PowerUps);
    }

    public void Clear()
    {
        m_PowerUps.Clear();
        OnListChanged?.Invoke(m_PowerUps);
    }

    public List<SSO_PowerUp> GetPowerUps() {  return m_PowerUps; }

    public bool ContainPowerUp(PowerUpType type)
    {
        foreach (SSO_PowerUp powerUp in m_PowerUps)
        {
            if (powerUp.PowerUpType == type) return true;
        }

        return false;
    }

    public bool IsHandlingPowerUps() => m_IsHandlingPowerUps;

    public void OnPlayerDeath()
    {
        if(m_HandleCouroutine != null) StopCoroutine(m_HandleCouroutine);
        Clear();
    }
}