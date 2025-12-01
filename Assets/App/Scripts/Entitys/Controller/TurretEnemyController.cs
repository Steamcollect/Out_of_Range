using System.Collections;
using System.Linq;
using MVsToolkit.Dev;
using UnityEngine;

public class TurretEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] float detectionRange;
    [SerializeField] LayerMask detectionMask;
    [SerializeField, TagName] string playertag;

    [Space(10)]
    [SerializeField] float delayToAim;
    [SerializeField] float delayBeforeAttack;
    [SerializeField] float delayAfterAttack;

    bool isTargetingPlayer = false;
    bool canLookAtPlayer = true;
    bool isAttacking = false;

    Vector3 lastPlayerPos;

    [Header("Internal References")]
    [SerializeField] GameObject detectionLight;

    [Header("Input")]
    [SerializeField] RSO_PlayerController player;

    //[Header("Output")]

    private void Start()
    {
        detectionLight.SetActive(false);

        health.OnTakeDamage += OnTakeDamage;
    }

    private void FixedUpdate()
    {
        bool canSeePlayer = CanSeePlayer();

        if (!isTargetingPlayer && IsPlayerInRange(detectionRange) && canSeePlayer)
        {
            isTargetingPlayer = true;
            detectionLight.SetActive(true);
        }

        if (isTargetingPlayer)
        {
            if (canLookAtPlayer && canSeePlayer)
            {
                combat.LookAt(lastPlayerPos);
                lastPlayerPos = player.Get().GetTargetPosition();
            }

            if (!isAttacking) StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(delayToAim);

        canLookAtPlayer = false;
        yield return new WaitForSeconds(delayBeforeAttack);
        combat.Attack();
        yield return new WaitForSeconds(delayAfterAttack);

        canLookAtPlayer = true;
        isAttacking = false;
    }

    void OnTakeDamage()
    {
        FightDetectorManager.Instance?.OnEnemyStartCombat(this);
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
    }

    public void OnSpawn()
    {
        isTargetingPlayer = true;
    }
}