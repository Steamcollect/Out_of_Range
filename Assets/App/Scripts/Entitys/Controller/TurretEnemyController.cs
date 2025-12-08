using System.Collections;
using MVsToolkit.Dev;
using MVsToolkit.Utils;
using UnityEngine;

public class TurretEnemyController : EntityController, ISpawnable
{
    [Header("Settings")]
    [SerializeField] private float m_AttackRange;
    [SerializeField] private float m_DetectionRange;
    [SerializeField] private float bonusRangeWhenAttacked;
    [SerializeField] private float delayWhenAttacked;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField, TagName] private string playertag;
    [Space(10)]
    [SerializeField] private float delayToAim;
    [SerializeField] private float delayBeforeAttack;
    [SerializeField] private float delayAfterAttack;
    [Space(10)]
    [SerializeField] private LayerMask laserMask;
    [SerializeField] private float laserMaxDistance;

    [Space(10)]
    [SerializeField] Gradient TargetGradient;
    [SerializeField] Gradient OnShootGradient;
    
    [Header("Internal References")]
    [SerializeField] private GameObject detectionLight;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform laserOrigin;

    [Header("Input")]
    [SerializeField] private RSO_PlayerController player;

    private bool isTargetingPlayer = false;
    private bool canLookAtPlayer = true;
    private bool isAttacking = false;

    private Vector3 lastPlayerPos;

    private void Start()
    {
        SetInactive();

        health.OnTakeDamage += OnTakeDamage;
    }

    private void SetInactive()
    {
        isTargetingPlayer = false;
        detectionLight.SetActive(false);
        lineRenderer.enabled = false;
    }

    private void SetAware()
    {
        if (!isTargetingPlayer)
        {
            isTargetingPlayer = true;
            detectionLight.SetActive(true);
            lineRenderer.enabled = true;

            FightDetectorManager.Instance?.OnEnemyStartCombat(this);
        }
    }

    private void SetAttacking()
    {
        if (!isAttacking) StartCoroutine(Attack());
    }

    private void Update()
    {
        UpdateLaserPosition(lastPlayerPos);

        if (CanSeePlayer() && canLookAtPlayer)
        {
            combat.LookAt(lastPlayerPos);
            lastPlayerPos = player.Get().GetTargetPosition();
        }

        if (IsPlayerInRange(m_DetectionRange) && CanSeePlayer())
        {
            SetAware();
        }

        if (IsPlayerInRange(m_AttackRange))
        {
            SetAttacking();
        }

        if(IsPlayerInRange(m_AttackRange) && !CanSeePlayer())
        {
            isTargetingPlayer = false;
            SetAttacking();
        }

        if(!IsPlayerInRange(m_DetectionRange) && !IsPlayerInRange(m_AttackRange))
        {
            SetInactive();
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        lineRenderer.material.color = Color.red;
        yield return new WaitForSeconds(delayToAim);

        lineRenderer.material.color = Color.white;

        canLookAtPlayer = false;
        yield return new WaitForSeconds(delayBeforeAttack);

        combat.Attack();
        combat.GetCombatStyle().Reload();
        lineRenderer.material.color = Color.red;

        yield return new WaitForSeconds(delayAfterAttack);

        canLookAtPlayer = true;
        isAttacking = false;
    }

    private void UpdateLaserPosition(Vector3 targetPosition)
    {
        if (lineRenderer == null) return;

        lineRenderer.SetPosition(0, laserOrigin.position);

        Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, laserMaxDistance, laserMask))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            Vector3 endPoint = laserOrigin.position + (laserOrigin.forward * laserMaxDistance);
            lineRenderer.SetPosition(1, endPoint);
        }
    }

    void OnTakeDamage()
    {
        m_DetectionRange += bonusRangeWhenAttacked;
        m_AttackRange += bonusRangeWhenAttacked;
        CoroutineUtils.Delay(this, () => m_DetectionRange -= bonusRangeWhenAttacked, new WaitForSeconds(delayWhenAttacked));
        CoroutineUtils.Delay(this, () => m_AttackRange -= bonusRangeWhenAttacked, new WaitForSeconds(delayWhenAttacked));
    }
    private bool CanSeePlayer()
    {
        Vector3 dir = (player.Get().GetTargetPosition() - GetTargetPosition()).normalized;
        Vector3 eyePos = combat.GetVerticalPivotPos(); // bien plus robuste
        Ray ray = new Ray(eyePos, dir);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_DetectionRange, detectionMask))
        {
            if (hit.collider.CompareTag(playertag)) return true;
        }

        return false;
    }

    private bool IsPlayerInRange(float range)
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
        Gizmos.DrawWireSphere(transform.position, m_AttackRange); 
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRange);
    }

    public void OnSpawn()
    {
        SetAware();
    }
}