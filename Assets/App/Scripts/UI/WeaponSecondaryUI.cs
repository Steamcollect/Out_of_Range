using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSecondaryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_PrimaryWeaponUIContainer;
    [SerializeField] private Image m_ManaFillImage;
    [Space]
    [SerializeField] private RSO_PlayerController m_Controller;
    [SerializeField] private RSO_Mana m_Mana;

    private void OnEnable()
    {
        m_Controller.Get().GetPlayerCombat().OnSecondaryCombatStyleChange += UpdateUI;
        m_Mana.OnChanged += UpdateUIIndicator;
    }
    
    private void OnDisable()
    {
        m_Controller.Get().GetPlayerCombat().OnSecondaryCombatStyleChange -= UpdateUI;
        m_Mana.OnChanged -= UpdateUIIndicator;
    }

    private void Start()
    {
        UpdateUI();
        UpdateUIIndicator(m_Mana.Get());
    }

    private void UpdateUI()
    {
        if (!m_Controller.Get().GetPlayerCombat().GetSecondaryCombatStyle())
        {
            m_PrimaryWeaponUIContainer.SetActive(false);
            return;
        }

        m_PrimaryWeaponUIContainer.SetActive(true);
    }

    private void UpdateUIIndicator(Mana mana) => m_ManaFillImage.fillAmount = 1- (float)mana.CurrentMana / mana.MaxMana;
}
