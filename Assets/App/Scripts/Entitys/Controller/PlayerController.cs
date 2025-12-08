using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : EntityController
{
    [FormerlySerializedAs("moveIA")]
    [Header("Internal References")]
    [SerializeField] private InputActionReference m_MoveIa;

    [SerializeField] private RSO_PlayerCameraController m_CamController;

    [Space(10)] [SerializeField] private EntityRotationVisual m_RotationVisual;

    [FormerlySerializedAs("dash")] [SerializeField] private Entity_Dash m_Dash;

    [FormerlySerializedAs("dashIA")]
    [Header("Input")]
    [SerializeField] private InputActionReference m_DashIa;

    [FormerlySerializedAs("controller")]
    [Header("Output")]
    [SerializeField] private RSO_PlayerController m_Controller;

    private bool m_IsMoving;
    private Vector3 m_MoveDir;

    private void Awake()
    {
        m_Health.OnDeath += () => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); };

        base.Awake();

        m_Controller.Set(this);

        m_DashIa.action.Enable();
        m_MoveIa.action.Enable();
    }

    private void Start()
    {
        m_Rb.position = PlayerSpawnPoint.S_Position;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnEnable()
    {
        m_DashIa.action.started += Dash;
    }

    private void OnDisable()
    {
        m_DashIa.action.started -= Dash;
    }

    private void HandleMovement()
    {
        Vector2 moveInput = m_MoveIa.action.ReadValue<Vector2>();

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg
                      + m_CamController.Get().GetCamera().transform.eulerAngles.y;

        // Direction brute par rapport � la cam�ra
        Vector3 rawDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        // Raycast vers le sol pour obtenir la normale
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            // Projection de la direction sur le plan du sol
            m_MoveDir = Vector3.ProjectOnPlane(rawDir, hit.normal).normalized;
        else
            // Si pas de sol d�tect�, on garde la direction brute
            m_MoveDir = rawDir;

        if (moveInput.sqrMagnitude <= .1f)
        {
            m_IsMoving = false;
            m_RotationVisual.Rotate(Vector2.zero);
        }
        else
        {
            m_IsMoving = true;
            m_RotationVisual.Rotate(m_MoveDir);
            m_Movement.Value.Move(m_MoveDir);
        }
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if (!m_IsMoving) return;

        m_Dash.Dash(m_MoveDir);
    }
}