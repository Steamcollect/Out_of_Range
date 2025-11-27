using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : EntityCombat
{
    [Header("Internal Input")]
    [SerializeField] InputActionReference attackIA;

    private void Start()
    {
        attackIA.action.Enable();
    }

    private void Update()
    {
        if (attackIA.action.IsPressed())
            Attack();
    }

    public override void Attack()
    {
        currentCombatStyle.Attack();
    }
}
