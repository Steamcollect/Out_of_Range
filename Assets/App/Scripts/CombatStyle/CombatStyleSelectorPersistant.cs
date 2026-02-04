using UnityEngine;

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
