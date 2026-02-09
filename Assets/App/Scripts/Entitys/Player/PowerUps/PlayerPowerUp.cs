using System;
using MVsToolkit.Dev;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerPowerUp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, ReadOnly] List<PowerUpHandler> m_PowerUps = new();

    //[Header("References")]
    [Header("Input")]
    [SerializeField] RSE_AddPowerUp m_AddPowerUp;
    
    [Header("Output")]
    [SerializeField] RSE_OnPlayerPowerUpChange m_OnPowerUpChange;

    private void OnEnable()
    {
        m_AddPowerUp.Action += Add;
    }

    private void OnDisable()
    {
        m_AddPowerUp.Action -= Add;
    }

    private void Update()
    {
        for (int i = 0; i < m_PowerUps.Count; i++)
        {
            m_PowerUps[i].timer -= Time.deltaTime;

            if (m_PowerUps[i].timer <= 0)
            {
                m_PowerUps.RemoveAt(i);
                m_OnPowerUpChange.Call(m_PowerUps);
                return;
            }
        }
    }

    public void Add(SSO_PowerUp powerUp, float time)
    {
        PowerUpHandler handler = m_PowerUps.FirstOrDefault(c => c.PowerUp == powerUp);
        if (handler != null)
        {
            handler.SetTime(time);
            return;
        }

        m_PowerUps.Add(new PowerUpHandler(powerUp, time));
        m_OnPowerUpChange.Call(m_PowerUps);
    }

    public void Clear()
    {
        m_PowerUps.Clear();
        m_OnPowerUpChange.Call(m_PowerUps);
    }

    public List<PowerUpHandler> GetPowerUps() {  return m_PowerUps; }

    public bool ContainPowerUp(PowerUpType type)
    {
        foreach (PowerUpHandler powerUp in m_PowerUps)
        {
            if (powerUp.PowerUp.PowerUpType == type) return true;
        }

        return false;
    }

    public void OnPlayerDeath()
    {
        Clear();
    }
}

[Serializable]
public class PowerUpHandler
{
    public SSO_PowerUp PowerUp;
    public float MaxTime;
    public float timer;

    public PowerUpHandler(SSO_PowerUp powerUp, float time)
    {
        PowerUp = powerUp;
        SetTime(time);
    }

    public void SetTime(float time)
    {
        MaxTime = time;
        timer = time;
    }
}