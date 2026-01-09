using UnityEngine;
using UnityEngine.Serialization;

public class Checkpoint : MonoBehaviour
{
    [Header("Outputs")]
    [SerializeField] private RSE_OnCheckpointRegistered m_OnCheckpointRegistered;
    
    [Header("Settings")]
    [SerializeField] private bool m_ApplySpawnPosOnStart;

    [Header("References")]
    [SerializeField] private Transform m_SpawnPoint;

    //[Header("Input")]

    private void Awake()
    {
        if (m_ApplySpawnPosOnStart && PlayerSpawnPoint.S_Position == Vector3.zero)
            PlayerSpawnPoint.S_Position = m_SpawnPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PlayerSpawnPoint.S_Position != m_SpawnPoint.position)
            {
                m_OnCheckpointRegistered.Call();
                PlayerSpawnPoint.S_Position = m_SpawnPoint.position;
            }
            
            if (other.TryGetComponent(out EntityController controller))
               controller.GetHealth().TakeHealth(controller.GetHealth().GetMaxHealth());
        }
    }
}