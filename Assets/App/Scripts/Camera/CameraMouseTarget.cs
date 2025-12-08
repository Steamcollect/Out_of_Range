using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraMouseTarget : MonoBehaviour
{
    [SerializeField] private InputActionReference m_MousePositionActionReference;

    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;

    [FormerlySerializedAs("layerMask")] [SerializeField] private LayerMask m_LayerMask;
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private float m_MaxDistance = 20f;
    [SerializeField] private float m_MinDistance = 5f;

    [SerializeField] private RSO_PlayerCameraController m_CamController;
    private EntityCombat m_EntityCombat;
    private InputAction m_MousePositionAction;

    private void Update()
    {
        Vector2 screenPoint = m_MousePositionAction.ReadValue<Vector2>();
        Ray ray = m_CamController.Get().GetCamera().ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_LayerMask))
        {
            Vector3 playerPosition = m_PlayerController.Get().transform.position;
            float distanceToTarget = Vector3.Distance(playerPosition, hit.point);
            Vector3 pos;

            if (hit.collider.TryGetComponent(out ITargetable target))
            {
                pos = target.GetTargetPosition();
            }
            else
            {
                Vector3 toHit = hit.point - playerPosition;
                Vector3 directionToTarget = toHit.normalized;

                float clampedDistance = Mathf.Clamp(distanceToTarget, m_MinDistance, m_MaxDistance);
                pos = playerPosition + directionToTarget * clampedDistance;
            }

            transform.position = pos;
        }
    }

    private void OnEnable()
    {
        m_MousePositionAction = m_MousePositionActionReference.action;
        m_MousePositionAction.Enable();

        m_AimTarget.Set(transform);
    }
}