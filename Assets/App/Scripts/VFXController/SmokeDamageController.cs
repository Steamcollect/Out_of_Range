using UnityEngine;
using UnityEngine.VFX;

[DefaultExecutionOrder(10)]
public class SmokeDamageController : MonoBehaviour
{
    private static readonly int s_PropertyIdIntensity = Shader.PropertyToID("Intensity");

    [Header("References")]
    [SerializeField] private EntityHealth m_EntityHealth;

    [SerializeField] private VisualEffect m_VisualEffect;

    private void OnEnable()
    {
        if (m_EntityHealth) HandleTakeDamage();
        if (m_EntityHealth) m_EntityHealth.OnTakeDamage += HandleTakeDamage;
    }

    private void OnDisable()
    {
        if (m_EntityHealth) m_EntityHealth.OnTakeDamage -= HandleTakeDamage;
    }

    private void HandleTakeDamage()
    {
        m_VisualEffect.SetFloat(s_PropertyIdIntensity, Mathf.Clamp01(1 - m_EntityHealth.GetHealthPercentage()));
    }
}