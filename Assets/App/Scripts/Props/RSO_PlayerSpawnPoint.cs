using UnityEngine;
using MVsToolkit.Wrappers;

public static class PlayerSpawnPoint
{
    public static Vector3 position;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Chepa()
    {
        position = Vector3.zero;
    }
}