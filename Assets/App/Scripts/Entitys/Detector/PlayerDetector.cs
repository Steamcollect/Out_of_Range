using MVsToolkit.Dev;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask m_DetectionMask;
    [SerializeField, TagName] string m_PlayerTag;

    [Header("References")]
    [SerializeField] RSO_PlayerController m_Player;
    [SerializeField] EntityController m_Controller;

    //[Header("Input")]
    //[Header("Output")]

    public float DistanceFromPlayer()
    {
        return Vector3.Distance(m_Player.Get().GetTargetPosition(), m_Controller.GetTargetPosition());
    }

    public bool CanSeePlayer(float maxDist = 30)
    {
        Vector3 dir = (m_Player.Get().GetTargetPosition() - m_Controller.GetTargetPosition()).normalized;
        Vector3 eyePos = m_Controller.GetTargetPosition();
        Ray ray = new Ray(eyePos, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDist, m_DetectionMask))
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

    public bool IsLookDirectionWithinAngle(Vector3 current, Vector3 lookDir, float angleDegrees)
    {
        if (lookDir.sqrMagnitude <= 0f)
        {
            return false;
        }

        Vector3 toTarget = m_Player.Get().GetTargetPosition() - current;
        if (toTarget.sqrMagnitude <= 0f)
        {
            return false;
        }

        float angleBetween = Vector3.Angle(lookDir.normalized, toTarget.normalized);
        return angleBetween <= Mathf.Abs(angleDegrees);
    }
}