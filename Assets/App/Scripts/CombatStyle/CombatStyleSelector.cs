using UnityEngine;

public class CombatStyleSelector : MonoBehaviour
{
    [SerializeField] private PlayerCombat m_PrimaryPlayerCombat;
    [SerializeField] PlayerMana m_Mana;

    [SerializeField] private CombatStyle m_DefaultCombatStyle;
    [SerializeField] private CombatStyle m_GrenadeLauncherCombatStyle;

    [Space]
    [SerializeField] PlayerArms m_Arms;

    [Space(10)]
    [SerializeField] RSO_CanPickupMana m_CanPickupMana;

    [Space(5)]
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
        HandleSaveCombat();
    }

    private void HandleSaveCombat()
    {
        SetPrimaryCombatStyle(m_DefaultCombatStyle);
        SetSecondaryCombatStyle(CombatStyleSelectorPersistant.HasLaunchGrenade() ? m_GrenadeLauncherCombatStyle : null);

        //m_Arms.SetActive(((RangeOverloadCombatStyle)m_DefaultCombatStyle).GetCanShoot());
        //m_FlyingFX.SetActive(!((RangeOverloadCombatStyle)m_DefaultCombatStyle).GetCanShoot());
    }
    
    private void SetPrimaryCombatStyle(CombatStyle style)
    {
        m_PrimaryPlayerCombat.SetPrimaryCombatStyle(style);
    }

    private void SetSecondaryCombatStyle(CombatStyle style)
    {
        m_CanPickupMana.Set(style != null);
        m_PrimaryPlayerCombat.SetSecondaryCombatStyle(style);
    }
    
    private void EnableGrenadeLauncher()
    {
        CombatStyleSelectorPersistant.SetHasLaunchGrenade();
        m_CanPickupMana.Set(true);
        m_Mana.SetToMax();
        SetSecondaryCombatStyle(m_GrenadeLauncherCombatStyle);
        m_Arms.ShowGrenade();
    }

    private void EnableShotgun()
    {
        //CombatStyleSelectorPersistant.SetHasShotgun();
        //SetPrimaryCombatStyle(m_ShotgunCombatStyle);
    }
    
    private void EnableRifle()
    {
        ((RangeOverloadCombatStyle)m_DefaultCombatStyle).SetCanShoot(true);
        m_Arms.ShowRifle();
    }    
}
