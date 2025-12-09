using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TriggerEventCallback : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private string m_PlayerTag = "Player";
    
    [Header("Output")]
    [SerializeField] private UnityEvent m_EnterEvent;
    [SerializeField] private UnityEvent m_ExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(m_PlayerTag)) m_EnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(m_PlayerTag)) m_ExitEvent.Invoke();
    }
}