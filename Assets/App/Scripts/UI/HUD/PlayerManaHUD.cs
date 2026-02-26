using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManaHUD : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 m_PosOffset;

    [Header("References")]
    [SerializeField] GameObject m_Content;
    [SerializeField] Image m_ManaFillImage;

    [Space]
    [SerializeField] private RSO_PlayerController m_Player;
    [SerializeField] RSO_PlayerCameraController m_Camera;

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

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!m_Camera || !m_Camera.Get()) return;
        if (!m_Player || !m_Player.Get()) return;

        transform.position = (Vector2)m_Camera.Get().GetCamera()
            .WorldToScreenPoint(m_Player.Get().GetTargetPosition()) + m_PosOffset;
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
        m_ManaFillImage.fillAmount = (float)m_Player.Get().GetPlayerMana().CurrentMana / m_Player.Get().GetPlayerMana().MaxMana;

        m_Content.transform.DOKill();
        m_Content.transform.DOScale(1.1f, .07f).OnComplete(() =>
        {
            m_Content.transform.DOScale(1, .1f);
        });
    }
}
