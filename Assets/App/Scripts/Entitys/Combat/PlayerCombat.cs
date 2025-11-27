using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : EntityCombat
{
    [Header("Internal References")]
    [SerializeField] CombatStyle currentCombatStyle;

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

    void Attack()
    {
        currentCombatStyle.Attack();
    }
}
