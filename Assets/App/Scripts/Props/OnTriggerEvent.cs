using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class OnTriggerEvent : MonoBehaviour
{
    [FormerlySerializedAs("enterEvent")] public UnityEvent EnterEvent;

    [FormerlySerializedAs("exitEvent")] public UnityEvent ExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") EnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") ExitEvent.Invoke();
    }
}