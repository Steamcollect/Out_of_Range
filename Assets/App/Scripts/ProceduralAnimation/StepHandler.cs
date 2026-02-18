using UnityEngine;
using System.Collections;

public class StepHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_StepDuration = 0.25f;

    [Space(10)]
    [SerializeField] float m_StepLength = 0.5f;
    [SerializeField] float m_StepHeight = 0.1f;
    [SerializeField] AnimationCurve m_StepCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Space(10)]
    [SerializeField, Range(0, 1)] float m_AnticipationMultiplier = .5f;

    Vector3 m_StartLocalPosition;
    Vector3 m_CurrentIkPosition;

    bool m_CanHandleStep = true;
    bool m_IsDoingStep = false;

    Coroutine m_StepCoroutine;

    [Header("References")]
    [SerializeField] Transform m_IkTarget;

    Transform m_BodyPivot;
    StepManager m_StepManager;
    BossMovementController m_Movement;

    public void Setup(Transform bodyPivot, StepManager stepManager, BossMovementController movement)
    {
        this.m_StepManager = stepManager;
        this.m_BodyPivot = bodyPivot;
        this.m_Movement = movement;

        m_StartLocalPosition = m_IkTarget.position - bodyPivot.position;
        m_CurrentIkPosition = m_IkTarget.position;
    }

    public void HandleIkPosition()
    {
        if (m_IsDoingStep) return;

        m_IkTarget.position = m_CurrentIkPosition;
    }

    public void CheckStep()
    {
        if (!m_CanHandleStep 
            || m_IsDoingStep
            || !m_BodyPivot
            || !m_IkTarget) 
            return;

        float distance = Vector3.Distance(m_BodyPivot.position + m_StartLocalPosition, m_IkTarget.position);

        if (distance > m_StepLength)
        {
            m_CanHandleStep = false;
            m_StepManager.AddStep(HandleStep);
        }
    }

    void HandleStep()
    {
        if (m_StepCoroutine != null)
            StopCoroutine(m_StepCoroutine);

        m_StepCoroutine = StartCoroutine(DoMove());
    }

    IEnumerator DoMove()
    {
        m_IsDoingStep = true;

        float elapsed = 0f;

        Vector3 startPos = m_IkTarget.position;

        Vector3 endPos = m_Movement.ApplyOnCylinder(m_BodyPivot.position + m_StartLocalPosition);

        while (elapsed < m_StepDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / m_StepDuration);

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            //float curveValue = m_StepCurve != null ? m_StepCurve.Evaluate(t) : t;
            //pos.y += m_StepHeight * curveValue;

            m_IkTarget.position = pos;

            yield return null;
        }

        m_IkTarget.position = endPos;
        m_CurrentIkPosition = m_IkTarget.position;

        m_CanHandleStep = true;
        m_IsDoingStep = false;
    }

    private void OnDrawGizmos()
    {
        if (!m_IkTarget) return;

        Transform body = m_BodyPivot ? m_BodyPivot : (transform.parent ? transform.parent : null);

        Vector3 center;

        if (body != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                center = body.TransformPoint(m_StartLocalPosition);
            }
            else
            {
                center = m_IkTarget.position;
            }
#else
            center = body.TransformPoint(startLocalPosition);
#endif
        }
        else
        {
            center = m_IkTarget.position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, m_StepLength);
    }
}