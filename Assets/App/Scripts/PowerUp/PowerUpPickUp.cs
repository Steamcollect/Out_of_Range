using MVsToolkit.Dev;
using UnityEngine;

public class PowerUpPickUp : MonoBehaviour
{
    [SerializeField, Inline] PowerUp m_PowerUp;
    [SerializeField] RSO_CurrentPowerUp m_PlayerPowerUp;
    [SerializeField] private bool m_DestroyAfterPickup = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_PlayerPowerUp.Set(m_PowerUp);

            if (m_DestroyAfterPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}