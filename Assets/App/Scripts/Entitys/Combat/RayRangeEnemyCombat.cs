using FMODUnity;
using MoreMountains.Feedbacks;
using MVsToolkit.Dev;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RayRangeEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] private int m_Damage;
    [Space(5)]
    [SerializeField] private Vector2 m_TimeBeforeAttack;
    [SerializeField] private float m_TimeAfterAttack;
    [SerializeField] private float m_TimeBetweenAttacks;
    [Space(5)]
    [SerializeField] private LayerMask m_AttackMask;
    [SerializeField, TagName] private string m_PlayerTag;

    [Header("References")]
    [SerializeField] private Transform m_AttackPoint;
    [Space(5)]
    [SerializeField] private RSO_PlayerController m_Player;
    [SerializeField] private GameObject m_ImpactFeedback;

    [Space(10)]
    [SerializeField] EventReference m_OnShootFx;

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnShoot;

    public event Action<float /*DelayBeforeShoot*/, float /*DelayAfterShoot*/> OnShootLaunched;
    public event Action OnShootCompleted;

    private void Start()
    {
        SetTurnSmoothTime(m_TurnSmoothTime);
    }

    public override IEnumerator Attack()
    {
        if (!m_CanAttackOnSpawn) yield break;
        m_IsAttacking = true;

        yield return new WaitForSeconds(m_TimeBetweenAttacks);

        SetActiveLookAt(false);

        float timeBeforeAttack = UnityEngine.Random.Range(Mathf.Clamp(m_TimeBeforeAttack.x, 1.2f, m_TimeBeforeAttack.x), m_TimeBeforeAttack.y);
        OnShootLaunched?.Invoke(timeBeforeAttack, m_TimeAfterAttack);

        yield return new WaitForSeconds(timeBeforeAttack - 1.2f);
        RuntimeManager.PlayOneShot(m_OnShootFx, transform.position);
        yield return new WaitForSeconds(1.2f);


        Ray ray = new Ray(m_AttackPoint.position, m_AttackPoint.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f, m_AttackMask))
        {
            if (hit.collider.TryGetComponent(out HurtBox hurtBox))
            {
                hurtBox.TakeDamage(m_Damage);
                if(m_ImpactFeedback) 
                {
                    GameObject impact = Instantiate(m_ImpactFeedback, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    Destroy(impact, 5f);
                }
            }
        }

        m_OnShoot.Invoke();

        yield return new WaitForSeconds(m_TimeAfterAttack);

        SetActiveLookAt(true);
        OnShootCompleted?.Invoke();

        m_IsAttacking = false;
    }
}