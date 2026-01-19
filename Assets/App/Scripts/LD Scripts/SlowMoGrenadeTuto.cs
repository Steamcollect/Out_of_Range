using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SlowMoGrenadeTuto : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference m_SecondaryAction;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float m_SlowTimeScale = 0.2f;

    [Header("DOTween transitions")]
    [SerializeField, Min(0f)] private float m_EnterSlowDuration = 0.2f;
    [SerializeField, Min(0f)] private float m_ExitSlowDuration = 0.2f;
    [SerializeField] private Ease m_EnterEase = Ease.OutQuad;
    [SerializeField] private Ease m_ExitEase = Ease.OutQuad;

    public UnityEvent onSlowMoStart;
    public UnityEvent onSlowMoEnd;

    private bool isInSlowMo = false;
    private Tween m_TimeScaleTween;

    public void StartSlowMoAndWaitForSecondary()
    {
        if (isInSlowMo)
            return;
        StartCoroutine(SlowMoCoroutine());
        onSlowMoStart?.Invoke();
    }

    private IEnumerator SlowMoCoroutine()
    {
        isInSlowMo = true;
        float originalTimeScale = Time.timeScale;
        float originalFixedDelta = Time.fixedDeltaTime;

        m_TimeScaleTween?.Kill();
        m_TimeScaleTween = DOTween.To(
            () => Time.timeScale,
            x =>
            {
                Time.timeScale = Mathf.Clamp01(x);
                Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;
            },
            Mathf.Clamp01(m_SlowTimeScale),
            Mathf.Max(0.0001f, m_EnterSlowDuration)
        ).SetEase(m_EnterEase).SetUpdate(true);

        bool pressed = false;
        void OnPerformed(InputAction.CallbackContext ctx) => pressed = true;

        var action = m_SecondaryAction.action;
        bool weEnabledAction = false;
        if (!action.enabled)
        {
            action.Enable();
            weEnabledAction = true;
        }

        action.performed += OnPerformed;

        float startRealtime = Time.realtimeSinceStartup;

        while (!pressed)
        {
            yield return null;
        }

        action.performed -= OnPerformed;
        if (weEnabledAction)
            action.Disable();

        m_TimeScaleTween?.Kill();
        m_TimeScaleTween = DOTween.To(
            () => Time.timeScale,
            x =>
            {
                Time.timeScale = Mathf.Clamp01(x);
                Time.fixedDeltaTime = originalFixedDelta * Time.timeScale;
            },
            Mathf.Clamp01(originalTimeScale),
            Mathf.Max(0.0001f, m_ExitSlowDuration)
        ).SetEase(m_ExitEase).SetUpdate(true);

        yield return m_TimeScaleTween.WaitForCompletion();

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDelta;
        onSlowMoEnd?.Invoke();
        isInSlowMo = false;
    }
}
