using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private int m_PoolKey;
    private bool m_Released = false;

    public void Initialize(int poolKey)
    {
        m_PoolKey = poolKey;
        m_Released = false;
    }

    public void Release()
    {
        if (m_Released) return;
        m_Released = true;
        PoolManager.Instance.ReturnToPool(this.gameObject, m_PoolKey);
    }
}