using UnityEngine;

[DefaultExecutionOrder(-10)]
public abstract class PersistantSingleton<T> : MonoBehaviour where T : PersistantSingleton<T>
{
    private static T s_Instance;

    public static T Instance => s_Instance;
        
    protected virtual void Awake()
    {
        if (!s_Instance && s_Instance != this)
        { 
            s_Instance = (T)this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
    
    public static bool HasInstance() => s_Instance;
}