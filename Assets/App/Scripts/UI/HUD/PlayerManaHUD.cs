using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject m_Content;
    [SerializeField] TMP_Text m_PercentageTxt;

    [Space]
    [SerializeField] Image m_ManaBackgroundImage;
    [SerializeField] Image m_ManaFillImage;

    [Space]
    [SerializeField] RSO_PlayerController m_Player;

    int lastValue;

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
        m_Content.transform.DOKill();
        m_Content.transform.DOScale(1.2f, .07f).OnComplete(() =>
        {
            m_Content.transform.DOScale(1, .1f);
        });

        lastValue = m_Player.Get().GetPlayerMana().CurrentMana;
        float value = (float)m_Player.Get().GetPlayerMana().CurrentMana / m_Player.Get().GetPlayerMana().MaxMana;

        m_ManaFillImage.fillAmount = value * m_ManaBackgroundImage.fillAmount;
        m_PercentageTxt.text = (value * 100).ToString("F0") + "%";
    }
}
