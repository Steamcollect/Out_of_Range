using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;

public class TurretEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_AttackRange;
    [SerializeField, Range(1, 180)] float m_AngleRequireToAttack;

    [Space(10)]
    [SerializeField] float m_TimeBetweenAttacks;

    [Space(10)]
    [SerializeField, ReadOnly] EnemyStates m_CurrentState;

    bool m_CanAttack = true;

    [Header("Internal References")]
    [SerializeField] PlayerDetector m_Detector;

    [Space(10)]
    [SerializeField] private RSO_PlayerController m_Player;

    void Start()
    {
        m_Health.OnTakeDamage += () =>
        {
            if (m_CurrentState == EnemyStates.Idle)
                m_CurrentState = EnemyStates.Chasing;
        };
    }

    private void FixedUpdate()
    {
        if (m_CurrentState == EnemyStates.Chasing)
        {
            if (m_Detector.CanSeePlayer(m_DetectionRange))
            {
                m_Combat.LookAt(m_Player.Get().GetTargetPosition());

                if (m_CanAttack
                    && m_Detector.IsLookDirectionWithinAngle(GetTargetPosition(), m_Combat.GetLookAtDirection(), m_AngleRequireToAttack))
                    StartCoroutine(Attack());
            }
        }
        else if (m_CurrentState == EnemyStates.Idle
            && m_Detector.IsPlayerInRange(m_DetectionRange)
            && m_Detector.CanSeePlayer(m_DetectionRange))
        {
            m_CurrentState = EnemyStates.Chasing;
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator Attack()
    {
        m_CurrentState = EnemyStates.Attacking;
        yield return StartCoroutine(m_Combat.Attack());
        m_CurrentState = EnemyStates.Chasing;

        StartCoroutine(AttackCooldown());

    }

    IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_TimeBetweenAttacks);
        m_CanAttack = true;
    }

    public void OnSpawn()
    {
        m_CurrentState = EnemyStates.Chasing;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTargetPosition(), .2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);

        Gizmos.color = Color.cyan;
        Vector3 forward = transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector3.forward;
        }
        forward.Normalize();

        float halfAngle = m_AngleRequireToAttack * 0.5f;

        Vector3 dirLeft = Quaternion.Euler(0f, -halfAngle, 0f) * forward;
        Vector3 dirRight = Quaternion.Euler(0f, halfAngle, 0f) * forward;

        Gizmos.DrawLine(transform.position, transform.position + dirLeft * m_DetectionRange);
        Gizmos.DrawLine(transform.position, transform.position + dirRight * m_DetectionRange);
    }
}