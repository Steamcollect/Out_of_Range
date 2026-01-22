using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpUI : MonoBehaviour
{
        [Header("References")]
        [SerializeField] private RSO_CurrentPowerUp m_CurrentPowerUp;
        [SerializeField] private GameObject m_PowerUpContainer;

        private void OnEnable() => m_CurrentPowerUp.OnChanged += UpdateUI;
        private void OnDisable() => m_CurrentPowerUp.OnChanged -= UpdateUI;
        private void Start() => UpdateUI(m_CurrentPowerUp.Get());

        private void UpdateUI(HashSet<PowerUp> powerUps)
        {
                m_PowerUpContainer.SetActive(powerUps.Count > 0);
        }
}