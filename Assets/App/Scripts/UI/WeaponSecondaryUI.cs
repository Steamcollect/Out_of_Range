using UnityEngine;
using UnityEngine.UI;

public class WeaponSecondaryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_PrimaryWeaponUIContainer;
    [SerializeField] private Image m_ManaFillImage;
    [Space]
    [SerializeField] private Animator m_Animator;
    [Space]
    [SerializeField] private RSO_PlayerController m_Controller;

    private void OnEnable()
    {
        m_Controller.Get().GetPlayerCombat().OnSecondaryCombatStyleChange += UpdateUI;
        m_Controller.Get().GetPlayerMana().OnManaChanged += UpdateUIIndicator;
    }
    
    private void OnDisable()
    {
        m_Controller.Get().GetPlayerCombat().OnSecondaryCombatStyleChange -= UpdateUI;
        m_Controller.Get().GetPlayerMana().OnManaChanged -= UpdateUIIndicator;
    }

    private void Start()
    {
        UpdateUI();
        UpdateUIIndicator();
    }

    private void UpdateUI()
    {
        if (!m_Controller.Get().GetPlayerCombat().GetSecondaryCombatStyle())
        {
            m_Animator.Play("Disappear");
            m_PrimaryWeaponUIContainer.SetActive(false);
            return;
        }
        m_Animator.Play("Appear");
        m_PrimaryWeaponUIContainer.SetActive(true);
    }

    private void UpdateUIIndicator()
    {
        m_ManaFillImage.fillAmount =
            (float)m_Controller.Get().GetPlayerMana().CurrentMana / m_Controller.Get().GetPlayerMana().MaxMana;
    }
}
