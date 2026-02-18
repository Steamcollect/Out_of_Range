using System;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float timeBetweenSteps = 0.15f;
    [SerializeField] StepHandler[] handlers;

    Queue<Action> nextSteps = new Queue<Action>();
    float stepTimer = 0f;

    [Header("References")]
    [SerializeField] Transform mainBody;
    [SerializeField] Rigidbody mainRb;

    private void Awake()
    {
        if (!mainBody)
        {
            Debug.LogError($"[{nameof(StepManager)}] mainBody n'est pas assigné sur {name} !");
        }

        if (handlers == null || handlers.Length == 0)
        {
            Debug.LogWarning($"[{nameof(StepManager)}] Aucun StepHandler assigné sur {name}.");
            return;
        }

        foreach (StepHandler stepHandler in handlers)
        {
            if (stepHandler != null)
                stepHandler.Setup(mainBody, mainRb, this);
        }
    }

    private void Update()
    {
        stepTimer += Time.deltaTime;

        if (nextSteps.Count > 0 && stepTimer >= timeBetweenSteps)
        {
            var step = nextSteps.Dequeue();
            step?.Invoke();
            stepTimer = 0f;
        }

        if (handlers == null) return;

        foreach (StepHandler stepHandler in handlers)
        {
            if (stepHandler != null)
                stepHandler.HandleIkPosition();
        }
    }

    private void FixedUpdate()
    {
        if (handlers == null) return;

        foreach (StepHandler stepHandler in handlers)
        {
            if (stepHandler != null)
                stepHandler.CheckStep();
        }
    }

    public void AddStep(Action step)
    {
        if (step == null) return;
        nextSteps.Enqueue(step);
    }
}