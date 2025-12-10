using UnityEditor;
using UnityEngine;

[SelectionBase]
public class Barricade : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_DistanceThreshold = 3f;

    [SerializeField] private LayerMask m_BulletLayer;

    [Header("References")]
    [SerializeField] private EntityHealth m_Health;

    [SerializeField] private Collider m_TriggerCollider;


    private void Start()
    {
        m_Health.OnDeath += () => Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (m_TriggerCollider == null) return;

        Bounds bounds = m_TriggerCollider.bounds;
        Vector3 center = bounds.center;
        float radius = bounds.extents.magnitude + m_DistanceThreshold;

        Handles.color = new Color(1, 1, 0, 0.2f);

        int samples = 128;
        float phi = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));

        for (int i = 0; i < samples; i++)
        {
            float y = 1 - i / (float)(samples - 1) * 2; // y goes from 1 to -1
            float radiusAtY = Mathf.Sqrt(1 - y * y);

            float theta = phi * i;

            float x = Mathf.Cos(theta) * radiusAtY;
            float z = Mathf.Sin(theta) * radiusAtY;

            Vector3 pointOnSphere = center + new Vector3(x, y, z) * radius;
            Vector3 closestPoint = m_TriggerCollider.ClosestPoint(pointOnSphere);
            Vector3 direction = (pointOnSphere - closestPoint).normalized;

            if (direction != Vector3.zero)
                Handles.DrawSolidDisc(closestPoint + direction * m_DistanceThreshold, Camera.current.transform.forward,
                    0.05f);
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_BulletLayer.Contains(other.gameObject.layer)) return;
        if (!other.TryGetComponent(out Bullet bullet)) return;

        if (bullet.GetShootPosition().Distance(m_TriggerCollider.ClosestPoint(bullet.GetShootPosition())) >
            m_DistanceThreshold)
            bullet.Impact(gameObject);
    }
}