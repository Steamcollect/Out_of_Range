using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Serialization;

public class RollingEnemyController : EntityController
{
    [FormerlySerializedAs("detectionRange")]
    [Header("Settings")]
    [SerializeField] private float m_DetectionRange;

    [FormerlySerializedAs("rotationTime")] [SerializeField] private float m_RotationTime;
    [FormerlySerializedAs("damage")] [SerializeField] private int m_Damage;

    [FormerlySerializedAs("stunDelay")] [SerializeField] private float m_StunDelay;

    [FormerlySerializedAs("detectionMask")] [Space(10)] [SerializeField] private LayerMask m_DetectionMask;

    [FormerlySerializedAs("playerTag")] [SerializeField] [TagName] private string m_PlayerTag;

    [FormerlySerializedAs("player")]
    [Header("References")]
    [SerializeField] private RSO_PlayerController m_Player;

    private bool m_IsChasingPlayer;

    private bool m_IsStun;

    private Vector3 m_RollDir, m_DirVelocity;

    private void FixedUpdate()
    {
        if (!m_IsStun && !m_IsChasingPlayer)
        {
            if (IsPlayerInRange(m_DetectionRange) && CanSeePlayer())
            {
                m_RollDir = (m_Player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
                m_IsChasingPlayer = true;
            }

            return;
        }

        if (m_IsChasingPlayer)
        {
            Vector3 targetDir = (m_Player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
            m_RollDir = Vector3.SmoothDamp(m_RollDir, targetDir, ref m_DirVelocity, m_RotationTime);

            m_Combat.LookAt(transform.position + GetTargetPosition() + m_RollDir);
            m_Movement.Value.Move(m_RollDir);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(m_PlayerTag))
            if (collision.collider.TryGetComponent(out EntityTrigger trigger))
                trigger.GetController().GetHealth().TakeDamage(m_Damage);

        m_DirVelocity = Vector3.zero;
        StartCoroutine(StunCooldown());
        m_IsChasingPlayer = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);
    }

    private IEnumerator StunCooldown()
    {
        m_IsStun = true;
        yield return new WaitForSeconds(m_StunDelay);
        m_IsStun = false;
    }

    private bool CanSeePlayer()
    {
        Vector3 dir = (m_Player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
        Ray ray = new(transform.position, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_DetectionRange, m_DetectionMask))
            if (hit.collider.CompareTag(m_PlayerTag))
                return true;

        return false;
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(m_Player.Get().GetTargetPosition(), GetTargetPosition()) < range;
    }
}