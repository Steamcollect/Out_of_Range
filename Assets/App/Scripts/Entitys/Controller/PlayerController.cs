using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{
    Vector3 moveDir;
    bool isMoving = false;

    [Header("Internal References")]
    [SerializeField] InputActionReference moveIA;
    [SerializeField] RSO_PlayerCameraController m_CamController;

    [Space(10)]
    [SerializeField] EntityRotationVisual m_RotationVisual;
    [SerializeField] Entity_Dash dash;

    [Header("Input")]
    [SerializeField] InputActionReference dashIA;

    [Header("Output")]
    [SerializeField] RSO_PlayerController controller;

    private void OnEnable()
    {
        dashIA.action.started += Dash;
    }
    private void OnDisable()
    {
        dashIA.action.started -= Dash;
    }

    private void Awake()
    {
        controller.Set(this);

        dashIA.action.Enable();
        moveIA.action.Enable();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveIA.action.ReadValue<Vector2>();

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg
                      + m_CamController.Get().GetCamera().transform.eulerAngles.y;

        // Direction brute par rapport à la caméra
        Vector3 rawDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        // Raycast vers le sol pour obtenir la normale
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            // Projection de la direction sur le plan du sol
            moveDir = Vector3.ProjectOnPlane(rawDir, hit.normal).normalized;
        }
        else
        {
            // Si pas de sol détecté, on garde la direction brute
            moveDir = rawDir;
        }

        if (moveInput.sqrMagnitude <= .1f)
        {
            isMoving = false;
            m_RotationVisual.Rotate(Vector2.zero);
        }
        else
        {
            isMoving = true;
            m_RotationVisual.Rotate(moveDir);
            movement.Value.Move(moveDir);
        }
    }


    void Dash(InputAction.CallbackContext ctx)
    {
        if (!isMoving) return;

        dash.Dash(moveDir);
    }
}