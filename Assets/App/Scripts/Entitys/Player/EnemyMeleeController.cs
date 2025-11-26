using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeController : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float moveSpeed = 3.5f;
    public float attackRate = 1f;
    public float attackDamage = 20f;
    public float attackDistance = 2.0f;
    public float attackDelay = 0.5f;
    //DISTANCE DE DETECTION

    [Header("References")]
    public Transform target;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject attackZone;
    private float lastAttackTime;

    private bool isAlive = true;

    void Awake()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (!isAlive || target == null) return;
        lastAttackTime += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;

            if (lastAttackTime >= attackRate)
            {
                lastAttackTime = 0;
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        //FEEDBACK DE PREPARATION D'ATTAQUE
        yield return new WaitForSeconds(attackDelay);

        //FEEDBACKS D'ATTAQUE
    }

    public void TakeDamage(float amount)
    {
        if (!isAlive) return;

        //APPELER FEEDBACKS DE DEGATS
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;
        agent.isStopped = true;

        //A MODIFIER, Placer anim et autres effets de morts ect...
        Destroy(gameObject, 1f);
    }
}
