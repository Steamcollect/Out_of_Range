using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouseTarget : MonoBehaviour
{
    [SerializeField] private InputActionReference m_MousePositionActionReference;
    private InputAction m_MousePositionAction;
    
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private float m_MaxDistance = 20f;
    [SerializeField] private float m_MinDistance = 5f;
    private EntityCombat m_EntityCombat;
    private void OnEnable()
    {
        m_MousePositionAction = m_MousePositionActionReference.action;
        m_MousePositionAction.Enable();
        
        m_AimTarget.Set(transform);
    }

    void Update()
    {
        var screenPoint = m_MousePositionAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Vector3 playerPosition = m_PlayerController.Get().transform.position;
            float distanceToTarget = Vector3.Distance(playerPosition, hit.point);
            Vector3 pos;

            if (hit.collider.TryGetComponent<ITargetable>(out ITargetable target))
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
}
