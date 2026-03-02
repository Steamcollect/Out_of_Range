using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class InfiniteManaCollectibleToolPlacement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2 m_SpawnAreaSize = Vector3.one;

    [Space(10)]
    [SerializeField] Vector2Int m_MinMaxCount = Vector2Int.one;
    [SerializeField] Vector2 m_MinMaxSize;

    [Header("References")]
    [SerializeField] InfiniteManaCollectible m_Collectible;

    //[Header("Input")]
    //[Header("Output")]

    /// <summary>
    /// Create Infinite mana collectible inside the size and with random count base on values given
    /// </summary>
    [Button]
    public void Create()
    {
#if UNITY_EDITOR
        // 1. Clear existing collectibles
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (child.TryGetComponent<InfiniteManaCollectible>(out _))
                DestroyImmediate(child.gameObject);
        }

        // 2. Determine random count
        int count = Random.Range(m_MinMaxCount.x, m_MinMaxCount.y + 1);

        // 3. Spawn collectibles
        for (int i = 0; i < count; i++)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(m_Collectible.gameObject, transform) as GameObject;
            InfiniteManaCollectible newCol = go.GetComponent<InfiniteManaCollectible>();

            // Random local position inside spawn area
            Vector2 half = m_SpawnAreaSize * 0.5f;
            Vector3 localPos = new Vector3(
                Random.Range(-half.x, half.x),
                0f,
                Random.Range(-half.y, half.y)
            );

            // Convert to world position respecting parent rotation
            Vector3 worldPos = transform.TransformPoint(localPos);

            // Force Buttom.y to match parent Y
            float bottomOffset = newCol.Buttom.position.y - newCol.transform.position.y;
            worldPos.y = transform.position.y - bottomOffset;

            newCol.transform.position = worldPos;

            // Random rotation (only Y)
            newCol.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            // Random scale
            float scale = Random.Range(m_MinMaxSize.x, m_MinMaxSize.y);
            newCol.transform.localScale = Vector3.one * scale;
        }
#endif
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 size = new Vector3(m_SpawnAreaSize.x, 0f, m_SpawnAreaSize.y);

        // Dessine un carré orienté
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, size);

        Gizmos.matrix = oldMatrix;
    }
}