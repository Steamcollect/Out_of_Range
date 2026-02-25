using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class KillStreakHUD : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_IncreaseShakeForce;
    [SerializeField] float m_IncreaseShakeDuration;

    [Space(8)]
    [SerializeField] float m_DecreaseBumpSize;
    [SerializeField] float m_DecreaseBumpDuration;

    [Header("References")]
    [SerializeField] TMP_Text m_CurrentStreakNameTxt;
    [SerializeField] Image m_CurrentStreakImage;

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

    void OnIncrease(KillStreakStep step)
    {
        ResetTxt();

        if (step.StepName == "") return;

        m_CurrentStreakNameTxt.text = step.StepName;
        m_CurrentStreakNameTxt.color = step.StepColor;
        m_CurrentStreakNameTxt.transform.DOPunchRotation(Vector3.forward * m_IncreaseShakeForce, m_IncreaseShakeDuration, 1, 20);
    }

    void OnDecrease(KillStreakStep step)
    {
        ResetTxt();

        if (step.KillStreakRequire == 0)
        {
            m_CurrentStreakImage.fillAmount = 0;
            m_CurrentStreakNameTxt.transform.DOScale(1.1f, m_DecreaseBumpDuration).OnComplete(() =>
            {
                m_CurrentStreakNameTxt.transform.DOScale(0, m_DecreaseBumpDuration * 1.5f);
            });
        }
        else
        {
            m_CurrentStreakNameTxt.text = step.StepName;
            m_CurrentStreakNameTxt.color = step.StepColor;
            m_CurrentStreakNameTxt.transform.DOScale(m_DecreaseBumpSize, m_DecreaseBumpDuration * .8f).OnComplete(() =>
            {
                m_CurrentStreakNameTxt.transform.DOScale(1, m_DecreaseBumpDuration);
            });
        }        
    }

    void OnTimerChange(float value)
    {
        m_CurrentStreakImage.fillAmount = value;
    }

    void ResetTxt()
    {
        m_CurrentStreakNameTxt.DOKill();
        m_CurrentStreakNameTxt.transform.localRotation = Quaternion.identity;
        m_CurrentStreakNameTxt.transform.localScale = Vector3.one;
    }
}