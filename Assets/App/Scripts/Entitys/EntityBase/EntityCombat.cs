using System.Collections;
using MVsToolkit.Utilities;
using UnityEngine;

public class EntityCombat : MonoBehaviour, ILookAtTarget
{
    [Header("Settings")]
    [SerializeField] protected float m_TurnSmoothTime;
    [SerializeField] protected bool m_CanRotate = true;
    [SerializeField] protected float m_StunTimeOnSpawn = 1;

    [Header("References")]
    [SerializeField] protected Transform m_HorizontalPivot;

    protected bool m_CanLookAt = true;

    protected bool m_CanAttackOnSpawn = true;
    protected bool m_CanAttack = true;
    protected bool m_IsAttacking = false;

    private float m_CurrentTurnSmoothTime;

    Quaternion m_TurnSmoothHozirontalVelocity;

    public virtual void LookAt(Vector3 targetPos, LookAtAxis lookAtAxis = LookAtAxis.Both)
    {
        if (!m_CanRotate || !m_CanLookAt) return;

        Vector3 direction = targetPos - m_HorizontalPivot.position;
        if (direction.sqrMagnitude < 0.0001f) return;

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
                    m_CurrentTurnSmoothTime
                );
            }
        }
    }

    public virtual IEnumerator LockAttackOnSpawn()
    {
        m_CanAttackOnSpawn = false;
        yield return new WaitForSeconds(m_StunTimeOnSpawn);
        m_CanAttackOnSpawn = true;
    }

    public virtual IEnumerator Attack() { yield break; }

    public virtual bool IsAttacking() { return m_IsAttacking; }

    public Vector3 GetLookAtDirection() => m_HorizontalPivot.forward.normalized;

    public void SetActiveLookAt(bool canLookAt) => m_CanLookAt = canLookAt;

    public void SetCanAttack(bool canAttack) => m_CanAttack = canAttack;
    public void SetTurnSmoothTime(float turnSmoothTime) => m_CurrentTurnSmoothTime = turnSmoothTime;
}