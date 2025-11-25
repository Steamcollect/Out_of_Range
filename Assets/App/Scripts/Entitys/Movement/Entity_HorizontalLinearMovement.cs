using UnityEngine;

public class Entity_HorizontalLinearMovement : MonoBehaviour, IMovement
{
    [Header("Settings")]
    [SerializeField] float moveSpeed;

    [Header("References")]
    [SerializeField] Rigidbody rb;

    //[Header("Input")]
    //[Header("Output")]

    public void Move(Vector3 input)
    {
        rb.AddForce(input * moveSpeed);
    }
}