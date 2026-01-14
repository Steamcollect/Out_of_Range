using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int m_AmmoGiven;
    
    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            PlayerCombat combat = player.GetCombat() as PlayerCombat;

            if(combat.GetPrimaryCombatStyle().TryGetComponent(out IAmmoCombatStyle ammo))
            {
                ammo.AddAmmo(m_AmmoGiven);
            }
            
            if(combat.GetSecondaryCombatStyle().TryGetComponent(out IAmmoCombatStyle _ammo))
            {
                _ammo.AddAmmo(m_AmmoGiven);
            }

            Destroy(gameObject);
        }
    }
}