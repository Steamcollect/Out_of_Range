using MVsToolkit.Dev;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [Space(10)]
    [SerializeField] LayerMask m_DetectionMask;
    [SerializeField, TagName] string m_PlayerTag;

    [Header("References")]
    [SerializeField] RSO_PlayerController m_Player;
    [SerializeField] EntityController m_Controller;
    [SerializeField] EnemyState m_State;

    //[Header("Input")]
    //[Header("Output")]

    public float DistanceFromPlayer()
    {
        return Vector3.Distance(m_Player.Get().GetTargetPosition(), m_Controller.GetTargetPosition());
    }

    public bool CanSeePlayer()
    {
        Vector3 dir = (m_Player.Get().GetTargetPosition() - m_Controller.GetTargetPosition()).normalized;
        Vector3 eyePos = m_Controller.GetTargetPosition();
        Ray ray = new Ray(eyePos, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_DetectionRange, m_DetectionMask))
        {
            if (hit.collider.CompareTag(m_PlayerTag)) return true;
        }

        return false;
    }

    public bool IsPlayerInRange(float range)
    {
        Vector3 enemyPos = m_Controller.GetTargetPosition();
        Vector3 playerPos = m_Player.Get().GetTargetPosition();

        enemyPos.y = 0;
        playerPos.y = 0;

        return Vector3.Distance(enemyPos, playerPos) < range;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);
    }
}