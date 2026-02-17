using UnityEngine;
using System.Collections.Generic;

public class PowerUpVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject m_AttackSpeedPowerUp;
    [SerializeField] private GameObject m_StrengthPowerUp;
    [SerializeField] private GameObject m_ClonePowerUp;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerPowerUpChange m_OnPowerUpChange;

    private void OnEnable() => m_OnPowerUpChange.Action += UpdatePlayerVisual;
    private void OnDisable() => m_OnPowerUpChange.Action -= UpdatePlayerVisual;

    private void Awake()
    {
        m_AttackSpeedPowerUp.SetActive(false);
        m_StrengthPowerUp.SetActive(false);
        m_ClonePowerUp.SetActive(false);
    }

    private void UpdatePlayerVisual(List<PowerUpHandler> activePowerUps)
    {
        m_AttackSpeedPowerUp.SetActive(false);
        m_StrengthPowerUp.SetActive(false);
        m_ClonePowerUp.SetActive(false);

        foreach (PowerUpHandler powerUp in activePowerUps)
        {

            switch (powerUp.PowerUp.PowerUpType)
            {
                case PowerUpType.AttackSpeed:
                    m_AttackSpeedPowerUp.SetActive(true);
                    break;
                case PowerUpType.Strength:
                    m_StrengthPowerUp.SetActive(true);
                    break;
                case PowerUpType.Clone:
                    m_ClonePowerUp.SetActive(true);
                    break;
            }
        }
    }
}