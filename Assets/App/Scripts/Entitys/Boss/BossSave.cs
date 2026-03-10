using UnityEngine;

public static class BossSave
{
    public static bool IsFirstTimeSeeingBoss = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Initialize()
    {
        IsFirstTimeSeeingBoss = true;
    }
}