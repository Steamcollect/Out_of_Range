using System.Collections;
using MVsToolkit.Utils;
using UnityEngine;

public class EntityCombat : MonoBehaviour, ILookAtTarget
{
    [Header("Settings")]
    [SerializeField] private float m_TurnSmoothTime;

    [Header("References")]
    [SerializeField] protected Transform m_VerticalPivot;
    [SerializeField] protected Transform m_HorizontalPivot;

    private bool m_CanLookAt = true;

    protected bool m_IsAttacking = false;

    private Vector3 m_TurnSmoothHozirontalVelocity, m_TurnSmoothVerticalVelocity;

    public virtual void LookAt(Vector3 targetPos, LookAtAxis lookAtAxis = LookAtAxis.Both, float turnSmoothTime = 999)
    {
        if (!m_CanLookAt) return;

        Vector3 direction = targetPos - m_HorizontalPivot.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        if(turnSmoothTime == 999) turnSmoothTime = m_TurnSmoothTime;

        if (m_HorizontalPivot && lookAtAxis != LookAtAxis.Vertical)
        {
            Vector3 horizontalDir = direction;
            horizontalDir.y = 0f;
            if (horizontalDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotY = Quaternion.LookRotation(horizontalDir);
                m_HorizontalPivot.LookAtSmoothDamp(
                    m_HorizontalPivot.position + horizontalDir,
                    ref m_TurnSmoothHozirontalVelocity,
                    turnSmoothTime
                );
            }
        }

        if (m_VerticalPivot && lookAtAxis != LookAtAxis.Horizontal)
        {
            Vector3 verticalDir = direction.normalized;

            Vector3 verticalLookPoint = m_VerticalPivot.position + verticalDir;

            m_VerticalPivot.LookAtSmoothDamp(
                verticalLookPoint,
                ref m_TurnSmoothVerticalVelocity,
                turnSmoothTime
            );
        }
    }

    public virtual IEnumerator Attack() { yield break; }

    public virtual bool IsAttacking() { return m_IsAttacking; }

    public Vector3 GetLookAtDirection() => (m_VerticalPivot.forward + m_HorizontalPivot.forward).normalized;
    public Vector3 GetVerticalPivotPos() => m_VerticalPivot.position;

    public void SetActiveLookAt(bool canLookAt) => m_CanLookAt = canLookAt;
}