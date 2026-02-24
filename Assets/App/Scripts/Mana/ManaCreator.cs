using Sirenix.OdinInspector;
using UnityEngine;

public class ManaCreator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2Int m_ManaCount;

    [SerializeField] float m_PropulsionAngle;

    [SerializeField] bool m_RequireShotgunToSpawn = true;

    [Header("References")]
    [SerializeField] ManaPickup m_ManaPrefab;
    [SerializeField] RSO_CanPickupMana m_CanPickupMana;

    //[Header("Input")]
    //[Header("Output")]

    /// <summary>
    /// Spawn mana pick up
    /// </summary>
    [Button]
    public void Create()
    {
        if (m_RequireShotgunToSpawn && !m_CanPickupMana.Get()) return;

        for (int i = 0; i < Random.Range(m_ManaCount.x, m_ManaCount.y); i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(dir.x, m_PropulsionAngle, dir.y);

            ManaPickup mana = PoolManager.Instance.Spawn(m_ManaPrefab, transform.position + direction, Quaternion.identity);

            mana.AddForce(direction).Setup();
        }
    }
}