using UnityEngine;

public class PlayerCombatStyleHUD : BaseCombatStyleHUD
{
    [Header("Player Settings")]
    [SerializeField] Vector2 m_PosOffset;

    [Header("Player References")]
    [SerializeField] RSO_PlayerCameraController m_PlayerCameraController;
    [SerializeField] RSO_PlayerController m_PlayerController;

    private void OnEnable()
    {
        m_PlayerController.Get().GetPlayerCombat().OnPrimaryCombatStyleChange += OnCombatStyleChange;
        InitBindings();
    }

    private void OnDisable()
    {
        Unbind();
        m_PlayerController.Get().GetPlayerCombat().OnPrimaryCombatStyleChange -= OnCombatStyleChange;
    }

    private void InitBindings()
    {
        CombatStyle primaryStyle = m_PlayerController.Get().GetPlayerCombat().GetPrimaryCombatStyle();
        BindToCombatStyle(primaryStyle);
    }

    private void OnCombatStyleChange()
    {
        InitBindings();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!m_PlayerCameraController || !m_PlayerCameraController.Get()) return;
        if (!m_PlayerController || !m_PlayerController.Get()) return;

        transform.position = (Vector2)m_PlayerCameraController.Get().GetCamera()
            .WorldToScreenPoint(m_PlayerController.Get().GetTargetPosition()) + m_PosOffset;
    }
}

