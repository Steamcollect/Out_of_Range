using UnityEngine;
using UnityEngine.Serialization;

public class Entity_HorizontalLinearMovement : MonoBehaviour, IMovement
{
    [FormerlySerializedAs("moveSpeed")]
    [Header("Settings")]
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] float m_MoveSpeedMultiplier = 1;

    [FormerlySerializedAs("rb")]
    [Header("References")]
    [SerializeField] private Rigidbody m_Rb;

    //[Header("Input")]
    //[Header("Output")]

    public void Move(Vector3 input)
    {
        if (m_MoveSpeed <= 0) return;
        m_Rb.AddForce(input * m_MoveSpeed * m_MoveSpeedMultiplier);
    }

    public void SetSpeedMult(float mult) => m_MoveSpeedMultiplier = mult;

    public void ResetVelocity()
    {
        m_Rb.linearVelocity = Vector3.zero;
        m_Rb.angularVelocity = Vector3.zero;
    }
}