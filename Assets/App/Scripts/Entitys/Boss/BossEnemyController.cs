using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using MVsToolkit.Utilities;

public class BossEnemyController : EntityController, ISpawnable, ITargetable
{
    [Header("Settings")]
    [SerializeField] float m_DetectionRange;
    [SerializeField] float m_AttackRange;
    [SerializeField, Range(0, 360)] float m_AngleRequireToAttack = 20;
    [SerializeField] float m_TimeToLoseTarget = 2f;
    [Space(10)]
    [SerializeField, ReadOnly] EnemyStates m_CurrentState;

    [Header("Internal References")]
    [SerializeField] PlayerDetector m_Detector;
    [SerializeField, ReadOnly] List<CombatPatern> m_CombatPaterns;
    CombatPatern m_CurrentPatern;

    [ReadOnly] public bool HaveAtkSpeedPowerUp;
    [ReadOnly] public bool HaveClonePowerUp;
    [ReadOnly] public bool HaveStrenghtPowerUp;
    [HideInInspector] public bool CanHandlePaterns;

    [SerializeField] BossHealthStep[] m_HealthSteps;

    [Space(10)]
    [SerializeField] private RSO_PlayerController m_Player;

    private float m_LoseSightTimer;
    public event System.Action<EnemyStates> OnStateChanged;

    [System.Serializable]
    class BossHealthStep
    {
        public int HealthRequire;
        [HideInInspector] public bool IsUsed = false;
        public UnityEvent Callback;
    }

    void Start()
    {
        m_Health.OnTakeDamage += () =>
        {
            if(IsSpawning) return;

            SetState(EnemyStates.Attacking);

            foreach (BossHealthStep step in m_HealthSteps)
            {
                if (!step.IsUsed && m_Health.GetCurrentHealth() <= step.HealthRequire)
                {
                    step.Callback?.Invoke();
                    step.IsUsed = true;
                }
            }
        };
    }

    private void FixedUpdate()
    {
        if(IsSpawning || !CanHandlePaterns) return;

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
                if (canSee && m_CurrentPatern.Combat) m_Combat.LookAt(m_Player.Get().GetTargetPosition());
                break;
            case EnemyStates.Attacking:
                if (canSee && m_CurrentPatern.Combat) m_Combat.LookAt(m_Player.Get().GetTargetPosition());
                break;
        }

        if (m_CurrentPatern.Combat && m_CurrentPatern.Combat.IsAttacking()) return;

        SelectRandomPatern();

        if (isAwareOfPlayer)
        {
            float dist = Vector3.Distance(transform.position, m_Player.Get().GetTargetPosition());

            if (dist <= m_AttackRange && m_Detector.IsLookDirectionWithinAngle(GetTargetPosition(), m_CurrentPatern.Combat.GetLookAtDirection(), m_AngleRequireToAttack))
            {
                SetState(EnemyStates.Attacking);
                StartCoroutine(m_CurrentPatern.Combat.Attack());
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
        SetState(EnemyStates.Chasing);
        m_LoseSightTimer = 0;

        IsSpawning = true;
        m_Health.GainInvincibility(m_SpawnDuration);

        this.Delay(() =>         {
            IsSpawning = false;
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
        m_CurrentState = EnemyStates.Chasing;
        m_LoseSightTimer = m_TimeToLoseTarget;
    }

    void SelectRandomPatern()
    {
        if (m_CombatPaterns == null || m_CombatPaterns.Count == 0)
        {
            Debug.LogWarning("No combat patterns assigned.");
            return;
        }

        float total = 0f;
        foreach (var p in m_CombatPaterns)
            total += p.UseProbability;

        if (total <= 0f)
        {
            Debug.LogWarning("Total probability is zero. Cannot select a pattern.");
            return;
        }

        float r = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var p in m_CombatPaterns)
        {
            cumulative += p.UseProbability;
            if (r <= cumulative)
            {
                m_CurrentPatern = p;
                return;
            }
        }

        m_CurrentPatern = m_CombatPaterns[m_CombatPaterns.Count - 1];
    }

    public void AddPatern(CombatPatern patern) =>m_CombatPaterns.Add(patern);
    public void RemovePatern(CombatPatern patern) =>m_CombatPaterns.Remove(patern);

    public void SetAtkSpeedPowerUp(bool value) => HaveAtkSpeedPowerUp = value;
    public void SetClonePowerUp(bool value) => HaveClonePowerUp = value;
    public void SetStrengthPowerUp(bool value) => HaveStrenghtPowerUp = value;

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