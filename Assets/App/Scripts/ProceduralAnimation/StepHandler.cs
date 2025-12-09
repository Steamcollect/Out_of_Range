using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class StepHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_StepDuration = 0.25f;
    [Space(10)] 
    [SerializeField] private float m_StepLength = 0.5f;
    [SerializeField] private float m_StepHeight = 0.1f;
    [SerializeField] private AnimationCurve m_StepCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Space(10)] 
    [SerializeField] [Range(0, 1)] private float m_AnticipationMultiplier;
    [Header("References")]
    [SerializeField] private Transform m_IKTarget;

    private Transform m_BodyPivot;
    private Rigidbody m_BodyRb;

    private bool m_CanHandleStep = true;
    private Vector3 m_CurrentIkPosition;
    private bool m_IsDoingStep;

    private Vector3 m_StartLocalPosition;

    private Coroutine m_StepCoroutine;
    private StepManager m_StepManager;

    private void OnDrawGizmos()
    {
        if (!m_IKTarget) return;

        Transform body = m_BodyPivot ? m_BodyPivot : transform.parent ? transform.parent : null;

        Vector3 center;

        if (body != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                center = body.TransformPoint(m_StartLocalPosition);
            else
                center = m_IKTarget.position;
#else
            center = body.TransformPoint(startLocalPosition);
#endif
        }
        else
        {
            center = m_IKTarget.position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, m_StepLength);
    }

    public void Setup(Transform bodyPivot, Rigidbody bodyRb, StepManager stepManager)
    {
        this.m_StepManager = stepManager;
        this.m_BodyPivot = bodyPivot;

        m_StartLocalPosition = m_IKTarget.position - bodyPivot.position;
        this.m_BodyRb = bodyRb;
        m_CurrentIkPosition = m_IKTarget.position;
    }

    public void HandleIkPosition()
    {
        if (m_IsDoingStep) return;

        m_IKTarget.position = m_CurrentIkPosition;
    }

    public void CheckStep()
    {
        if (!m_CanHandleStep
            || m_IsDoingStep
            || !m_BodyPivot
            || !m_IKTarget)
            return;

        float distance = Vector3.Distance(m_BodyPivot.position + m_StartLocalPosition, m_IKTarget.position);

        if (distance > m_StepLength)
        {
            m_CanHandleStep = false;
            m_StepManager.AddStep(HandleStep);
        }
    }

    private void HandleStep()
    {
        if (m_StepCoroutine != null)
            StopCoroutine(m_StepCoroutine);

        m_StepCoroutine = StartCoroutine(DoMove());
    }

    private IEnumerator DoMove()
    {
        m_IsDoingStep = true;

        float elapsed = 0f;

        Vector3 startPos = m_IKTarget.position;

        Vector3 endPos = m_BodyPivot.TransformPoint(m_StartLocalPosition)
                         + m_BodyRb.linearVelocity.normalized * (m_StepLength * m_AnticipationMultiplier);

        while (elapsed < m_StepDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / m_StepDuration);

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            float curveValue = m_StepCurve?.Evaluate(t) ?? t;
            pos.y += m_StepHeight * curveValue;

            m_IKTarget.position = pos;

            yield return null;
        }

        m_IKTarget.position = endPos;
        m_CurrentIkPosition = m_IKTarget.position;

        m_CanHandleStep = true;
        m_IsDoingStep = false;
    }
}