using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private int m_PoolKey;

    public void Initialize(int poolKey)
    {
        m_PoolKey = poolKey;
    }

    public void Release()
    {
        PoolManager.Instance.ReturnToPool(this.gameObject, m_PoolKey);
    }
}