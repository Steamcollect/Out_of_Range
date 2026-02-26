using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public sealed class PoolManager: RegularSingleton<PoolManager>
{
    [Header("PREWARM")]
    [SerializeField] private List<PoolDefinition> m_PoolsToPrewarm = new();

    private readonly Dictionary<int, ObjectPool<GameObject>> m_Pools = new(capacity:8);
    private readonly Dictionary<int, Transform> m_PoolParents = new(capacity:8);

    [System.Serializable]
    public class PoolDefinition
    {
        public string Name = "New PoolableObject";
        public GameObject Prefab;
        [Min(1)] public int InitialAmount = 20;
    }
    

    private void Start()
    {
        foreach(PoolDefinition def in m_PoolsToPrewarm)
        {
            if (!def.Prefab) continue;

            Prewarm(def.Prefab, def.InitialAmount);
        }
    }

    public void Prewarm(GameObject prefab, int count)
    {
        int prefabID = GetPrefabID(prefab);

        InitPool(prefab, prefabID);

        var tempObjects = new List<GameObject>(count);

        for(int i = 0; i < count; i++)
        {
            tempObjects.Add(m_Pools[prefabID].Get());
        }

        foreach(GameObject obj in tempObjects)
        {
            m_Pools[prefabID].Release(obj);
        }
    }
        
    private int GetPrefabID(GameObject prefab)
    {
        return prefab.GetHashCode();
    }
    
    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
    {
        int prefabID = GetPrefabID(prefab.gameObject);

        InitPool(prefab.gameObject, prefabID);

        GameObject spawnedObject = m_Pools[prefabID].Get();

        if (parent)
        {
            spawnedObject.transform.parent = parent;
        }

        PooledObject returnHandler = spawnedObject.GetComponent<PooledObject>();
        if (!returnHandler) returnHandler = spawnedObject.AddComponent<PooledObject>();
        returnHandler.Initialize(prefabID);

        spawnedObject.transform.SetPositionAndRotation(position, rotation);

        spawnedObject.SetActive(true);

        return spawnedObject.GetComponent<T>();
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        int prefabID = GetPrefabID(prefab.gameObject);

        InitPool(prefab.gameObject, prefabID);

        GameObject spawnedObject = m_Pools[prefabID].Get();

        if (parent)
        {
            spawnedObject.transform.parent = parent;
        }

        PooledObject returnHandler = spawnedObject.GetComponent<PooledObject>();
        if (!returnHandler) returnHandler = spawnedObject.AddComponent<PooledObject>();
        returnHandler.Initialize(prefabID);

        spawnedObject.transform.SetPositionAndRotation(position, rotation);

        spawnedObject.SetActive(true);
        return spawnedObject;
    }

    public void ReturnToPool(GameObject prefab, int prefabID)
    {
        if(m_Pools.TryGetValue(prefabID, out ObjectPool<GameObject> pool))
        {
            pool.Release(prefab);
        }
        else
        {
            Debug.LogWarning($"Pool introuvable pour {prefab.name}, destruction classique.");
            Destroy(prefab);
        }
    }

    private void InitPool(GameObject prefab, int prefabID)
    {
        if(m_Pools.ContainsKey(prefabID)) return;

        GameObject poolParentObj = new($"Pool_{prefab.name}");
        poolParentObj.transform.SetParent(this.transform);
        m_PoolParents[prefabID] = poolParentObj.transform;

        m_Pools[prefabID] = new ObjectPool<GameObject>
        (
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab, m_PoolParents[prefabID]);
                obj.SetActive(false);
                return obj;
            },
            actionOnGet: _ => { },
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
                if (obj.transform.parent != m_PoolParents[prefabID])
                    obj.transform.SetParent(m_PoolParents[prefabID]);
            },

#if UNITY_EDITOR
            actionOnDestroy: DestroyImmediate,
#else
            actionOnDestroy: (obj) => Destroy(obj),
#endif
            defaultCapacity: 20,
            maxSize: 500
        );
    }
}