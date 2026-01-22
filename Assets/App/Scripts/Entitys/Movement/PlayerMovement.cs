using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovement
{
    [Header("Settings")]
    [SerializeField] float m_MoveSpeed;

    [Header("References")]
    [SerializeField] Rigidbody m_Rb;

    //[Header("Input")]
    //[Header("Output")]
    public void Move(Vector3 input)
    {
        m_Rb.AddForce(input * m_MoveSpeed);
    }

    public void ResetVelocity()
    {
        m_Rb.linearVelocity = Vector3.zero;
        m_Rb.angularVelocity = Vector3.zero;
    }

    public float GetMoveSpeed() => m_MoveSpeed;
}