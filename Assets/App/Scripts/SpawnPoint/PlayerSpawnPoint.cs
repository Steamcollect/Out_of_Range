using UnityEngine;

public static class PlayerSpawnPoint
{
    public static Vector3 S_Position;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Initialize()
    {
        S_Position = Vector3.zero;
    }
}