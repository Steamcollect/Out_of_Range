using UnityEngine;

public static class PlayerSpawnPoint
{
    public static Vector3 S_Position;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Chepa()
    {
        S_Position = Vector3.zero;
    }
}