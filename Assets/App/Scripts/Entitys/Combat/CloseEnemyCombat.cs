using System.Collections;
using UnityEngine;

public class CloseEnemyCombat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int m_Damage;
    [SerializeField] float m_AttackCooldown;

    [Space(5)]
    [SerializeField] float m_AttackBeginDelay = .2f;
    [SerializeField] float m_AttackFinishedDelay = .2f;

    bool m_CanAttack = true;
    bool m_IsAttacking = false;

    [Header("References")]
    [SerializeField] Transform m_WeaponPivot;
    [SerializeField] ColliderCallback m_CollidCallback;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_CollidCallback.OnTriggerEnterCallback += OnWeaponTouchSomething;
    }

    void Attack()
    {

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