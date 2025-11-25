using UnityEngine;
using MVsToolkit.Dev;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //[Header("Settings")]
    
    [Header("References")]
    [SerializeField] InterfaceReference<IMovement> movement;

    [Header("Input")]
    [SerializeField] InputActionReference moveInputAction;
    
    //[Header("Output")]

    private void FixedUpdate()
    {
        Vector2 moveInput = moveInputAction.action.ReadValue<Vector2>();

        movement.Value.Move(new Vector3(moveInput.x, 0f, moveInput.y));
    }
}