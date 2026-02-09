using MVsToolkit.Dev;
using UnityEngine;
using UnityEngine.Events;


public class PowerUpPickUp : MonoBehaviour
{
    [SerializeField] SSO_PowerUp m_PowerUp;
    [SerializeField] RSE_AddPowerUp m_AddPowerUp;
    [SerializeField] private bool m_DestroyAfterPickup = false;
    [SerializeField] float m_HandleTime;
    public UnityEvent onWeaponPickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!m_PowerUp) Debug.LogWarning($"{gameObject.name} : No PowerUp Data assigned!");
            m_AddPowerUp.Call(m_PowerUp, m_HandleTime);
            onWeaponPickedUp?.Invoke();

            if (m_DestroyAfterPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}