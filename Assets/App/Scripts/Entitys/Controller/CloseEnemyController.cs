using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class CloseEnemyController : EntityController, ISpawnable
{
    [FormerlySerializedAs("detectionRange")]
    [Header("Settings")]
    [SerializeField] private float m_DetectionRange;

    [FormerlySerializedAs("attackRange")] [SerializeField] private float m_AttackRange;
    [FormerlySerializedAs("detectionMask")] [SerializeField] private LayerMask m_DetectionMask;
    [FormerlySerializedAs("playertag")] [SerializeField] [TagName] private string m_Playertag;

    [FormerlySerializedAs("stopMovementOnAttackDelay")] [Space(10)] [SerializeField] private float m_StopMovementOnAttackDelay;

    [FormerlySerializedAs("attackDashForce")] [Space(10)] [SerializeField] private float m_AttackDashForce;

    [FormerlySerializedAs("attackDashForceMode")] [SerializeField] private ForceMode m_AttackDashForceMode;

    [FormerlySerializedAs("agent")]
    [Header("Internal References")]
    [SerializeField] private NavMeshAgent m_Agent;

    [FormerlySerializedAs("detectionLight")] [SerializeField] private GameObject m_DetectionLight;

    [FormerlySerializedAs("player")]
    [Header("Input")]
    [SerializeField] private RSO_PlayerController m_Player;

    private bool m_CanChasePlayer = true;

    private bool m_IsChasingPlayer;

    //[Header("Output")]

    private void Start()
    {
        m_Agent.updatePosition = false;
        m_Agent.updateRotation = false;

        m_DetectionLight.SetActive(false);

        m_Health.OnTakeDamage += OnTakeDamage;

        m_Combat.GetCombatStyle().OnAttack += () =>
        {
            m_CanChasePlayer = false;
            m_Movement.Value.ResetVelocity();

            this.Delay(() => { m_CanChasePlayer = true; }, m_StopMovementOnAttackDelay);
        };

        m_Combat.GetCombatStyle().OnAttack += DashOnAttack;
    }

    private void Update()
    {
        m_Agent.nextPosition = m_Rb.position;
    }

    private void FixedUpdate()
    {
        if (!m_CanChasePlayer) return;

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
            if (!IsPlayerInRange(m_AttackRange))
            {
                MoveTowardPlayer();
            }
            else
            {
                if (!m_Combat.GetCombatStyle().IsAttacking()
                    && m_Combat.GetCombatStyle().CanAttack())
                    m_Combat.Attack();
            }
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
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);
    }

    public void OnSpawn()
    {
        m_IsChasingPlayer = true;
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

    private void DashOnAttack()
    {
        m_Rb.AddForce(m_Combat.GetLookAtDirection() * m_AttackDashForce, m_AttackDashForceMode);
    }

    private void OnTakeDamage()
    {
        FightDetectorManager.S_Instance?.OnEnemyStartCombat(this);
        m_IsChasingPlayer = true;
        m_DetectionLight.SetActive(true);
    }

    private bool CanSeePlayer()
    {
        Vector3 dir = (m_Player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
        Vector3 eyePos = m_Combat.GetVerticalPivotPos(); // bien plus robuste
        Ray ray = new(eyePos, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_DetectionRange, m_DetectionMask))
            if (hit.collider.CompareTag(m_Playertag))
                return true;

        return false;
    }

    private bool IsPlayerInRange(float range)
    {
        Vector3 enemyPos = GetTargetPosition();
        Vector3 playerPos = m_Player.Get().GetTargetPosition();

        enemyPos.y = 0;
        playerPos.y = 0;

        return Vector3.Distance(enemyPos, playerPos) < range;
    }
}