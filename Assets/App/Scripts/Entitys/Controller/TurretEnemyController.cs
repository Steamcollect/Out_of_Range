using System.Collections;
using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class TurretEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] private float m_AttackRange;

    [SerializeField] private float m_DetectionRange;
    [FormerlySerializedAs("bonusRangeWhenAttacked")] [SerializeField] private float m_BonusRangeWhenAttacked;
    [FormerlySerializedAs("delayWhenAttacked")] [SerializeField] private float m_DelayWhenAttacked;
    [FormerlySerializedAs("detectionMask")] [SerializeField] private LayerMask m_DetectionMask;
    [FormerlySerializedAs("playertag")] [SerializeField] [TagName] private string m_Playertag;

    [FormerlySerializedAs("delayToAim")] [Space(10)] [SerializeField] private float m_DelayToAim;

    [FormerlySerializedAs("delayBeforeAttack")] [SerializeField] private float m_DelayBeforeAttack;
    [FormerlySerializedAs("delayAfterAttack")] [SerializeField] private float m_DelayAfterAttack;

    [FormerlySerializedAs("laserMask")] [Space(10)] [SerializeField] private LayerMask m_LaserMask;

    [FormerlySerializedAs("laserMaxDistance")] [SerializeField] private float m_LaserMaxDistance;

    [FormerlySerializedAs("TargetGradient")] [Space(10)] [SerializeField] private Gradient m_TargetGradient;

    [FormerlySerializedAs("OnShootGradient")] [SerializeField] private Gradient m_OnShootGradient;

    [FormerlySerializedAs("detectionLight")]
    [Header("Internal References")]
    [SerializeField] private GameObject m_DetectionLight;

    [FormerlySerializedAs("lineRenderer")] [SerializeField] private LineRenderer m_LineRenderer;
    [FormerlySerializedAs("laserOrigin")] [SerializeField] private Transform m_LaserOrigin;

    [FormerlySerializedAs("player")]
    [Header("Input")]
    [SerializeField] private RSO_PlayerController m_Player;

    private bool m_CanLookAtPlayer = true;
    private bool m_IsAttacking;

    private bool m_IsTargetingPlayer;

    private Vector3 m_LastPlayerPos;

    private void Start()
    {
        SetInactive();

        m_Health.OnTakeDamage += OnTakeDamage;
    }

    private void Update()
    {
        UpdateLaserPosition(m_LastPlayerPos);

        if (CanSeePlayer() && m_CanLookAtPlayer)
        {
            m_Combat.LookAt(m_LastPlayerPos);
            m_LastPlayerPos = m_Player.Get().GetTargetPosition();
        }

        if (IsPlayerInRange(m_DetectionRange) && CanSeePlayer()) SetAware();

        if (IsPlayerInRange(m_AttackRange)) SetAttacking();

        if (IsPlayerInRange(m_AttackRange) && !CanSeePlayer())
        {
            m_IsTargetingPlayer = false;
            SetAttacking();
        }

        if (!IsPlayerInRange(m_DetectionRange) && !IsPlayerInRange(m_AttackRange)) SetInactive();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_AttackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);
    }

    public void OnSpawn()
    {
        SetAware();
    }

    private void SetInactive()
    {
        m_IsTargetingPlayer = false;
        m_DetectionLight.SetActive(false);
        m_LineRenderer.enabled = false;
    }

    private void SetAware()
    {
        if (!m_IsTargetingPlayer)
        {
            m_IsTargetingPlayer = true;
            m_DetectionLight.SetActive(true);
            m_LineRenderer.enabled = true;

            FightDetectorManager.S_Instance?.OnEnemyStartCombat(this);
        }
    }

    private void SetAttacking()
    {
        if (!m_IsAttacking) StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        m_IsAttacking = true;
        m_LineRenderer.material.color = Color.red;
        yield return new WaitForSeconds(m_DelayToAim);

        m_LineRenderer.material.color = Color.white;

        m_CanLookAtPlayer = false;
        yield return new WaitForSeconds(m_DelayBeforeAttack);

        m_Combat.Attack();
        m_Combat.GetCombatStyle().Reload();
        m_LineRenderer.material.color = Color.red;

        yield return new WaitForSeconds(m_DelayAfterAttack);

        m_CanLookAtPlayer = true;
        m_IsAttacking = false;
    }

    private void UpdateLaserPosition(Vector3 targetPosition)
    {
        if (m_LineRenderer == null) return;

        m_LineRenderer.SetPosition(0, m_LaserOrigin.position);

        Ray ray = new(m_LaserOrigin.position, m_LaserOrigin.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_LaserMaxDistance, m_LaserMask))
        {
            m_LineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            Vector3 endPoint = m_LaserOrigin.position + m_LaserOrigin.forward * m_LaserMaxDistance;
            m_LineRenderer.SetPosition(1, endPoint);
        }
    }

    private void OnTakeDamage()
    {
        m_DetectionRange += m_BonusRangeWhenAttacked;
        m_AttackRange += m_BonusRangeWhenAttacked;
        this.Delay(() => m_DetectionRange -= m_BonusRangeWhenAttacked, new WaitForSeconds(m_DelayWhenAttacked));
        this.Delay(() => m_AttackRange -= m_BonusRangeWhenAttacked, new WaitForSeconds(m_DelayWhenAttacked));
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