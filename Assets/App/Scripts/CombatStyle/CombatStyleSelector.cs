using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatStyleSelector : MonoBehaviour
{
    public List<CombatStyle> combatStyles;
    private CombatStyle currentStyle;
    public PlayerCombat playerCombat;
    public InputActionReference inputActionReference_1, inputActionReference_2, inputActionReference_3;
    
    private InputAction inputAction1, inputAction2, inputAction3;
    private void OnEnable()
    {
        inputAction1 = inputActionReference_1.action;
        inputAction2 = inputActionReference_2.action;
        inputAction3 = inputActionReference_3.action;
        
        inputAction1.Enable();
        inputAction2.Enable();
        inputAction3.Enable();
        
        inputAction1.started += ctx => SelectStyle(0);
        inputAction2.started += ctx => SelectStyle(1);
        inputAction3.started += ctx => SelectStyle(2);
    }
    
    private void OnDisable()
    {
        inputAction1.started -= ctx => SelectStyle(0);
        inputAction2.started -= ctx => SelectStyle(1);
        inputAction3.started -= ctx => SelectStyle(2);
    }
    
    void SelectStyle(int index)
    {
        Debug.Log("Selecting combat style index: " + index);
        if (index < 0 || index >= combatStyles.Count) return;

        playerCombat.SetCombatStyle(combatStyles[index]);
    }
}
