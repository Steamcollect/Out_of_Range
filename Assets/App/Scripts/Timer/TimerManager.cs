using UnityEngine;
using UnityEngine.Serialization;

public class TimerManager : MonoBehaviour
{
    [FormerlySerializedAs("timerSpeed")] [SerializeField] private float m_TimerSpeed = 1;

    [FormerlySerializedAs("timer")] [SerializeField] private RSO_GameTimer m_Timer;

    private void Start()
    {
        m_Timer.Set(0);
    }

    private void Update()
    {
        m_Timer.Set(m_Timer.Get() + Time.deltaTime * m_TimerSpeed);
    }
}