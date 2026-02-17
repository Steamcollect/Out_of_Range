using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class PlayerController : EntityController
{
    [SerializeField] float m_MinYPos;
    [SerializeField] float gravityScale;

    Vector3 m_LastGroundPos;
    private readonly Queue<(float time, Vector3 pos)> m_PositionHistory
        = new Queue<(float time, Vector3 pos)>();

    [Header("References")]
    [SerializeField] private RSO_PlayerCameraController m_CamController;
    [Space(10)] 
    [SerializeField] private InputPlayerController m_InputPlayerController;
    [SerializeField] private PlayerAnimationVisual m_AnimVisual;
    [SerializeField] private Entity_Dash m_Dash;
    [SerializeField] private PlayerMana m_Mana;
    [SerializeField] PlayerPowerUp m_PowerUp;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerDie m_OnPlayerDie;
    [SerializeField] private RSO_PlayerController m_Controller;
    
    private bool m_IsMoving;
    private Vector3 m_MoveDir;

    private void OnEnable() => m_InputPlayerController.OnInputDashPressed += Dash;

    private void OnDisable() => m_InputPlayerController.OnInputDashPressed -= Dash;
    
    protected override void OnEntityDie()
    {
        m_PowerUp.OnPlayerDeath();
        base.OnEntityDie();
        m_OnPlayerDie.Call();
        SceneLoader.Instance?.LoadGameplayScene();
    }

    protected override void Awake()
    {
        base.Awake();
        m_Controller.Set(this);
    }
    
    private void Start() => Teleport(PlayerSpawnPoint.S_Position);

    private void Update()
    {
        if (transform.position.y <= m_MinYPos) m_Rb.position = m_LastGroundPos;
    }

    private void FixedUpdate()
    {
        if (m_Dash.IsGrounded(transform.position))
        {
            if (!m_Dash.IsDashing)
                UpdateLastGroundPosBuffer();
        }
        else
        {
            m_Rb.AddForce(Vector3.down * gravityScale);
        }

        HandleMovement();
    }


    private void HandleMovement()
    {
        Vector2 moveInput = m_InputPlayerController.GetMoveDirection();

        float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg
                      + m_CamController.Get().GetCamera().transform.eulerAngles.y;

        Vector3 rawDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            m_MoveDir = Vector3.ProjectOnPlane(rawDir, hit.normal).normalized;
        else
            m_MoveDir = rawDir;

        if (moveInput.sqrMagnitude <= 0.1f)
        {
            m_IsMoving = false;
            m_MoveDir = Vector3.zero;
        }
        else if (!m_Dash.IsDashing)
        {
            m_IsMoving = true;
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

    private void UpdateLastGroundPosBuffer()
    {
        float now = Time.time;

        m_PositionHistory.Enqueue((now, transform.position));

        // Supprimer les positions trop anciennes (> 1 seconde)
        while (m_PositionHistory.Count > 0 && now - m_PositionHistory.Peek().time > 1f)
            m_PositionHistory.Dequeue();

        // La plus ancienne position restante = position d'il y a ~1 seconde
        if (m_PositionHistory.Count > 0)
            m_LastGroundPos = m_PositionHistory.Peek().pos;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTargetPosition(), .2f);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(transform.position.x, m_MinYPos, transform.position.z), new Vector3(1, .05f, 1) * 10);
    }

    public PlayerCombat GetPlayerCombat()
    {
        return m_Combat as PlayerCombat;
    }

    public Entity_Dash GetDash()
    {
        return m_Dash;
    }

    public PlayerMana GetPlayerMana()
    {
        return m_Mana;
    }

    public PlayerPowerUp GetPowerUp()=> m_PowerUp;
}