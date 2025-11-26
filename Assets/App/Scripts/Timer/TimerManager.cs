using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] float timerSpeed = 1;

    [SerializeField] RSO_GameTimer timer;

    private void Start()
    {
        timer.Set(0);
    }

    private void Update()
    {
        timer.Set(timer.Get() + Time.deltaTime * timerSpeed);
    }
}