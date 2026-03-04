using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] RSO_PlayerController m_Player;
    [SerializeField] Transform m_Pivot;

    private void Update()
    {
        m_Pivot.LookAt(m_Player.Get().GetTargetPosition());
    }
}
