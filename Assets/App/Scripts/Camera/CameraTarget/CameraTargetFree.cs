using UnityEngine;
using UnityEngine.InputSystem;

public sealed class CameraTargetFree : MonoBehaviour, ICameraTarget
{
    [Header("Settings")]
    [SerializeField] float m_GamepadSensitivity = 0.5f;

    [Space]
    [SerializeField] private LayerMask m_LayerMask = ~0;
    [SerializeField] private QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Ignore;

    Vector2 m_NormalizedPoint;

    [Header("References")]
    [SerializeField] private InputActionReference m_MousePosIA;
    [SerializeField] private InputActionReference m_LookIA;
    [Space(10)]
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private RSO_PlayerCameraController m_CamController;

    [SerializeField] RSO_CurrentInputDeviceType m_CurrentInputDevice;

    [Header("Input")]
    [SerializeField] RSE_SetFreeLookCamTargetPos m_SetTargetPos;

    void Awake()
    {
        m_LookIA.action.Enable();
    }

    void OnEnable()
    {
        m_SetTargetPos.Action += SetTargetPos;
    }

    void OnDisable()
    {
        m_SetTargetPos.Action -= SetTargetPos;
    }

    public Vector3? GetCameraTargetPosition()
    {
        if(m_CurrentInputDevice.Value == InputDeviceType.Gamepad)
        {
            m_NormalizedPoint += m_LookIA.action.ReadValue<Vector2>()
            / new Vector2(Screen.width, Screen.height)
             * (m_GamepadSensitivity);
        }
        else if (m_CurrentInputDevice.Value == InputDeviceType.KeyboardMouse)
        {
            m_NormalizedPoint = m_MousePosIA.action.ReadValue<Vector2>() 
                / new Vector2(Screen.width, Screen.height);
        }

        m_NormalizedPoint.x = Mathf.Clamp01(m_NormalizedPoint.x);
        m_NormalizedPoint.y = Mathf.Clamp01(m_NormalizedPoint.y);

        Vector2 pixelPoint = new Vector2(
            m_NormalizedPoint.x * Screen.width,
            m_NormalizedPoint.y * Screen.height
        );

        Ray ray = m_CamController.Get().GetCamera().ScreenPointToRay(pixelPoint);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_LayerMask, m_QueryTriggerInteraction))
            return null;

        return hit.point;
    }

    public ITargetable GetCameraTarget()
    {
        return null;
    }

    void SetTargetPos(Vector2 pos)
    {
        m_NormalizedPoint = pos;
    }
}