using MVsToolkit.Dev;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_DamageMultiplier = 1;

    [Header("References")]
    [SerializeField] InterfaceReference<IHealth> m_HealthConnected;

    //[Header("Input")]
    //[Header("Output")]

    public void TakeDamage(int damage)
    {
        if (m_HealthConnected == null) return;

        m_HealthConnected.Value.TakeDamage(Mathf.RoundToInt(damage * m_DamageMultiplier));
    }
}