using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TechnicalSceneBoostrap
{
    private static AsyncOperation s_AsyncOperation;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        s_AsyncOperation = null;
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        s_AsyncOperation = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BindingInjectionCurrentScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        
        if (s_AsyncOperation != null)
        {
            s_AsyncOperation.completed += InjectCurrentScene;
        }
    }

    private static void InjectCurrentScene(AsyncOperation obj)
    {
        FieldInfo field = typeof(SceneLoader).GetField("m_CurrentScene",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (field == null || SceneLoader.Instance == null) return;
        field.SetValue(SceneLoader.Instance, SceneManager.GetActiveScene());
    }
}
