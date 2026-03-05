using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCursorFeedback : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_ShakeForce;
    [SerializeField] float m_ScaleMultPerKill;
    [SerializeField] float m_ScaleMax = 4f;
    [SerializeField] float m_ShakeDuration;
    [SerializeField] float m_AnimDuration;

    Color m_DefaultColor;
    float m_CurrentScale;

    bool m_IsRunning = false;

    Coroutine m_CooldownCoroutine;

    [Header("References")]
    [SerializeField] Image m_FeedbackCursor;
    [SerializeField] InputActionReference m_MousePositionIA;

    [Header("Input")]
    [SerializeField] RSE_OnEnemyDie m_OnEnemyDie;

    //[Header("Output")]

    private void OnEnable()
    {
        m_OnEnemyDie.Action += OnEntityKilled;
    }
    private void OnDisable()
    {
        m_OnEnemyDie.Action -= OnEntityKilled;
    }

    private void Start()
    {
        m_DefaultColor = m_FeedbackCursor.color;
        m_FeedbackCursor.gameObject.SetActive(false);
    }

    void OnEntityKilled()
    {
        if (m_IsRunning)
        {
            StopCoroutine(m_CooldownCoroutine);
            m_FeedbackCursor.DOKill();
            m_FeedbackCursor.transform.DOKill();
        }
        else
        {
            m_FeedbackCursor.transform.localScale = Vector3.one;
            m_FeedbackCursor.transform.rotation = Quaternion.identity;
            m_CurrentScale = 1;

            m_FeedbackCursor.gameObject.SetActive(true);
        }

        m_FeedbackCursor.color = m_DefaultColor;
        m_CooldownCoroutine = StartCoroutine(RunningCooldown());

        m_FeedbackCursor.transform.DOPunchRotation(Vector3.forward * m_ShakeForce, m_ShakeDuration, 20, 1);
        
        m_CurrentScale = Mathf.Min(m_CurrentScale * m_ScaleMultPerKill, m_ScaleMax);
        m_FeedbackCursor.transform.DOScale(m_CurrentScale, m_ShakeDuration);

        m_FeedbackCursor.DOFade(0, m_AnimDuration);
    }

    IEnumerator RunningCooldown()
    {
        m_IsRunning = true;
        yield return new WaitForSeconds(m_AnimDuration);

        m_FeedbackCursor.gameObject.SetActive(false);
        m_IsRunning = false;
    }
}