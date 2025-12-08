using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CloseEnemyCombat : EntityCombat
{
    [Header("Settings")]
    [SerializeField] int m_Damage;
    [SerializeField] float m_AttackCooldown;

    [Space(5)]
    [SerializeField] float m_AttackBeginDelay = .2f;
    [SerializeField] float m_AttackFinishedDelay = .2f;

    [Space(10)]
    [SerializeField] float attackDashForce;
    [SerializeField] ForceMode dashForceMode;

    bool m_CanAttack = true;
    bool m_IsAttacking = false;

    [Header("References")]
    [SerializeField] Transform m_WeaponPivot;
    [SerializeField] ColliderCallback m_CollidCallback;
    [SerializeField] Rigidbody m_Rb;

    //[Header("Input")]
    //[Header("Output")]
    public Action OnAttack;

    private void Start()
    {
        m_CollidCallback.OnTriggerEnterCallback += OnWeaponTouchSomething;
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack)
        {
            SetActiveLookAt(false);

            StartCoroutine(AttackCooldown());
            m_WeaponPivot.localRotation = Quaternion.identity;

            m_WeaponPivot.gameObject.SetActive(true);
            m_WeaponPivot.DOLocalRotate(
                new Vector3(0, -20, 0),
                m_AttackBeginDelay,
                RotateMode.FastBeyond360
            );

            yield return new WaitForSeconds(m_AttackBeginDelay);

            OnAttack?.Invoke();
            m_IsAttacking = true;

            m_Rb.AddForce(GetLookAtDirection() * attackDashForce, dashForceMode);

            float rot = -20f;
            DOTween.To(() => rot, x => rot = x, 200f, 0.1f)
                .SetEase(Ease.Linear)
                .OnUpdate(() => { m_WeaponPivot.localRotation = Quaternion.Euler(0, rot, 0); });

            yield return new WaitForSeconds(0.1f);

            m_IsAttacking = false;

            yield return new WaitForSeconds(m_AttackFinishedDelay);

            m_WeaponPivot.gameObject.SetActive(false);
            SetActiveLookAt(true);
        }
    }

    IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        if (m_AttackCooldown < m_AttackBeginDelay + m_AttackFinishedDelay + 0.1f)
        {
            m_AttackCooldown = m_AttackBeginDelay + m_AttackFinishedDelay + 0.2f;
            Debug.LogWarning("Attack cooldown too small, adjusted to fit attack animation.");
        }

        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    void OnWeaponTouchSomething(Collider collid)
    {
        if (!m_IsAttacking) return;

        if (collid.TryGetComponent(out EntityController controller)) controller.GetHealth().TakeDamage(m_Damage);
    }
}