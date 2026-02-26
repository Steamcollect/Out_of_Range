using MoreMountains.Tools;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public abstract class RegularSingleton<T> : MonoBehaviour where T : RegularSingleton<T>
{
        
        [RuntimeInitializeOnLoadMethod]
        private static void ResetInstance() => s_Instance = null;
        
        private static T s_Instance;

        public static T Instance => s_Instance;
        
        protected virtual void Awake()
        {
                if (!HasInstance())
                { 
                        s_Instance = (T)this;
                }
                else
                {
                        Destroy(gameObject);
                }
        }
        
        public static bool HasInstance() => s_Instance;
}