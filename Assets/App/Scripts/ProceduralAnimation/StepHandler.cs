using UnityEngine;
using System.Collections;

public class StepHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float stepDuration = 0.25f;

    [Space(10)]
    [SerializeField] float stepLength = 0.5f;
    [SerializeField] float stepHeight = 0.1f;
    [SerializeField] AnimationCurve stepCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Space(10)]
    [SerializeField, Range(0, 1)] float anticipationMultiplier = .5f;

    Vector3 startLocalPosition;
    Vector3 currentIkPosition;

    bool canHandleStep = true;
    bool isDoingStep = false;

    Coroutine stepCoroutine;

    [Header("References")]
    [SerializeField] Transform ikTarget;

    Transform bodyPivot;
    StepManager stepManager;
    Rigidbody bodyRb;

    public void Setup(Transform bodyPivot, Rigidbody bodyRb, StepManager stepManager)
    {
        this.stepManager = stepManager;
        this.bodyPivot = bodyPivot;

        startLocalPosition = ikTarget.position - bodyPivot.position;
        this.bodyRb = bodyRb;
        currentIkPosition = ikTarget.position;
    }

    public void HandleIkPosition()
    {
        if (isDoingStep) return;

        ikTarget.position = currentIkPosition;
    }

    public void CheckStep()
    {
        if (!canHandleStep 
            || isDoingStep
            || !bodyPivot
            || !ikTarget) 
            return;

        float distance = Vector3.Distance(bodyPivot.position + startLocalPosition, ikTarget.position);

        if (distance > stepLength)
        {
            canHandleStep = false;
            stepManager.AddStep(HandleStep);
        }
    }

    void HandleStep()
    {
        if (stepCoroutine != null)
            StopCoroutine(stepCoroutine);

        stepCoroutine = StartCoroutine(DoMove());
    }

    IEnumerator DoMove()
    {
        isDoingStep = true;

        float elapsed = 0f;

        Vector3 startPos = ikTarget.position;

        Vector3 endPos = bodyPivot.TransformPoint(startLocalPosition)
            + bodyRb.linearVelocity.normalized * stepLength * anticipationMultiplier;

        while (elapsed < stepDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / stepDuration);

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            float curveValue = stepCurve != null ? stepCurve.Evaluate(t) : t;
            pos.y += stepHeight * curveValue;

            ikTarget.position = pos;

            yield return null;
        }

        ikTarget.position = endPos;
        currentIkPosition = ikTarget.position;

        canHandleStep = true;
        isDoingStep = false;
    }

    private void OnDrawGizmos()
    {
        if (!ikTarget) return;

        Transform body = bodyPivot ? bodyPivot : (transform.parent ? transform.parent : null);

        Vector3 center;

        if (body != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                center = body.TransformPoint(startLocalPosition);
            }
            else
            {
                center = ikTarget.position;
            }
#else
            center = body.TransformPoint(startLocalPosition);
#endif
        }
        else
        {
            center = ikTarget.position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, stepLength);
    }
}