using UnityEngine;

public class CombatStyleSelector : MonoBehaviour
{
    [SerializeField] private PlayerCombat m_PrimaryPlayerCombat;
    [SerializeField] PlayerMana m_Mana;

    [SerializeField] private CombatStyle m_DefaultCombatStyle;
    [SerializeField] private CombatStyle m_ShotgunCombatStyle;
    [SerializeField] private CombatStyle m_RifleCombatStyle;
    [SerializeField] private CombatStyle m_GrenadeLauncherCombatStyle;

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
        if (CombatStyleSelectorPersistant.HasRifle() || CombatStyleSelectorPersistant.HasShotgun())
        {
            if (CombatStyleSelectorPersistant.HasRifle())
            {
                SetPrimaryCombatStyle(m_RifleCombatStyle);
            }
            else if (CombatStyleSelectorPersistant.HasShotgun())
            {
                SetPrimaryCombatStyle(m_ShotgunCombatStyle);
            }
        }
        else
        {
            SetPrimaryCombatStyle(m_DefaultCombatStyle);
        }

        SetSecondaryCombatStyle(CombatStyleSelectorPersistant.HasLaunchGrenade() ? m_GrenadeLauncherCombatStyle : null);
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
        CombatStyleSelectorPersistant.SetHasLaunchGrenade();
        m_CanPickupMana.Set(true);
        m_Mana.SetToMax();
        SetSecondaryCombatStyle(m_GrenadeLauncherCombatStyle);
    }
    
    private void EnableShotgun()
    {
        CombatStyleSelectorPersistant.SetHasShotgun();
        SetPrimaryCombatStyle(m_ShotgunCombatStyle);
    }
    
    private void EnableRifle()
    {
        CombatStyleSelectorPersistant.SetHasRifle();
        SetPrimaryCombatStyle(m_RifleCombatStyle);
    }
    
}
