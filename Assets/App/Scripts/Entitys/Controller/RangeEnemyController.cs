using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.AI;

public class RangeEnemyController : EntityController
{
    [Header("Settings")]
    [SerializeField] float detectionRange;
    [SerializeField] float closeRange;
    [SerializeField] LayerMask detectionMask;
    [SerializeField, TagName] string playertag;

    bool isChasingPlayer = false;

    [Header("Internal References")]
    [SerializeField] NavMeshAgent agent;

    [Header("Input")]
    [SerializeField] RSO_PlayerController player;

    //[Header("Output")]

    private void Start()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;

        health.OnTakeDamage += OnTakeDamage;
    }

    private void Update()
    {
        agent.nextPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!isChasingPlayer)
        {
            if (IsPlayerInRange(detectionRange) && CanSeePlayer())
            {
                isChasingPlayer = true;
                FightDetectorManager.Instance?.OnEnemyStartCombat(this);
            }
            return;
        }

        combat.LookAt(player.Get().GetTargetPosition());

        if (CanSeePlayer())
        {
            if (!IsPlayerInRange(detectionRange))
            {
                MoveTowardPlayer();
            }
            else if (IsPlayerInRange(closeRange))
            {
                StayBack();
            }
            else
            {
                combat.Attack();
            }
        }
        else
        {
            MoveTowardPlayer();
        }
    }

    void StayBack()
    {
        Vector3 awayDir = (GetTargetPosition() - player.Get().GetTargetPosition()).normalized;
        Vector3 target = GetTargetPosition() + awayDir * closeRange;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(target, out navHit, 2f, agent.areaMask))
        {
            agent.SetDestination(navHit.position);
            movement.Value.Move(agent.desiredVelocity.normalized);
        }

        Debug.DrawLine(GetTargetPosition(), target, Color.blue);
    }

    void OnTakeDamage()
    {
        FightDetectorManager.Instance?.OnEnemyStartCombat(this);
        isChasingPlayer = true;
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

    bool CanSeePlayer()
    {
        Vector3 dir = (player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
        Ray ray = new Ray(transform.position, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, detectionMask))
        {
            if (hit.collider.CompareTag(playertag)) return true;
        }

        return false;
    }

    bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(player.Get().GetTargetPosition(), GetTargetPosition()) < range;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeRange);
    }
}