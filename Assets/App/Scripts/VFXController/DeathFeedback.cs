using UnityEngine;
using UnityEngine.VFX;

public class DeathFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EntityHealth m_EntityHealth;
    [SerializeField] private VisualEffect m_DeathEffect;
    
    private void OnEnable() => m_EntityHealth.OnDeath += PlayDeathEffect;

    private void PlayDeathEffect() => Instantiate(m_DeathEffect, transform.position, Quaternion.identity).Play();

    private void OnDisable() => m_EntityHealth.OnDeath -= PlayDeathEffect;
    
}
