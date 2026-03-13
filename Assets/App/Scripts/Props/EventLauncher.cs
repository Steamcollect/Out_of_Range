using UnityEngine;
using UnityEngine.Events;

public class EventLauncher : MonoBehaviour
{
    [SerializeField] UnityEvent m_OnStart;

    void Start()
    {
        m_OnStart?.Invoke();
    }
}
