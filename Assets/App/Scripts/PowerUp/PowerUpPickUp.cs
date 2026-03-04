using FMODUnity;
using MVsToolkit.Dev;
using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;


public class PowerUpPickUp : MonoBehaviour
{
    [SerializeField] SSO_PowerUp m_PowerUp;
    [SerializeField] RSE_AddPowerUp m_AddPowerUp;
    [SerializeField] private GameObject m_Visual;
    [SerializeField] private bool m_DestroyAfterPickup = false;
    [SerializeField] float m_HandleTime;
    public UnityEvent onWeaponPickedUp;
    [SerializeField] EventReference m_PickUpFx;

    private bool m_PickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if(m_PickedUp) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (!m_PowerUp) Debug.LogWarning($"{gameObject.name} : No PowerUp Data assigned!");
            m_AddPowerUp.Call(m_PowerUp, m_HandleTime);
            onWeaponPickedUp?.Invoke();
            RuntimeManager.PlayOneShot(m_PickUpFx);

            m_PickedUp = true;

            if (m_DestroyAfterPickup)
            {
                m_Visual.SetActive(false);
                this.Delay(() => { Destroy(gameObject); }, 2.0f);
            }
        }
    }
}