using MVsToolkit.Dev;
using UnityEngine;

public class PowerUpPickUp : MonoBehaviour
{
    [SerializeField, Inline] PowerUp m_PowerUp;
    [SerializeField] RSO_CurrentPowerUp m_PlayerPowerUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_PlayerPowerUp.Set(m_PowerUp);
        }
    }
}