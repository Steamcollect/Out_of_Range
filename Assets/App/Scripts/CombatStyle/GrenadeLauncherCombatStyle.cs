using System.Collections;
using UnityEngine;

public class GrenadeLauncherCombatStyle : CombatStyle
{
    [Header("Combat Settings")]
    [SerializeField] Grenade m_GrenadePrefab;
    [SerializeField] Transform m_AttackPoint;

    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;

    public override IEnumerator Attack()
    {
        Grenade grenade = Instantiate(m_GrenadePrefab, m_AttackPoint.position, m_AttackPoint.rotation);
        grenade.Setup(m_AttackPoint.position, m_AimTarget.Get().position);

        grenade.Move();

        yield break;
    }
}