using Sirenix.OdinInspector;
using UnityEngine;

public class ManaCreator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector2Int m_ManaCount;

    [SerializeField] float m_PropulsionForce;
    [SerializeField] float m_PropulsionAngle;

    [Header("References")]
    [SerializeField] ManaPickup m_ManaPrefab;

    //[Header("Input")]
    //[Header("Output")]

    [Button]
    public void Create()
    {
        for (int i = 0; i < Random.Range(m_ManaCount.x, m_ManaCount.y); i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(dir.x, m_PropulsionAngle, dir.y);

            ManaPickup mana = PoolManager.Instance.Spawn(m_ManaPrefab, transform.position + direction, Quaternion.identity);

            mana.StartCoroutine(mana.SpawningCooldown());
            mana.AddForce(direction, ForceMode.Impulse);
            mana.Setup();
        }
    }
}