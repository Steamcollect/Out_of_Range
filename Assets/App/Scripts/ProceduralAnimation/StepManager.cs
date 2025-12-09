using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StepManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_TimeBetweenSteps = 0.15f;
    [SerializeField] private StepHandler[] m_Handlers;
    
    [Header("References")]
    [SerializeField] private Transform m_MainBody;
    [SerializeField] private Rigidbody m_MainRb;

    private readonly Queue<Action> m_NextSteps = new();
    private float m_StepTimer;

    private void Awake()
    {
        if (!m_MainBody) Debug.LogError($"[{nameof(StepManager)}] mainBody n'est pas assigne sur {name} !");

        if (m_Handlers == null || m_Handlers.Length == 0)
        {
            Debug.LogWarning($"[{nameof(StepManager)}] Aucun StepHandler assign� sur {name}.");
            return;
        }

        foreach (StepHandler stepHandler in m_Handlers)
            if (stepHandler != null)
                stepHandler.Setup(m_MainBody, m_MainRb, this);
    }

    private void Update()
    {
        m_StepTimer += Time.deltaTime;

        if (m_NextSteps.Count > 0 && m_StepTimer >= m_TimeBetweenSteps)
        {
            Action step = m_NextSteps.Dequeue();
            step?.Invoke();
            m_StepTimer = 0f;
        }

        if (m_Handlers == null) return;

        foreach (StepHandler stepHandler in m_Handlers)
            if (stepHandler)
                stepHandler.HandleIkPosition();
    }

    private void FixedUpdate()
    {
        if (m_Handlers == null) return;

        foreach (StepHandler stepHandler in m_Handlers)
            if (stepHandler)
                stepHandler.CheckStep();
    }

    public void AddStep(Action step)
    {
        if (step == null) return;
        m_NextSteps.Enqueue(step);
    }
}