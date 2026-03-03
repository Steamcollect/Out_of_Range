using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using UnityEngine;

public class TurretEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_AttackRange;
    [SerializeField, Range(0, 360)] float m_AngleRequireToAttack = 360f;
    [SerializeField] float m_TimeToLoseTarget = 2f;
    [Space(10)]
    [SerializeField, ReadOnly] EnemyStates m_CurrentState;

    [Header("Internal References")]
    [SerializeField] PlayerDetector m_Detector;

    [Space(10)]
    [SerializeField] private RSO_PlayerController m_Player;

    private float m_LoseSightTimer;
    public event System.Action<EnemyStates> OnStateChanged;

    void Start()
    {
        m_Health.OnTakeDamage += () =>
        {
            SetState(EnemyStates.Attacking);
        };
    }

    private void FixedUpdate()
    {
        if(IsSpawning) return;

        bool canSee = m_Detector.CanSeePlayer(m_DetectionRange);

        if (canSee)
        {
            m_LoseSightTimer = m_TimeToLoseTarget;
        }
        else
        {
            m_LoseSightTimer -= Time.fixedDeltaTime;
        }

        bool isAwareOfPlayer = m_LoseSightTimer > 0;

        switch (m_CurrentState)
        {
            case EnemyStates.Idle:
                // DO SOMETHING
                break;
            case EnemyStates.Chasing:
                if(canSee) m_Combat.LookAt(m_Player.Get().GetTargetPosition());
                break;
            case EnemyStates.Attacking:
                if (canSee) m_Combat.LookAt(m_Player.Get().GetTargetPosition());
                break;
        }

        if (m_Combat.IsAttacking()) return;

        if (isAwareOfPlayer)
        {
            float dist = Vector3.Distance(transform.position, m_Player.Get().GetTargetPosition());

            if (dist <= m_AttackRange && m_Detector.IsLookDirectionWithinAngle(GetTargetPosition(), m_Combat.GetLookAtDirection(), m_AngleRequireToAttack))
            {
                SetState(EnemyStates.Attacking);
                if (!m_Combat.IsAttacking()) StartCoroutine(m_Combat.Attack());
            }
            else
            {
                SetState(EnemyStates.Chasing);
            }
        }
        else
        {
            SetState(EnemyStates.Idle);
        }
    }

    public void OnSpawn()
    {
        StartCoroutine(m_Combat.LockAttackOnSpawn());
        m_LoseSightTimer = 0;

        IsSpawning = true;
        m_Health.GainInvincibility(m_SpawnDuration);

        this.Delay(() => {
            IsSpawning = false;
            SetState(EnemyStates.Chasing);
        }, m_SpawnDuration);
    }

    private void SetState(EnemyStates newState)
    {
        if (m_CurrentState == newState) return;
        m_CurrentState = newState;
        OnStateChanged?.Invoke(m_CurrentState);
    }

    public void SetAware()
    {
        SetState(EnemyStates.Chasing);
        m_LoseSightTimer = m_TimeToLoseTarget;
    }

    #region Gizmos

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
    #endregion
}