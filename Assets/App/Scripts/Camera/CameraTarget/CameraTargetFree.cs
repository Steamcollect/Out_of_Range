using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class CameraTargetFree : MonoBehaviour, ICameraTarget
    {
        
        [Header("Settings")]
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
            
            return hit.point;
        }
    }
}