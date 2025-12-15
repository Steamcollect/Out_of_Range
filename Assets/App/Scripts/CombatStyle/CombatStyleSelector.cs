using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatStyleSelector : MonoBehaviour
{
    [SerializeField] private PlayerCombat m_PrimaryPlayerCombat;
    
    [SerializeField] private CombatStyle m_DefaultCombatStyle;
    [SerializeField] private CombatStyle m_ShotgunCombatStyle;
    [SerializeField] private CombatStyle m_RifleCombatStyle;
    [SerializeField] private CombatStyle m_GrenadeLauncherCombatStyle;

    [SerializeField] private RSE_OnGrenadeLauncherPickedUp m_OnGrenadeLauncherPickedUp;
    [SerializeField] private RSE_OnShotgunPickedUp m_OnShotgunPickedUp;
    [SerializeField] private RSE_OnRiflePickedUp m_OnRiflePickedUp;
    
    private void OnEnable()
    {
        m_OnGrenadeLauncherPickedUp.Action += EnableGrenadeLauncher;
        m_OnShotgunPickedUp.Action += EnableShotgun;
        m_OnRiflePickedUp.Action += EnableRifle;
    }
    
    private void OnDisable()
    {
        m_OnGrenadeLauncherPickedUp.Action -= EnableGrenadeLauncher;
        m_OnShotgunPickedUp.Action -= EnableShotgun;
        m_OnRiflePickedUp.Action -= EnableRifle;
    }
    
    private void Start()
    {
        SetPrimaryCombatStyle(m_DefaultCombatStyle);
        SetSecondaryCombatStyle(null);
    }
    
    private void SetPrimaryCombatStyle(CombatStyle style)
    {
        m_PrimaryPlayerCombat.SetPrimaryCombatStyle(style);
    }

    private void SetSecondaryCombatStyle(CombatStyle style)
    {
        m_PrimaryPlayerCombat.SetSecondaryCombatStyle(style);
    }
    
    private void EnableGrenadeLauncher()
    {
        SetSecondaryCombatStyle(m_GrenadeLauncherCombatStyle);
    }
    
    private void EnableShotgun()
    {
        SetPrimaryCombatStyle(m_ShotgunCombatStyle);
    }
    
    private void EnableRifle()
    {
        SetPrimaryCombatStyle(m_DefaultCombatStyle);
    }
    
}
