using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private Dictionary<int, ObjectPool<GameObject>> m_Pools = new Dictionary<int, ObjectPool<GameObject>>();
    private Dictionary<int, Transform> m_PoolParents = new Dictionary<int, Transform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Prewarm(GameObject prefab, int count)
    {
        int prefabID = prefab.GetInstanceID();

        InitPool(prefab, prefabID);

        List<GameObject> tempObjects = new List<GameObject>(count);

        for(int i = 0; i < count; i++)
        {
            tempObjects.Add(m_Pools[prefabID].Get());
        }

        foreach(GameObject obj in tempObjects)
        {
            m_Pools[prefabID].Release(obj);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        int prefabID = prefab.GetInstanceID();

        InitPool(prefab, prefabID);

        GameObject spawnedObject = m_Pools[prefabID].Get();

        spawnedObject.transform.SetPositionAndRotation(position, rotation);

        if(parent != null)
        {
            spawnedObject.transform.parent = parent;
        }

        PooledObject returnHandler = spawnedObject.GetComponent<PooledObject>();
        if(returnHandler == null) returnHandler = spawnedObject.AddComponent<PooledObject>();
        returnHandler.Initialize(prefabID);

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

        GameObject poolParentObj = new GameObject($"Pool_{prefab.name}");

        poolParentObj.transform.SetParent(this.transform);
        m_PoolParents[prefabID] = poolParentObj.transform;

        m_Pools[prefabID] = new ObjectPool<GameObject>
        (
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab, m_PoolParents[prefabID]);
                return obj;
            },
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
                if (obj.transform.parent != m_PoolParents[prefabID])
                    obj.transform.SetParent(m_PoolParents[prefabID]);
            },
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 20,
            maxSize: 500
        );
    }
}