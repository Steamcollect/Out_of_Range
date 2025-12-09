using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ExplodingEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_MinChaseRange;
    [SerializeField] float m_AttackRange;

    [SerializeField] float m_TimeToCheckBeforeExplosion;
    [SerializeField] float m_DelayBeforeExplosion;

    [SerializeField, ReadOnly] EnemyStates m_CurrentState;
    [SerializeField, ReadOnly] ExplosionStates m_ExplosionState;

    [Header("Internal References")]
    [SerializeField] NavMeshAgent m_Agent;
    [SerializeField] PlayerDetector m_Detector;

    [Space(10)]
    [SerializeField] RSO_PlayerController m_Player;
    //[Header("Input")]
    //[Header("Output")]

    enum ExplosionStates
    {
        Idle,
        Checking,
        Exploding
    }

    private void Start()
    {
        m_Agent.updatePosition = false;
        m_Agent.updateRotation = false;
    }

    private void Update()
    {
        m_Agent.nextPosition = m_Rb.position;
    }

    private void FixedUpdate()
    {
        if (m_ExplosionState != ExplosionStates.Idle) return;

        if (m_CurrentState == EnemyStates.Chasing)
        {
            if (m_ExplosionState == ExplosionStates.Idle
                && m_Detector.IsPlayerInRange(m_AttackRange)) // Pas de detection à travers les murs pour le moment
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
        else if (m_CurrentState == EnemyStates.Idle
            && m_Detector.CanSeePlayer(m_DetectionRange)
            && m_Detector.IsPlayerInRange(m_DetectionRange))
        {
            m_CurrentState = EnemyStates.Chasing;
        }
    }

    IEnumerator Attack()
    {
        m_CurrentState = EnemyStates.Attacking;
        m_ExplosionState = ExplosionStates.Checking;

        yield return new WaitForSeconds(m_TimeToCheckBeforeExplosion);

        if (!m_Detector.IsPlayerInRange(m_AttackRange))
        {
            m_CurrentState = EnemyStates.Chasing;
            m_ExplosionState = ExplosionStates.Idle;
            yield break;
        }
        
        m_ExplosionState = ExplosionStates.Exploding;
        yield return StartCoroutine(m_Combat.Attack());
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);

        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, m_MinChaseRange);
    }

    public void OnSpawn()
    {
        m_CurrentState = EnemyStates.Chasing;
    }
}