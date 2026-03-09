using DG.Tweening;
using UnityEngine;

public class KillStreakVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrailRenderer m_Trail;

    [Space(5)]
    [SerializeField] RSO_KillStreakTimer m_KillStreakTimer;

    [Header("Input")]
    [SerializeField] RSE_OnStepIncrease m_OnStepIncrease;
    [SerializeField] RSE_OnStepDecrease m_OnStepDecrease;

    //[Header("Output")]

    private void OnEnable()
    {
        m_OnStepIncrease.Action += OnIncrease;
        m_OnStepDecrease.Action += OnDecrease;
        m_KillStreakTimer.OnChanged += OnTimerChange;
    }

    private void OnDisable()
    {
        m_OnStepIncrease.Action -= OnIncrease;
        m_OnStepDecrease.Action -= OnDecrease;
        m_KillStreakTimer.OnChanged -= OnTimerChange;
    }

    private void Start()
    {
        m_Trail.time = 0;
    }

    void OnIncrease(KillStreakStep step)
    {
        m_Trail.DOKill();
        m_Trail.material.DOKill();

        m_Trail.DOTime(step.TrailLifetime, 1f);
        m_Trail.material.DOColor(step.StepColor, .1f);
    }

    void OnDecrease(KillStreakStep step)
    {
        m_Trail.DOKill();
        m_Trail.material.DOKill();

        m_Trail.DOTime(step.TrailLifetime, 1f);
        m_Trail.material.DOColor(step.StepColor, .1f);
    }

    void OnTimerChange(float value) { }
}