using UnityEngine;
using MVsToolkit.Dev;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float turnSmoothTime;
    [SerializeField] float angleOffset;
    float turnSmoothVelocity;

    [Header("References")]
    [SerializeField] InterfaceReference<IMovement> movement;

    [Header("Input")]
    [SerializeField] RSO_MainCamera cam;

    [Space(10)]
    [SerializeField] InputActionReference moveIA;
    [SerializeField] InputActionReference mousePositionIA;

    //[Header("Output")]

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveIA.action.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude <= .1f) return;

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + (cam.Get().transform.eulerAngles.y);
        Vector3 moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        movement.Value.Move(moveDir);
    }

    void HandleRotation()
    {
        Vector3 targetPos = GetMouseWorldPos();
        float targetAngle = Mathf.Atan2(targetPos.x, targetPos.z) * Mathf.Rad2Deg + (cam.Get().transform.eulerAngles.y) + angleOffset;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    Vector3 GetMouseWorldPos()
    {
        Ray ray = cam.Get().GetCamera().ScreenPointToRay(mousePositionIA.action.ReadValue<Vector2>());

        if (Mathf.Abs(ray.direction.y) < 0.0001f)
            return Vector3.zero;

        float t = (transform.position.y - ray.origin.y) / ray.direction.y;

        return ray.origin + ray.direction * t;
    }
}