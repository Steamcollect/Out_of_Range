using MVsToolkit.Wrappers;
using UnityEngine;
using UnityEngine.Events;

public class RSECallback : MonoBehaviour
{
    [SerializeField] RuntimeScriptableEvent m_RSE;
    public UnityEvent m_Callback;

    private void OnEnable()
    {
        m_RSE.Action += CallEvent;
    }

    private void OnDisable()
    {
        m_RSE.Action -= CallEvent;
    }

    public void CallEvent()
    {
        m_Callback.Invoke();
    }
}
