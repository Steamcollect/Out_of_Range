using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponPrimary : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_PrimaryWeaponUIContainer;
    [SerializeField] private Image m_WeaponAmmoFillImage;
    [Space]
    [SerializeField] private RSO_PlayerController m_Controller;

    private void OnEnable()
    {
        m_Controller.Get().GetPlayerCombat().OnPrimaryCombatStyleChange += UpdateUI;
    }
    
    private void OnDisable()
    {
        m_Controller.Get().GetPlayerCombat().OnPrimaryCombatStyleChange -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (!m_Controller.Get().GetPlayerCombat().GetPrimaryCombatStyle())
        {
            m_PrimaryWeaponUIContainer.SetActive(false);
            return;
        }
        
        m_PrimaryWeaponUIContainer.SetActive(true);
        m_Controller.Get().GetPlayerCombat().GetPrimaryCombatStyle().OnAmmoChange += UpdateUIIndicator;
    }

    private void UpdateUIIndicator(float c, float m) => m_WeaponAmmoFillImage.fillAmount = 1- c/ m;
}
