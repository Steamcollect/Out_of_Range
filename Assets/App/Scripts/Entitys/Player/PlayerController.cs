using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{    
    [Header("References")]
    [SerializeField] private RSO_PlayerCameraController m_CamController;
    [Space(10)] 
    [SerializeField] private InputPlayerController m_InputPlayerController;
    [SerializeField] private EntityRotationVisual m_RotationVisual;
    [SerializeField] private Entity_Dash m_Dash;

    [Space(10)]
    [SerializeField] RSO_CurrentPowerUp m_CurrentPowerUp;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerDie m_OnPlayerDie;
    [SerializeField] private RSO_PlayerController m_Controller;

    private bool m_IsMoving;
    private Vector3 m_MoveDir;
    
    private void OnEnable() => m_InputPlayerController.OnInputDashPressed += Dash;

    private void OnDisable() => m_InputPlayerController.OnInputDashPressed -= Dash;
    
    protected override void OnEntityDie()
    {
        base.OnEntityDie();
        m_OnPlayerDie.Call();
        SceneLoader.Instance.LoadGameplayScene();
    }

    protected override void Awake()
    {
        base.Awake();
        m_Controller.Set(this);

        m_CurrentPowerUp.Set(null);
        m_Health.OnTakeDamage += () => m_CurrentPowerUp.Set(null);
    }
    
    private void Start() => Teleport(PlayerSpawnPoint.S_Position);


    private void FixedUpdate() => HandleMovement();

    private void HandleMovement()
    {
        Vector2 moveInput = m_InputPlayerController.GetMoveDirection();

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg
                      + m_CamController.Get().GetCamera().transform.eulerAngles.y;

        // Direction brute par rapport a la camera
        Vector3 rawDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        // Raycast vers le sol pour obtenir la normale
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            // Projection de la direction sur le plan du sol
            m_MoveDir = Vector3.ProjectOnPlane(rawDir, hit.normal).normalized;
        else
            // Si pas de sol detecte, on garde la direction brute
            m_MoveDir = rawDir;

        if (moveInput.sqrMagnitude <= 0.1f)
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
    
    public void Teleport(Vector3 position) => m_Rb.position = position;

    public PlayerCombat GetPlayerCombat()
    {
        return m_Combat as PlayerCombat;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTargetPosition(), .2f);
    }
}