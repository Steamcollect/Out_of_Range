using System;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float mTimeBetweenSteps = 0.15f;
    [SerializeField] StepHandler[] m_Handlers;

    Queue<Action> m_NextSteps = new Queue<Action>();
    float m_StepTimer = 0f;

    [Header("References")]
    [SerializeField] Transform m_MainBody;
    [SerializeField] BossMovementController m_Movement;

    private void Start()
    {
        if (!m_MainBody)
        {
            Debug.LogError($"[{nameof(StepManager)}] mainBody n'est pas assigné sur {name} !");
        }

        if (m_Handlers == null || m_Handlers.Length == 0)
        {
            Debug.LogWarning($"[{nameof(StepManager)}] Aucun StepHandler assigné sur {name}.");
            return;
        }

        foreach (StepHandler stepHandler in m_Handlers)
        {
            if (stepHandler != null)
                stepHandler.Setup(m_MainBody, this);
        }
    }

    private void Update()
    {
        m_StepTimer += Time.deltaTime;

        if (m_NextSteps.Count > 0 && m_StepTimer >= mTimeBetweenSteps)
        {
            var step = m_NextSteps.Dequeue();
            step?.Invoke();
            m_StepTimer = 0f;
        }

        if (m_Handlers == null) return;

        foreach (StepHandler stepHandler in m_Handlers)
        {
            if (stepHandler != null)
                stepHandler.HandleIkPosition();
        }
    }

    private void FixedUpdate()
    {
        if (m_Handlers == null) return;

        foreach (StepHandler stepHandler in m_Handlers)
        {
            if (stepHandler != null)
                stepHandler.CheckStep();
        }
    }

    public void AddStep(Action step)
    {
        if (step == null) return;
        m_NextSteps.Enqueue(step);
    }
}