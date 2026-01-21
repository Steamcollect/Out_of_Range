using System;
using UnityEngine;

public class PowerUpUI : MonoBehaviour
{
        [Header("References")]
        [SerializeField] private RSO_CurrentPowerUp m_CurrentPowerUp;
        [SerializeField] private GameObject m_PowerUpContainer;

        private void OnEnable() => m_CurrentPowerUp.OnChanged += UpdateUI;
        private void OnDisable() => m_CurrentPowerUp.OnChanged -= UpdateUI;
        private void Start() => UpdateUI(m_CurrentPowerUp.Get());

        private void UpdateUI(PowerUp obj)
        { 
                m_PowerUpContainer.SetActive(obj != null);
        }
}