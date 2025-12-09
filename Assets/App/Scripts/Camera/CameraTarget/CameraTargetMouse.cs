using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class CameraTargetMouse : MonoBehaviour, ICameraTarget
    {
        
        [Header("Settings")]
        [SerializeField] private float m_MinDistance = 2f;
        [SerializeField] private float m_MaxDistance = 10f;
        [SerializeField] private LayerMask m_LayerMask = ~0;
        [SerializeField] private QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Ignore;
        
        [Header("References")]
        [SerializeField] private InputActionReference m_MousePositionIa;
        [Space(10)]
        [SerializeField] private RSO_PlayerController m_PlayerController;
        [SerializeField] private RSO_PlayerCameraController m_CamController;
        
        public Vector3? GetCameraTargetPosition()
        {
            Vector2 screenPoint = m_MousePositionIa.action.ReadValue<Vector2>();
            Ray ray = m_CamController.Get().GetCamera().ScreenPointToRay(screenPoint);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_LayerMask,m_QueryTriggerInteraction)) return null;
            
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
            return pos;

        }
    }
}