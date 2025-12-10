using System.Collections;
using MVsToolkit.Dev;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CloseEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_MinChaseRange;
    [SerializeField] float m_AttackRange;

    [Space(5)]
    [SerializeField] float m_AngleRequireToAttack;
    bool m_CanAttack = true;

    [Space(10)]
    [SerializeField] float m_DelayBetweenAttacks;

    [SerializeField, ReadOnly] EnemyStates m_CurrentState;

    [Header("Internal References")]
    [SerializeField] NavMeshAgent m_Agent;
    [SerializeField] PlayerDetector m_Detector;

    [Space(10)]
    [SerializeField] RSO_PlayerController m_Player;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_Agent.updatePosition = false;
        m_Agent.updateRotation = false;

        m_Health.OnTakeDamage += () =>
        {
            if (m_CurrentState == EnemyStates.Idle)
                m_CurrentState = EnemyStates.Chasing;
        };
    }

    private void Update()
    {
        m_Agent.nextPosition = m_Rb.position;
    }

    private void FixedUpdate()
    {
        if (m_CurrentState == EnemyStates.Chasing)
        {
            if(m_CanAttack
                && m_Detector.IsPlayerInRange(m_AttackRange)
                && m_Detector.IsLookDirectionWithinAngle(GetTargetPosition(), m_Combat.GetLookAtDirection(), m_AngleRequireToAttack)) // Not check if wall between player and enemy
            {
                StartCoroutine(Attack());
            }
            else
            {
                m_Combat.LookAt(transform.position + m_Agent.desiredVelocity.normalized, LookAtAxis.Horizontal);
                
                if (!m_Detector.IsPlayerInRange(m_MinChaseRange))
                    MoveTowardPlayer();
            }
        }
        else if(m_CurrentState == EnemyStates.Idle 
            && m_Detector.CanSeePlayer(m_DetectionRange) 
            && m_Detector.IsPlayerInRange(m_DetectionRange))
        {
            m_CurrentState = EnemyStates.Chasing;
        }
    }

    IEnumerator Attack()
    {
        m_CurrentState = EnemyStates.Attacking;
        m_CanAttack = false;

        yield return StartCoroutine(m_Combat.Attack());
        m_CurrentState = EnemyStates.Chasing;

        yield return new WaitForSeconds(m_DelayBetweenAttacks);
        m_CanAttack = true;
    }

    void MoveTowardPlayer()
    {
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = m_Player.Get().GetTargetPosition();

        m_Agent.SetDestination(playerPos);

        if (!m_Agent.hasPath || m_Agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return;

        m_Movement.Value.Move(m_Agent.desiredVelocity.normalized);
    }

    public void OnSpawn()
    {
        m_CurrentState = EnemyStates.Chasing;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTargetPosition(), .2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);

        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, m_MinChaseRange);

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