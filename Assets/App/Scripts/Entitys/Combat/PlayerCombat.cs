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
        LookAt(m_AimTarget.Get().position);
        if (attackIA.action.IsPressed())
            Attack();
    }

    public override void Attack()
    {
        currentCombatStyle.Attack();
    }
}
