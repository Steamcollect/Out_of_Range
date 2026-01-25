using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
public static class TechnicalSceneBoostrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }
}
#endif
