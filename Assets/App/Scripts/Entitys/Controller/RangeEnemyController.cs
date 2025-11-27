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
    }

    private void FixedUpdate()
    {
        if (!isChasingPlayer)
        {
            if (IsPlayerInRange(detectionRange) && CanSeePlayer())
                isChasingPlayer = true;
        }
        else
        {
            combat.LookAt(player.Get().GetTargetPosition());

            if (CanSeePlayer())
            {
                if(!IsPlayerInRange(detectionRange))
                {
                    MoveTowardPlayer();
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
    }

    void MoveTowardPlayer()
    {
        agent.SetDestination(player.Get().GetTargetPosition());
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