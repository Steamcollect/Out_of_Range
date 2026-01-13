using UnityEngine;
using UnityEngine.Serialization;

public class Checkpoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool m_ApplySpawnPosOnStart;

    [Header("References")]
    [SerializeField] private Transform m_SpawnPoint;
    [SerializeField] RSO_PlayerController m_Player;

    //[Header("Input")]
    
    public Vector3 Position => m_SpawnPoint.position;

    private void Awake()
    {
        if (m_ApplySpawnPosOnStart && PlayerSpawnPoint.S_Position == Vector3.zero)
            PlayerSpawnPoint.S_Position = m_SpawnPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerSpawnPoint.S_Position = m_SpawnPoint.position;
            if (other.TryGetComponent(out EntityController controller))
               controller.GetHealth().Heal(controller.GetHealth().GetMaxHealth());
        }
    }

    public void TpPlayer()
    {
        m_Player.Get().Teleport(m_SpawnPoint.position);
    }
}