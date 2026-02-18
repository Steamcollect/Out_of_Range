using System.Collections;
using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.AI;

public class RangeEnemyController : EnemyController
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_BackUpRange;
    [SerializeField] float m_AttackRange;

    [Space(5)]
    [SerializeField, Range(1, 180)] float m_AngleRequireToAttack;
    bool m_CanAttack = true;

    [Header("Internal References")]
    [SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] PlayerDetector m_Detector;

    [Space(10)]
    [SerializeField] private RSO_PlayerController m_Player;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_Agent.updatePosition = false;
        m_Agent.updateRotation = false;
    }

    private void Update()
    {
        m_Agent.nextPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if(IsSpawning) return;

        if (m_CurrentState == EnemyStates.Chasing)
        {
            bool canSeePlayer = m_Detector.IsPlayerInRange(m_DetectionRange);

            if (m_Detector.IsPlayerInRange(m_BackUpRange))
            {
                if(canSeePlayer)
                {
                    BackUp();
                }
            }
            else if (m_Detector.IsPlayerInRange(m_AttackRange))
            {
                if(canSeePlayer
                    && m_CanAttack
                    && m_Detector.IsLookDirectionWithinAngle(GetTargetPosition(), m_Combat.GetLookAtDirection(), m_AngleRequireToAttack))
                {
                    StartCoroutine(Attack());
                }
            }
            else
            {
                MoveTowardPlayer();
            }
        }
        else if(m_CurrentState == EnemyStates.Idle)
        {
            if(m_Detector.IsPlayerInRange(m_DetectionRange) 
                && m_Detector.CanSeePlayer(m_DetectionRange))
            {
                m_CurrentState = EnemyStates.Chasing;
            }
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = m_Player.Get().GetTargetPosition();

        m_Agent.SetDestination(playerPos);

        if (!m_Agent.hasPath || m_Agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return;

        m_Movement.Value.Move(m_Agent.desiredVelocity.normalized);
    }
    private void BackUp()
    {
        Vector3 awayDir = (GetTargetPosition() - m_Player.Get().GetTargetPosition()).normalized;
        Vector3 target = GetTargetPosition() + awayDir * m_BackUpRange;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, 2f, m_Agent.areaMask))
        {
            m_Agent.SetDestination(navHit.position);
            m_Movement.Value.Move(m_Agent.desiredVelocity.normalized);
        }

        Debug.DrawLine(GetTargetPosition(), target, Color.blue);
    }

    IEnumerator Attack()
    {
        m_CurrentState = EnemyStates.Attacking;
        yield return StartCoroutine(m_Combat.Attack());
        m_CurrentState = EnemyStates.Chasing;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);

        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, m_BackUpRange);

        // draw cone edges and arc for m_AngleRequireToAttack
        Gizmos.color = Color.cyan;

        // use forward on horizontal plane
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