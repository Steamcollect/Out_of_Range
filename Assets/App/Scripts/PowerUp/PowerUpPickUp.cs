using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;


public class PowerUpPickUp : MonoBehaviour
{
    [SerializeField] SSO_PowerUp m_PowerUp;
    [SerializeField] RSO_CurrentPowerUp m_PlayerPowerUp;
    [SerializeField] private bool m_DestroyAfterPickup = false;
    public UnityEvent onWeaponPickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!m_PowerUp) Debug.LogWarning($"{gameObject.name} : No PowerUp Data assigned!");
            m_PlayerPowerUp.Value.Add(m_PowerUp);
            m_PlayerPowerUp.Set(m_PlayerPowerUp.Value);
            onWeaponPickedUp?.Invoke();

            if (m_DestroyAfterPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}