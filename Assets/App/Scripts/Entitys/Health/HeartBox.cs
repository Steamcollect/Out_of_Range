using MVsToolkit.Dev;
using UnityEngine;

public class HeartBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_DamageMultiplier;

    [Header("References")]
    [SerializeField] InterfaceReference<IHealth>[] m_HealthsConnected;

    //[Header("Input")]
    //[Header("Output")]

    public void TakeDamage(int damage)
    {
        if (m_HealthsConnected.Length <= 0) return;

        foreach (IHealth health in m_HealthsConnected)
        {
            health.TakeDamage(Mathf.RoundToInt(damage * m_DamageMultiplier));
        }
    }
}