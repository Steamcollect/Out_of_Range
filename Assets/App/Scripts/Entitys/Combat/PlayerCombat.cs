using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : EntityCombat
{
    [Header("Internal Input")]
    [SerializeField] InputActionReference attackIA;
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;
    
    private void Start()
    {
        attackIA.action.Enable();
    }

    private void Update()
    {
        Vector3 targetPosition = m_AimTarget.Get().position;
        targetPosition.y = verticalPivot.position.y;
        
        LookAt(targetPosition);
        if (attackIA.action.IsPressed())
            Attack();
    }

    public override void Attack()
    {
        currentCombatStyle.Attack();
    }
}
