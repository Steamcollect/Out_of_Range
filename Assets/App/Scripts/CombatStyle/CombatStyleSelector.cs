using UnityEngine;

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
        
        if (CombatStyleSelectorPersistant.HasLaunchGrenade())
        {
            SetSecondaryCombatStyle(m_GrenadeLauncherCombatStyle);
        }
        else
        {
            SetSecondaryCombatStyle(null);
        }
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


public static class CombatStyleSelectorPersistant
{
    private static bool s_HasLaunchGrenade;
    private static bool s_HasShotgun;
    private static bool s_HasRifle;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Initialize()
    {
        s_HasLaunchGrenade = false;
        s_HasShotgun = false;
        s_HasRifle = false;
    }
    
    public static void SetHasLaunchGrenade()
    {
        s_HasLaunchGrenade = true;
    }
    
    public static void SetHasShotgun()
    {
        s_HasShotgun = true;
        s_HasRifle = false;
    }
    
    
    public static void SetHasRifle()
    {
        s_HasRifle = true;
        s_HasShotgun = false;
    }
    
    public static bool HasLaunchGrenade()
    {
        return s_HasLaunchGrenade;
    }
    public static bool HasShotgun()
    {
        return s_HasShotgun;
    }

    public static bool HasRifle()
    {
        return s_HasRifle;
    }
}