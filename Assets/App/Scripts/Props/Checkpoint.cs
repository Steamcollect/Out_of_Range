using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool applySpawnPosOnStart;

    [Header("References")]
    [SerializeField] Transform spawnPoint;

    //[Header("Input")]

    private void Awake()
    {
        if (applySpawnPosOnStart && PlayerSpawnPoint.position == Vector3.zero) PlayerSpawnPoint.position = spawnPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerSpawnPoint.position = spawnPoint.position;
            if (other.TryGetComponent(out EntityTrigger trigger))
            {
                trigger.GetController().GetHealth().TakeHealth(trigger.GetController().GetHealth().GetMaxHealth());
            }
        }
    }
}