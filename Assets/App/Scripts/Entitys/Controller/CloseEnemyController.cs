using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.AI;

public class CloseEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float attackRange;

    [Header("Internal References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemyState state;

    [Space(10)]
    [SerializeField] RSO_PlayerController player;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void Update()
    {
        agent.nextPosition = rb.position;
    }

    private void FixedUpdate()
    {

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void OnSpawn()
    {
        state.SetState(EnemyStates.Chasing);
    }
}