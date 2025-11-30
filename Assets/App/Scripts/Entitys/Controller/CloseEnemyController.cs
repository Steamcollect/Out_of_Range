using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.AI;

public class CloseEnemyController : EntityController
{
    [Header("Settings")]
    [SerializeField] float detectionRange;
    [SerializeField] float attackRange;
    [SerializeField] LayerMask detectionMask;
    [SerializeField, TagName] string playertag;

    [Space(10)]
    [SerializeField] float stopMovementOnAttackDelay;

    [Space(10)]
    [SerializeField] float attackDashForce;
    [SerializeField] ForceMode attackDashForceMode;

    bool isChasingPlayer = false;
    bool canChasePlayer = true;

    [Header("Internal References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject detectionLight;

    [Header("Input")]
    [SerializeField] RSO_PlayerController player;

    //[Header("Output")]

    private void Start()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
        
        detectionLight.SetActive(false);
        
        health.OnTakeDamage += OnTakeDamage;

        combat.GetCombatStyle().OnAttack += () =>
        {
            canChasePlayer = false;
            movement.Value.ResetVelocity();

            CoroutineUtils.Delay(this, () =>
            {
                canChasePlayer = true;
            }, stopMovementOnAttackDelay);
        };

        combat.GetCombatStyle().OnAttack += DashOnAttack;
    }

    private void Update()
    {
        agent.nextPosition = rb.position;
    }

    private void FixedUpdate()
    {
        if (!canChasePlayer) return;

        if (!isChasingPlayer)
        {
            if (IsPlayerInRange(detectionRange) && CanSeePlayer())
            {
                isChasingPlayer = true;
                FightDetectorManager.Instance?.OnEnemyStartCombat(this);
                detectionLight.SetActive(true);
            }
            return;
        }

        combat.LookAt(player.Get().GetTargetPosition());

        if (CanSeePlayer())
        {
            if (!IsPlayerInRange(attackRange))
            {
                MoveTowardPlayer();
            }
            else
            {
                if (!combat.GetCombatStyle().IsAttacking()
                    && combat.GetCombatStyle().CanAttack())
                {
                    combat.Attack();
                }
            }

        }
        else
        {
            MoveTowardPlayer();
        }
    }

    void MoveTowardPlayer()
    {
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.Get().GetTargetPosition();

        agent.SetDestination(playerPos);

        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return;

        movement.Value.Move(agent.desiredVelocity.normalized);
    }

    void DashOnAttack()
    {
        rb.AddForce(combat.GetLookAtDirection() * attackDashForce, attackDashForceMode);
    }

    void OnTakeDamage()
    {
        FightDetectorManager.Instance?.OnEnemyStartCombat(this);
        isChasingPlayer = true;
        detectionLight.SetActive(true);
    }
    bool CanSeePlayer()
    {
        Vector3 dir = (player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
        Vector3 eyePos = combat.GetVerticalPivotPos(); // bien plus robuste
        Ray ray = new Ray(eyePos, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, detectionMask))
        {
            if (hit.collider.CompareTag(playertag)) return true;
        }

        return false;
    }

    bool IsPlayerInRange(float range)
    {
        Vector3 enemyPos = GetTargetPosition();
        Vector3 playerPos = player.Get().GetTargetPosition();

        enemyPos.y = 0;
        playerPos.y = 0;

        return Vector3.Distance(enemyPos, playerPos) < range;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}