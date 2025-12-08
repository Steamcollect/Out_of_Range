using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerCombat : EntityCombat
{
    [FormerlySerializedAs("attackIA")]
    [Header("Internal Input")]
    [SerializeField] private InputActionReference m_AttackIa;

    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;

    private void Start()
    {
        m_AttackIa.action.Enable();
    }

    private void Update()
    {
        Vector3 targetPosition = m_AimTarget.Get().position;
        targetPosition.y = m_VerticalPivot.position.y;

        LookAt(targetPosition);
        if (m_AttackIa.action.IsPressed())
            Attack();
    }

    public override void Attack()
    {
        m_CurrentCombatStyle.Attack();
    }
}