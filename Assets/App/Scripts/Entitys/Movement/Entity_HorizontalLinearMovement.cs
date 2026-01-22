using MoreMountains.Tools;
using UnityEngine;

public class Entity_HorizontalLinearMovement : MonoBehaviour, IMovement
{
    [Header("Settings")]
    [SerializeField] private float m_MoveSpeed;
    
    [Space(10)]
    [SerializeField] LayerMask m_GroundLayer;
    [SerializeField] float m_MaxFallHeight;

    [Header("References")]
    [SerializeField] private Rigidbody m_Rb;

    //[Header("Input")]
    //[Header("Output")]

    public virtual void Move(Vector3 input)
    {
        float stepHeight = 0.4f;
        float stepDistance = m_MoveSpeed * Time.fixedDeltaTime;

        Vector3 stepCheckPos = transform.position
                               + Vector3.up * stepHeight
                               + input * stepDistance;

        bool stepHasGround = Physics.Raycast(
            stepCheckPos,
            Vector3.down,
            out RaycastHit stepHit,
            stepHeight + m_MaxFallHeight,
            m_GroundLayer
        );

        if (stepHasGround)
        {
            m_Rb.AddForce(input * m_MoveSpeed);
            return;
        }

        Vector3 vel = m_Rb.linearVelocity;
        float dot = Vector3.Dot(vel, input);

        if (dot > 0f)
        {
            vel -= input * dot;
            m_Rb.linearVelocity = vel;
        }
        return;
    }

    public void ResetVelocity()
    {
        m_Rb.linearVelocity = Vector3.zero;
        m_Rb.angularVelocity = Vector3.zero;
    }

    public float GetMoveSpeed() => m_MoveSpeed;
}