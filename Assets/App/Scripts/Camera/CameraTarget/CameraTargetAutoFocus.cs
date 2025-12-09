
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetAutoFocus : MonoBehaviour, ICameraTarget
{
    [Header("Settings")]
    [SerializeField] private bool m_IsVerbose;
    [Tooltip("Must contain hittable, ex: Wall, Default ... And Target Layer")]
    [SerializeField] private LayerMask m_LayerMaskHittable;
    [SerializeField] private LayerMask m_LayerMaskTargets;
    [SerializeField] private float m_RadiusCursorDetection = 5;
    [SerializeField] private Vector2 m_MinMaxDistanceToTarget = new Vector2(2, 20);
    [SerializeField] private QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("References")]
    [SerializeField] private InputActionReference m_MousePositionIa;
    [Space(10)]
    [SerializeField] private RSO_PlayerCameraController m_CamController;
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private const int k_TargetResultsBufferSize = 10;
    private readonly Collider[] m_TargetResults = new Collider[k_TargetResultsBufferSize];

    
    private Ray m_RayCamToScreenPoint;
    private Ray m_RayPlayerToMouseWorld;
    private bool m_TargetDirectlyFound;
    private bool m_TargetInRangeFound;
    
    private void OnEnable() => m_MousePositionIa.action.Enable();
    private void OnDisable() => m_MousePositionIa.action.Disable();

    public Vector3? GetCameraTargetPosition()
    {

        InitDebug();
        
        Vector2 screenPoint = m_MousePositionIa.action.ReadValue<Vector2>();
        m_RayCamToScreenPoint = m_CamController.Get().GetCamera().ScreenPointToRay(screenPoint);

        if (!Physics.Raycast(m_RayCamToScreenPoint, out RaycastHit hitMouseWorld, Mathf.Infinity, m_LayerMaskHittable, m_QueryTriggerInteraction))
        {
            return null;
        }
        Vector3? resultPosition = FindDirectTarget(hitMouseWorld.point) ?? FindTargetInRadius(hitMouseWorld.point) ?? hitMouseWorld.point;
        return resultPosition;
    }

    private void InitDebug()
    {
        m_TargetDirectlyFound = false;
        m_TargetInRangeFound = false;
    }

    private Vector3? FindDirectTarget(Vector3 mouseWorldPos)
    {
        Vector3 playerPos = m_PlayerController.Get().transform.position;
        m_RayPlayerToMouseWorld = new Ray(playerPos, (mouseWorldPos - playerPos).normalized);

        if (Physics.Raycast(m_RayPlayerToMouseWorld, out RaycastHit hit, Vector3.Distance(playerPos, mouseWorldPos), m_LayerMaskHittable, m_QueryTriggerInteraction))
        {
            if (hit.collider.TryGetComponent(out ITargetable target))
            {
                if (!TargetInRange(target.GetTargetPosition(),out float _)) return null;
                m_TargetDirectlyFound = true;
                return target.GetTargetPosition();
            }
        }
        return null;
    }

    private bool TargetInRange(Vector3 targetPosition, out float distanceToTarget)
    {
        distanceToTarget = Vector3.Distance(m_PlayerController.Get().transform.position, targetPosition);
        return distanceToTarget >= m_MinMaxDistanceToTarget.x && distanceToTarget <= m_MinMaxDistanceToTarget.y;
    }

    private Vector3? FindTargetInRadius(Vector3 mouseWorldPos)
    {
        int size = Physics.OverlapSphereNonAlloc(mouseWorldPos, m_RadiusCursorDetection, m_TargetResults, m_LayerMaskTargets, m_QueryTriggerInteraction);

        for (int i = 0; i < size; i++)
        {
            if (m_TargetResults[i].TryGetComponent(out ITargetable sphereTarget))
            {
                Vector3 playerPos = m_PlayerController.Get().transform.position;
                Vector3 targetPos = sphereTarget.GetTargetPosition();
                var rayToTarget = new Ray(playerPos, (targetPos - playerPos).normalized);

                if (!TargetInRange(targetPos, out float distanceToTarget)) continue;
                
                bool isObstructed = Physics.Raycast(rayToTarget, out RaycastHit obstructionHit, distanceToTarget, m_LayerMaskHittable, m_QueryTriggerInteraction) &&
                                    obstructionHit.transform != m_TargetResults[i].transform;
                if (!isObstructed)
                {
                    m_TargetInRangeFound = true;
                    return sphereTarget.GetTargetPosition();
                }
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !m_IsVerbose) return;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(m_RayCamToScreenPoint);

        Gizmos.DrawWireSphere(m_RayPlayerToMouseWorld.origin, m_MinMaxDistanceToTarget.y);
        
        
        Gizmos.color = m_TargetDirectlyFound ? Color.green : Color.red;
        Gizmos.DrawRay(m_RayPlayerToMouseWorld.origin, m_RayPlayerToMouseWorld.direction * m_MinMaxDistanceToTarget.y);

        if (!m_TargetDirectlyFound)
        {
            Gizmos.color = m_TargetInRangeFound ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, m_RadiusCursorDetection);
        }
    }
}