using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class RadioAntennaController : MonoBehaviour
{
    //[Header("References")]

    [FormerlySerializedAs("trigger")]
    [Header("References")]
    [SerializeField] private RadioAntennaTrigger m_Trigger;

    [FormerlySerializedAs("_OnPlayerInteract")]
    [Header("Output")]
    [SerializeField] private UnityEvent m_OnPlayerInteract;

    private bool m_AsInteract;

    private void Start()
    {
        m_AsInteract = false;
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
        if (m_AsInteract) return;

        m_AsInteract = true;
        m_OnPlayerInteract.Invoke();
    }
}