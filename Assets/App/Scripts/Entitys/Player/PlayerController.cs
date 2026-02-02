using DefaultNamespace;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class PlayerController : EntityController
{    
    [Header("References")]
    [SerializeField] private RSO_PlayerCameraController m_CamController;
    [Space(10)] 
    [SerializeField] private InputPlayerController m_InputPlayerController;
    [SerializeField] private PlayerAnimationVisual m_AnimVisual;
    [SerializeField] private Entity_Dash m_Dash;
    [SerializeField] private PlayerMana m_Mana;

    [Space(10)]
    [SerializeField] RSO_CurrentPowerUp m_CurrentPowerUp;

    [Space(10)]
    [SerializeField] LayerMask m_GroundLayer;
    
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
        SceneLoader.Instance?.LoadGameplayScene();
    }

    protected override void Awake()
    {
        base.Awake();
        m_Controller.Set(this);

        m_CurrentPowerUp.Set(new());
        m_Health.OnTakeDamage += () => m_CurrentPowerUp.Value.Clear();
    }
    
    private void Start() => Teleport(PlayerSpawnPoint.S_Position);


    private void FixedUpdate() => HandleMovement();

    private void HandleMovement()
    {
        Vector2 moveInput = m_InputPlayerController.GetMoveDirection();

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg
                      + m_CamController.Get().GetCamera().transform.eulerAngles.y;

        Vector3 rawDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            // Projection de la direction sur le plan du sol
            m_MoveDir = Vector3.ProjectOnPlane(rawDir, hit.normal).normalized;
        else
            m_MoveDir = rawDir;

        if (moveInput.sqrMagnitude <= 0.1f)
        {
            m_IsMoving = false;
            m_MoveDir = Vector3.zero;
        }
        else
        {
            m_IsMoving = true;

            float stepDistance = m_Movement.Value.GetMoveSpeed() * Time.fixedDeltaTime;
            Vector3 nextPos = transform.position + m_MoveDir * stepDistance;

            bool hasGround = Physics.Raycast(
                nextPos + Vector3.up * 0.2f,
                Vector3.down,
                out RaycastHit groundHit,
                2f,
                m_GroundLayer
            );

            if (!hasGround)
            {
                m_IsMoving = false;

                // Annule la vélocité qui pousse vers le vide
                Vector3 vel = m_Rb.linearVelocity;
                float dot = Vector3.Dot(vel, m_MoveDir);

                if (dot > 0f)
                {
                    vel -= m_MoveDir * dot;
                    m_Rb.linearVelocity = vel;
                }

                m_MoveDir = Vector3.zero;
                m_AnimVisual.OnMove(Vector3.zero);
                return;
            }

            m_Movement.Value.Move(m_MoveDir);
        }

        m_AnimVisual.OnMove(m_MoveDir);

    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if (!m_IsMoving) return;
        m_Dash.Dash(m_MoveDir);
    }
    
    public void Teleport(Vector3 position)
    {
        m_CamController.Get().PreProcessTeleportCamera();
        transform.position = position;
        m_Rb.position = position;
        m_CamController.Get().TeleportCamera();
    }

    public PlayerCombat GetPlayerCombat()
    {
        return m_Combat as PlayerCombat;
    }
    
    public Entity_Dash GetDash()
    {
        return m_Dash;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTargetPosition(), .2f);
    }

    public PlayerMana GetPlayerMana()
    {
        return m_Mana;
    }
}