using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class RadioAntennaController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RadioAntennaTrigger m_Trigger;
    [ColorUsage(true, true)] [SerializeField] private Color m_DefaultColor;
    [ColorUsage(true, true)] [SerializeField] private Color m_ActiveColor;
    [SerializeField] private VisualEffect m_Effect;

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnPlayerInteract;

    private bool m_HasInteract;

    private void Start()
    {
        m_HasInteract = false;
        m_Trigger.SetCanPlayerInteract(true);
        m_Effect.SetVector4("Color", m_DefaultColor);
    }

    private void OnEnable()
    {
        m_Trigger.OnPlayerInteract += OnPlayerInteract;
    }

    private void OnDisable()
    {
        m_Trigger.OnPlayerInteract -= OnPlayerInteract;
    }

    private void OnPlayerInteract()
    {
        if (m_HasInteract) return;

        m_HasInteract = true;
        m_Trigger.SetCanPlayerInteract(false);
        m_OnPlayerInteract.Invoke();
        m_Effect.SetVector4("Color", m_ActiveColor);
    }
}