using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Outputs")]
    [SerializeField] RSE_OnCheckpointRegistered m_OnCheckpointRegistered;
    [SerializeField] RSE_OnRiflePickedUp m_RifleEvent;
    [SerializeField] RSE_OnGrenadeLauncherPickedUp m_GrenadeEvent;

    [Header("Settings")]
    [SerializeField] private bool m_ApplySpawnPosOnStart;
    bool m_IsActivated = false;

    [Header("References")]
    [SerializeField] private Transform m_SpawnPoint;
    [SerializeField] RSO_PlayerController m_Player;

    [Space(10)]
    [SerializeField] GameObject[] roomConnected;
    [SerializeField] RSE_SetActiveRooms m_SetActiveRooms;

    //[Header("Input")]
    
    public Vector3 Position => m_SpawnPoint.position;

    private void Awake()
    {
        if (m_ApplySpawnPosOnStart && PlayerSpawnPoint.S_Position == Vector3.zero)
        {
            PlayerSpawnPoint.S_Position = m_SpawnPoint.position;
        }
    }

    private void Start()
    {
        if (m_ApplySpawnPosOnStart)
            m_SetActiveRooms.Call(roomConnected);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !m_IsActivated)
        {
            m_IsActivated = true;
            m_SetActiveRooms.Call(roomConnected);

            if (PlayerSpawnPoint.S_Position != m_SpawnPoint.position)
            {
                m_OnCheckpointRegistered.Call();
                PlayerSpawnPoint.S_Position = m_SpawnPoint.position;
            }
            
            if (other.TryGetComponent(out EntityController controller))
               controller.GetHealth().Heal(controller.GetHealth().GetMaxHealth());
        }
    }

    public void TpPlayer()
    {
        m_SetActiveRooms.Call(roomConnected);
        
        m_RifleEvent.Call();
        m_GrenadeEvent.Call();

        m_Player.Get().Teleport(m_SpawnPoint.position);
    }
}