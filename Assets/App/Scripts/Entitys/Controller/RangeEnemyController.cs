using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class RangeEnemyController : EntityController, ISpawnable
{
    [FormerlySerializedAs("detectionRange")]
    [Header("Settings")]
    [SerializeField] private float m_DetectionRange;

    [FormerlySerializedAs("closeRange")] [SerializeField] private float m_CloseRange;
    [FormerlySerializedAs("detectionMask")] [SerializeField] private LayerMask m_DetectionMask;
    [FormerlySerializedAs("playertag")] [SerializeField] [TagName] private string m_Playertag;

    [FormerlySerializedAs("agent")]
    [Header("Internal References")]
    [SerializeField] private NavMeshAgent m_Agent;

    [FormerlySerializedAs("detectionLight")] [SerializeField] private GameObject m_DetectionLight;

    [FormerlySerializedAs("player")]
    [Header("Input")]
    [SerializeField] private RSO_PlayerController m_Player;

    private bool m_IsChasingPlayer;

    //[Header("Output")]

    private void Start()
    {
        m_Agent.updatePosition = false;
        m_Agent.updateRotation = false;

        m_DetectionLight.SetActive(false);

        m_Health.OnTakeDamage += OnTakeDamage;
    }

    private void Update()
    {
        m_Agent.nextPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!m_IsChasingPlayer)
        {
            if (IsPlayerInRange(m_DetectionRange) && CanSeePlayer())
            {
                m_IsChasingPlayer = true;
                FightDetectorManager.S_Instance?.OnEnemyStartCombat(this);
                m_DetectionLight.SetActive(true);
            }

            return;
        }

        m_Combat.LookAt(m_Player.Get().GetTargetPosition());

        if (CanSeePlayer())
        {
            if (!IsPlayerInRange(m_DetectionRange))
                MoveTowardPlayer();
            else if (IsPlayerInRange(m_CloseRange))
                StayBack();
            else
                m_Combat.Attack();
        }
        else
        {
            MoveTowardPlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_CloseRange);
    }

    public void OnSpawn()
    {
        m_IsChasingPlayer = true;
    }

    private void StayBack()
    {
        Vector3 awayDir = (GetTargetPosition() - m_Player.Get().GetTargetPosition()).normalized;
        Vector3 target = GetTargetPosition() + awayDir * m_CloseRange;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, 2f, m_Agent.areaMask))
        {
            m_Agent.SetDestination(navHit.position);
            m_Movement.Value.Move(m_Agent.desiredVelocity.normalized);
        }

        Debug.DrawLine(GetTargetPosition(), target, Color.blue);
    }

    private void OnTakeDamage()
    {
        FightDetectorManager.S_Instance?.OnEnemyStartCombat(this);
        m_IsChasingPlayer = true;
        m_DetectionLight.SetActive(true);
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

    private bool CanSeePlayer()
    {
        Vector3 dir = (m_Player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
        Ray ray = new(transform.position, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_DetectionRange, m_DetectionMask))
            if (hit.collider.CompareTag(m_Playertag))
                return true;

        return false;
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(m_Player.Get().GetTargetPosition(), GetTargetPosition()) < range;
    }
}