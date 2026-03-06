using MVsToolkit.Dev;
using UnityEngine;

public class Entity_HorizontalLinearMovement : MonoBehaviour, IMovement
{
    [Header("Settings")]
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] float m_MoveSpeedMultiplier = 1;
    [SerializeField, ReadOnly] bool m_CanMove = true;

    [Header("References")]
    [SerializeField] private Rigidbody m_Rb;

    //[Header("Input")]
    //[Header("Output")]

    public void Move(Vector3 input)
    {
        if (!m_CanMove || m_MoveSpeed <= 0) return;
        m_Rb.AddForce(input * m_MoveSpeed * m_MoveSpeedMultiplier);
    }

    public void SetSpeedMult(float mult) => m_MoveSpeedMultiplier = mult;

    public void ResetVelocity()
    {
        m_Rb.linearVelocity = Vector3.zero;
        m_Rb.angularVelocity = Vector3.zero;
    }

    public void SetCanMove(bool canMove)
    {
        m_CanMove = canMove;
    }
}