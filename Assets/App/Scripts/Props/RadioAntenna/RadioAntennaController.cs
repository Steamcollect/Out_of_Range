using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RadioAntennaController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RadioAntennaTrigger m_Trigger;

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnPlayerInteract;

    private bool m_HasInteract;

    private void Start()
    {
        m_HasInteract = false;
        m_Trigger.SetCanPlayerInteract(true);
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
        m_OnPlayerInteract.Invoke();
    }
}