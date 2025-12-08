using System.Collections;
using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.AI;

public class CloseEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_AttackRange;

    [Space(10)]
    [SerializeField] float m_AngleRequireToAttack;

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
    }

    private void Update()
    {
        m_Agent.nextPosition = m_Rb.position;
    }

    private void FixedUpdate()
    {
        if (m_CurrentState == EnemyStates.Chasing)
        {
            if(m_Detector.IsPlayerInRange(m_AttackRange))
            {
                StartCoroutine(Attack());
            }
            else
            {
                MoveTowardPlayer();
                m_Combat.LookAt(transform.position + m_Agent.desiredVelocity.normalized);
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
        yield return StartCoroutine(m_Combat.Attack());
        m_CurrentState = EnemyStates.Chasing;
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
    }

    public void OnSpawn()
    {
        m_CurrentState = EnemyStates.Chasing;
    }
}