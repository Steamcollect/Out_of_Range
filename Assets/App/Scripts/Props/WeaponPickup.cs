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
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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

            onWeaponPickedUp.Invoke();

            if (m_DestroyAfterPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}
