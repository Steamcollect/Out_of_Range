using UnityEngine;
using UnityEngine.UI;

public class WeaponSecondaryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_PrimaryWeaponUIContainer;
    [SerializeField] private Image m_ManaFillImage;
    [SerializeField] private Image m_MaxManaIndicatorImage;
    [Space]
    [SerializeField] private Animator m_Animator;
    [Space]
    [SerializeField] private RSO_PlayerController m_Controller;

    private void OnEnable()
    {
        m_Controller.Get().GetPlayerCombat().OnSecondaryCombatStyleChange += UpdateUI;
        m_Controller.Get().GetPlayerMana().OnManaChanged += UpdateUIIndicator;
        UpdateUIIndicator();
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
        if (m_Controller.Get().GetPlayerCombat().GetSecondaryCombatStyle() is GrenadeLauncherCombatStyle
            grenadeLauncherCombatStyle)
        {
            m_ManaFillImage.fillAmount = Mathf.Clamp01((float)m_Controller.Get().GetPlayerMana().CurrentMana / grenadeLauncherCombatStyle.Cost);
        }
        else
        {
            m_ManaFillImage.fillAmount = (float)m_Controller.Get().GetPlayerMana().CurrentMana / m_Controller.Get().GetPlayerMana().MaxMana;
        }
        m_MaxManaIndicatorImage.CrossFadeColor(new Color(1,1,1, m_ManaFillImage.fillAmount >= 1 ? 1 : 0), 0.2f, true, true);
        
    }
}
