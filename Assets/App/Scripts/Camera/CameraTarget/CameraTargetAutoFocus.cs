
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
    // [SerializeField] private Vector2 m_MinMaxDistanceToTarget = new Vector2(2, 20);
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
    
    private float m_SphereRadius = 1f;
    
    private Vector3 m_LastMousePosition;

    private void Awake()
    {
        m_SphereRadius = m_RadiusCursorDetection /2f;
    }

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
        m_LastMousePosition = hitMouseWorld.point;
        Vector3? resultPosition = FindTargetInRadius(hitMouseWorld.point) ?? FindDirectTarget(hitMouseWorld.point) ?? hitMouseWorld.point;
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
        Vector3 direction = (mouseWorldPos - playerPos).normalized;
        float distance = Vector3.Distance(playerPos, mouseWorldPos);
        float sphereRadius = m_SphereRadius; // Ajustez selon vos besoins

        if (Physics.SphereCast(playerPos, sphereRadius, direction, out RaycastHit hit, distance, m_LayerMaskHittable, m_QueryTriggerInteraction))
        {
            if (!hit.collider.TryGetComponent(out ITargetable target)) return null;
            if (!TargetInRange(mouseWorldPos, target.GetTargetPosition())) return null;
            m_TargetDirectlyFound = true;
            return target.GetTargetPosition();
        }
        return null;
        
        // Vector3 playerPos = m_PlayerController.Get().transform.position;
        // m_RayPlayerToMouseWorld = new Ray(playerPos, (mouseWorldPos - playerPos).normalized);
        //
        // if (Physics.Raycast(m_RayPlayerToMouseWorld, out RaycastHit hit, Vector3.Distance(playerPos, mouseWorldPos), m_LayerMaskHittable, m_QueryTriggerInteraction))
        // {
        //     if (hit.collider.TryGetComponent(out ITargetable target))
        //     {
        //         if (!TargetInRange(mouseWorldPos,target.GetTargetPosition(),out float _)) return null;
        //         m_TargetDirectlyFound = true;
        //         return target.GetTargetPosition();
        //     }
        // }
        // return null;
    }

    private bool TargetInRange(Vector3 origin, Vector3 targetPosition)
    {
        Vector3 targetPositionVS = m_CamController.Get().GetCamera().WorldToViewportPoint(targetPosition);

        return targetPositionVS.x is >= 0 and <= 1 &&
               targetPositionVS.y is >= 0 and <= 1 &&
               targetPositionVS.z > 0;
    }

    private Vector3? FindTargetInRadius(Vector3 mouseWorldPos)
    {
        int size = Physics.OverlapSphereNonAlloc(mouseWorldPos, m_RadiusCursorDetection, m_TargetResults, m_LayerMaskTargets, m_QueryTriggerInteraction);

        ITargetable closestTargetToMouse = null;
        float dCloseMouseTarget = Mathf.Infinity;
        
        ITargetable closestTargetToPlayer = null;
        float dClosePlayerTarget = Mathf.Infinity;
        
        for (int i = 0; i < size; i++)
        {
            if (m_TargetResults[i].TryGetComponent(out ITargetable sphereTarget))
            {
                Vector3 playerPos = m_PlayerController.Get().transform.position;
                Vector3 targetPos = sphereTarget.GetTargetPosition();
                Ray rayToTarget = new(playerPos, (targetPos - playerPos).normalized);

                if (!TargetInRange(mouseWorldPos,targetPos)) continue;
                
                float dPlayerTarget = Vector3.Distance(playerPos, targetPos);
                float dMouseTarget = Vector3.Distance(mouseWorldPos, targetPos);
                
                
                bool isObstructed = Physics.Raycast(rayToTarget, out RaycastHit obstructionHit, dPlayerTarget, m_LayerMaskHittable, m_QueryTriggerInteraction) &&
                                    obstructionHit.transform != m_TargetResults[i].transform;
                if (isObstructed) continue;
                
                if (dPlayerTarget < dClosePlayerTarget)
                {
                    dClosePlayerTarget = dPlayerTarget;
                    closestTargetToPlayer = sphereTarget;
                }

                if (dMouseTarget < dCloseMouseTarget)
                {
                    dCloseMouseTarget = dMouseTarget;
                    closestTargetToMouse = sphereTarget;
                }
            }
        }

        if (closestTargetToMouse == null) return null;
        
        
        m_TargetInRangeFound = true;
        return dClosePlayerTarget < dCloseMouseTarget ? closestTargetToPlayer!.GetTargetPosition() : closestTargetToMouse.GetTargetPosition();
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !m_IsVerbose) return;
        
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(m_RayCamToScreenPoint);

        Vector3 playerPos = m_PlayerController.Get().transform.position;
        float distance = Vector3.Distance(playerPos, m_LastMousePosition);

        Gizmos.color = m_TargetDirectlyFound ? Color.green : Color.red;

        // Dessiner le SphereCast (sphère de départ + sphère d'arrivée + lignes)
        Vector3 endPos = playerPos + m_RayPlayerToMouseWorld.direction * distance;

        Gizmos.DrawWireSphere(playerPos, m_SphereRadius);
        Gizmos.DrawWireSphere(endPos, m_SphereRadius);

        // Lignes pour visualiser le cylindre du SphereCast
        Vector3 up = Vector3.Cross(m_RayPlayerToMouseWorld.direction, Vector3.right).normalized * m_SphereRadius;
        if (up == Vector3.zero) up = Vector3.Cross(m_RayPlayerToMouseWorld.direction, Vector3.forward).normalized * m_SphereRadius;

        Vector3 right = Vector3.Cross(m_RayPlayerToMouseWorld.direction, up).normalized * m_SphereRadius;

        Gizmos.DrawLine(playerPos + up, endPos + up);
        Gizmos.DrawLine(playerPos - up, endPos - up);
        Gizmos.DrawLine(playerPos + right, endPos + right);
        Gizmos.DrawLine(playerPos - right, endPos - right);

        
            Gizmos.color = m_TargetInRangeFound ? Color.green : Color.red;
            Gizmos.DrawWireSphere(m_LastMousePosition, m_RadiusCursorDetection);
    }
}