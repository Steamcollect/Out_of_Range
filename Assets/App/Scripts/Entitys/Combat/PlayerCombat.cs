using System.Collections;
using UnityEngine;

public class PlayerCombat : EntityCombat
{
    [Header("Internal References")]
    [SerializeField] CombatStyle m_CurrentCombatStyle;

    [SerializeField] private InputPlayerController m_InputPlayerController;
    [Space(10)]
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;
    
    private void Update()
    {
        LookAt(m_AimTarget.Get().position, LookAtAxis.Horizontal);
        
        if (m_InputPlayerController.IsAttackPressed())
        {
            StartCoroutine(m_CurrentCombatStyle.Attack());
        }
    }

    public CombatStyle GetCurrentCombatStyle()
    {
        return m_CurrentCombatStyle;
    }
    
    public void SetCombatStyle(CombatStyle newStyle)
    {
        m_CurrentCombatStyle = newStyle;
    }
}