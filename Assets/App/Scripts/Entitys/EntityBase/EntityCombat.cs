using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityCombat : MonoBehaviour, ILookAtTarget
{
    [FormerlySerializedAs("turnSmoothTime")]
    [Header("Settings")]
    [SerializeField] private float m_TurnSmoothTime;

    [FormerlySerializedAs("currentCombatStyle")]
    [Header("References")]
    [SerializeField] protected CombatStyle m_CurrentCombatStyle;

    [FormerlySerializedAs("verticalPivot")] [Space(10)] [SerializeField] protected Transform m_VerticalPivot;

    [FormerlySerializedAs("horizontalPivot")] [SerializeField] protected Transform m_HorizontalPivot;
    private bool m_CanLookAt = true;

    private Vector3 m_TurnSmoothHozirontalVelocity, m_TurnSmoothVerticalVelocity;

    public virtual void LookAt(Vector3 targetPos)
    {
        if (!m_CanLookAt) return;

        Vector3 direction = targetPos - m_HorizontalPivot.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        if (m_HorizontalPivot)
        {
            Vector3 horizontalDir = direction;
            horizontalDir.y = 0f;
            if (horizontalDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotY = Quaternion.LookRotation(horizontalDir);
                m_HorizontalPivot.LookAtSmoothDamp(
                    m_HorizontalPivot.position + horizontalDir,
                    ref m_TurnSmoothHozirontalVelocity,
                    m_TurnSmoothTime
                );
            }
        }

        if (m_VerticalPivot)
        {
            Vector3 verticalDir = direction.normalized;

            Vector3 verticalLookPoint = m_VerticalPivot.position + verticalDir;

            m_VerticalPivot.LookAtSmoothDamp(
                verticalLookPoint,
                ref m_TurnSmoothVerticalVelocity,
                m_TurnSmoothTime
            );
        }
    }

    public virtual void Attack()
    {
        m_CurrentCombatStyle.Attack();
    }

    public Vector3 GetLookAtDirection()
    {
        return (m_VerticalPivot.forward + m_HorizontalPivot.forward).normalized;
    }

    public CombatStyle GetCombatStyle()
    {
        return m_CurrentCombatStyle;
    }

    public Vector3 GetVerticalPivotPos()
    {
        return m_VerticalPivot.position;
    }

    public void SetActiveLookAt(bool canLookAt)
    {
        this.m_CanLookAt = canLookAt;
    }
}