using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent enterEvent;

    public UnityEvent exitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            enterEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            exitEvent.Invoke();
        }
    }
}
