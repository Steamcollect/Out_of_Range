using System.Collections;
using DG.Tweening;
using MVsToolkit.Utils;
using UnityEngine;

public class CloseCombatStyle : CombatStyle
{
    [Header("Settings")]
    [SerializeField] int damage;
    [SerializeField] float attackCooldown;
    [SerializeField] float attackBeginDelay = .2f;
    [SerializeField] float attackFinishedDelay = .2f;
    //STOPPER LES ENNEMIS QUAND ILS ATTAQUENT

    bool canAttack = true;

    [Header("References")]
    [SerializeField] Transform weaponPivot;
    [SerializeField] ColliderCallback callback;
    [SerializeField] EntityCombat combatHandler;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        callback._OnTriggerEnter += OnWeaponTouchSomething;
    }

    public override void Attack()
    {
        if (canAttack)
        {
            combatHandler.SetActiveLookAt(false);

            StartCoroutine(AttackCooldown());
            weaponPivot.localRotation = Quaternion.identity;

            weaponPivot.gameObject.SetActive(true);
            weaponPivot.DOLocalRotate(
                new Vector3(0, -20, 0),
                attackBeginDelay,
                RotateMode.FastBeyond360
            ).OnComplete(() =>
            {
                OnAttack?.Invoke();

                float rot = -20f;
                DOTween.To(() => rot, x => rot = x, 200f, 0.1f)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        weaponPivot.localRotation = Quaternion.Euler(0, rot, 0);
                    })
                    .OnComplete(() =>
                    {
                    CoroutineUtils.Delay(this, () =>
                    {
                        weaponPivot.gameObject.SetActive(false);
                        combatHandler.SetActiveLookAt(true);
                    }, attackFinishedDelay);
                });
            });
        }
    }

    void OnWeaponTouchSomething(Collider collid)
    {
        if (collid.TryGetComponent(out EntityTrigger trigger))
        {
            trigger.GetController().GetHealth().TakeDamage(damage);
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        if(attackCooldown < attackBeginDelay + attackFinishedDelay + 0.1f)
        {
            attackCooldown = attackBeginDelay + attackFinishedDelay + 0.2f;
            Debug.LogWarning("Attack cooldown too small, adjusted to fit attack animation.");
        }
            yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}