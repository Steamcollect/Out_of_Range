using System.Collections;
using DG.Tweening;
using MVsToolkit.Utils;
using UnityEngine;

public class CloseCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] private int m_Damage;

    [SerializeField] private float m_AttackCooldown;
    [SerializeField] private float m_AttackBeginDelay = .2f;

    [SerializeField] private float m_AttackFinishedDelay = .2f;
    //STOPPER LES ENNEMIS QUAND ILS ATTAQUENT

    [Header("References")]
    [SerializeField] private Transform m_WeaponPivot;

    [SerializeField] private ColliderCallback m_Callback;
    [SerializeField] private EntityCombat m_CombatHandler;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_Callback.OnTriggerEnterCallback += OnWeaponTouchSomething;
    }

    public override void Attack()
    {
        if (m_CanAttack)
        {
            m_CombatHandler.SetActiveLookAt(false);

            StartCoroutine(AttackCooldown());
            m_WeaponPivot.localRotation = Quaternion.identity;

            m_WeaponPivot.gameObject.SetActive(true);
            m_WeaponPivot.DOLocalRotate(
                new Vector3(0, -20, 0),
                m_AttackBeginDelay,
                RotateMode.FastBeyond360
            ).OnComplete(() =>
            {
                OnAttack?.Invoke();
                m_IsAttacking = true;

                float rot = -20f;
                DOTween.To(() => rot, x => rot = x, 200f, 0.1f)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => { m_WeaponPivot.localRotation = Quaternion.Euler(0, rot, 0); })
                    .OnComplete(() =>
                    {
                        m_IsAttacking = false;
                        this.Delay(() =>
                        {
                            m_WeaponPivot.gameObject.SetActive(false);
                            m_CombatHandler.SetActiveLookAt(true);
                        }, m_AttackFinishedDelay);
                    });
            });
        }
    }

    private void OnWeaponTouchSomething(Collider collid)
    {
        if (!m_IsAttacking) return;

        if (collid.TryGetComponent(out EntityController controller)) controller.GetHealth().TakeDamage(m_Damage);
    }

    private IEnumerator AttackCooldown()
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
}