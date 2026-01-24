using MVsToolkit.Dev;
using UnityEngine;


public class PowerUpPickUp : MonoBehaviour
{
    [SerializeField] SSO_PowerUp m_PowerUp;
    [SerializeField] RSO_CurrentPowerUp m_PlayerPowerUp;
    [SerializeField] private bool m_DestroyAfterPickup = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!m_PowerUp) Debug.LogWarning($"{gameObject.name} : No PowerUp Data assigned!");
            m_PlayerPowerUp.Value.Add(m_PowerUp);
            m_PlayerPowerUp.Set(m_PlayerPowerUp.Value);

            if (m_DestroyAfterPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}