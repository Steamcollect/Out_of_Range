using MVsToolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class WeaponPickup : MonoBehaviour
{
    enum WeaponType
    {
        Rifle,
        Shotgun,
        GrenadeLauncher
    }
    
    [SerializeField] private WeaponType m_WeaponType;
    [SerializeField] private bool m_DestroyAfterPickup = false;
    [SerializeField] private RSE_OnRiflePickedUp m_OnRiflePickedUp;
    [SerializeField] private RSE_OnShotgunPickedUp m_OnShotgunPickedUp;
    [SerializeField] private RSE_OnGrenadeLauncherPickedUp m_OnGrenadeLauncherPickedUp;

    public UnityEvent onWeaponPickedUp;

    bool m_PickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (m_PickedUp) return;

        if (other.CompareTag("Player"))
        {
            m_PickedUp = true;

            switch (m_WeaponType)
            {
                case WeaponType.Rifle:
                    m_OnRiflePickedUp.Call();
                    break;
                case WeaponType.Shotgun:
                    m_OnShotgunPickedUp.Call();
                    break;
                case WeaponType.GrenadeLauncher:
                    m_OnGrenadeLauncherPickedUp.Call();
                    break;
            }

            onWeaponPickedUp?.Invoke();

            if (m_DestroyAfterPickup)
            {
                this.Delay(() => { Destroy(gameObject); }, 2.0f);
            }
        }
    }
}
