using UnityEngine;

public class HUDFollowPlayer : MonoBehaviour
{
    [SerializeField] Vector2 m_PosOffset;

    [Space]
    [SerializeField] RSO_PlayerController m_Player;
    [SerializeField] RSO_PlayerCameraController m_Camera;

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
}
