using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private int m_PoolKey;
    private bool m_Initialized = false;

    public void Initialize(int poolKey)
    {
        m_PoolKey = poolKey;
        m_Initialized = true;
    }

    public void Release()
    {
        if (!m_Initialized) return;
        m_Initialized = false;
        PoolManager.Instance.ReturnToPool(this.gameObject, m_PoolKey);
    }
}