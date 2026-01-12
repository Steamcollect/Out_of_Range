using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeLauncherCombatStyle : CombatStyle
{
    [Header("Combat Settings")]
    [SerializeField] Grenade m_GrenadePrefab;
    [SerializeField] Transform m_AttackPoint;

    [Space(10)]
    [SerializeField] RSO_PlayerAimTarget m_AimTarget;

    [SerializeField] LayerMask m_UnpassingWallMask;

    public override void AttackStart(InputAction.CallbackContext ctx)
    {
        
    }

    public override void AttackEnd(InputAction.CallbackContext ctx)
    {
        StartCoroutine(Attack());
    }

    public override IEnumerator Attack()
    {
        Vector3 s = m_AttackPoint.position;
        Vector3 e = m_AimTarget.Get().position;

        Debug.DrawRay(s, (e - s).normalized * Vector3.Distance(e, s), Color.blue, .5f);
        if (Physics.Raycast(s, e, out RaycastHit hit, Vector3.Distance(e, s), m_UnpassingWallMask))
        {
            yield break;
        }

        Grenade grenade = Instantiate(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        grenade.Setup(m_AttackPoint.position, m_AimTarget.Get().position);

        grenade.Move();

        yield break;
    }
}