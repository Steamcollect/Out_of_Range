using DG.Tweening;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject m_Content;

    [Space]
    [SerializeField] Image m_ManaBackgroundImage;
    [SerializeField] Image m_ManaFillImage;

    [Space]
    [SerializeField] RSO_PlayerController m_Player;

    private void OnEnable()
    {
        m_Player.Get().GetPlayerCombat().OnSecondaryCombatStyleChange += UpdateUI;
        m_Player.Get().GetPlayerMana().OnManaChanged += UpdateUIIndicator;
        UpdateUIIndicator();
    }
    
    private void OnDisable()
    {
        m_Player.Get().GetPlayerCombat().OnSecondaryCombatStyleChange -= UpdateUI;
        m_Player.Get().GetPlayerMana().OnManaChanged -= UpdateUIIndicator;
    }

    private void Start()
    {
        UpdateUI();
        UpdateUIIndicator();
    }

    private void UpdateUI()
    {
        if (!m_Player.Get().GetPlayerCombat().GetSecondaryCombatStyle())
            m_Content.SetActive(false);
        else
            m_Content.SetActive(true);
    }

    private void UpdateUIIndicator()
    {
        m_ManaFillImage.fillAmount = (float)m_Player.Get().GetPlayerMana().CurrentMana / m_Player.Get().GetPlayerMana().MaxMana * m_ManaBackgroundImage.fillAmount;

        m_Content.transform.DOKill();
        m_Content.transform.DOScale(1.1f, .07f).OnComplete(() =>
        {
            m_Content.transform.DOScale(1, .1f);
        });
    }
}
