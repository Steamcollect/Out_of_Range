
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
    [SerializeField] private QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("References")]
    [SerializeField] private InputActionReference m_MousePositionIa;
    [SerializeField] private InputActionReference m_DirectionIa;
    [Space(10)]
    [SerializeField] private RSO_PlayerCameraController m_CamController;
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    private const int k_TargetResultsBufferSize = 10;
    private readonly Collider[] m_TargetResults = new Collider[k_TargetResultsBufferSize];

    
    private Ray m_RayCamToScreenPoint;
    private bool m_TargetDirectlyFound;
    private bool m_TargetInRangeFound;
    
    private float m_SphereRadius = 1f;
    
    private Vector3 m_LastMousePosition;
    
    private Vector3 m_LastGamepadDirection;

    private void Awake()
    {
        m_SphereRadius = m_RadiusCursorDetection;
    }

    private void OnEnable()
    {
        m_MousePositionIa.action.Enable();
        m_DirectionIa.action.Enable();
    }

    private void OnDisable()
    {
        m_DirectionIa.action.Disable();
        m_MousePositionIa.action.Disable();
    }

    public Vector3? GetCameraTargetPosition()
    {
        InitDebug();

        return HandleCameraTargetMouse();
        

        switch (m_CurrentInputDeviceType.Value)
        {
            case InputDeviceType.Gamepad:
                return HandleCameraTargetGamepad();
            case InputDeviceType.KeyboardMouse:
                return HandleCameraTargetMouse();
            default:
                throw new NotImplementedException(m_CurrentInputDeviceType.Value.ToString());
        }
    }

    private Vector3? HandleCameraTargetMouse()
    {
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

    private Vector3? HandleCameraTargetGamepad()
    {
        Vector3 directionInput = m_DirectionIa.action.ReadValue<Vector2>();

        if (directionInput == Vector3.zero)
            return m_PlayerController.Get().GetTargetPosition() + m_LastGamepadDirection;
        
        (directionInput.z, directionInput.y) = (directionInput.y, directionInput.z);
        
        m_LastGamepadDirection = directionInput.DirectionRelativeToCamera();
        
        m_RayCamToScreenPoint = new Ray(m_PlayerController.Get().GetTargetPosition(), m_LastGamepadDirection);
        
        if (!Physics.SphereCast(m_PlayerController.Get().GetTargetPosition(),5f, m_LastGamepadDirection, out RaycastHit hitInfo, Mathf.Infinity,
                m_LayerMaskHittable, m_QueryTriggerInteraction))
        {
           return m_PlayerController.Get().GetTargetPosition() + m_LastGamepadDirection;
        }
        
        Vector3 hitPosition = hitInfo.point;
        Vector3? resultPosition = FindTargetInRadius(hitPosition) ?? FindDirectTarget(hitPosition) ?? hitPosition;
        return resultPosition;
    }

    private void InitDebug()
    {
        m_TargetDirectlyFound = false;
        m_TargetInRangeFound = false;
    }

    private Vector3? FindDirectTarget(Vector3 mouseWorldPos)
    {
        Vector3 playerPos = m_PlayerController.Get().GetTargetPosition();
        
        mouseWorldPos.y = playerPos.y;
        
        Vector3 direction = (mouseWorldPos - playerPos).normalized;
        float distance = Vector3.Distance(playerPos, mouseWorldPos);
        
        
        if (!Physics.SphereCast(playerPos, m_SphereRadius, direction, out RaycastHit potentialTarget, distance,
                m_LayerMaskHittable, m_QueryTriggerInteraction)) return null;
        
        if (!potentialTarget.collider.TryGetComponent(out ITargetable target)) return null;
        
        Debug.DrawRay(playerPos, target.GetTargetPosition() - playerPos, Color.red);
        if (!Physics.Raycast(playerPos, (target.GetTargetPosition() - playerPos).normalized, out RaycastHit hit,
                Vector3.Distance(target.GetTargetPosition(), playerPos), m_LayerMaskHittable,
                m_QueryTriggerInteraction) || hit.transform != potentialTarget.transform) return null;
        
        
        if (!TargetInRange(mouseWorldPos, target.GetTargetPosition())) return null;
        
        m_TargetDirectlyFound = true;
        return target.GetTargetPosition();

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
                Vector3 playerPos = m_PlayerController.Get().GetTargetPosition();
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

        Vector3 playerPos = m_PlayerController.Get().GetTargetPosition();

        Vector3 targetPos = new(m_LastMousePosition.x, playerPos.y, m_LastMousePosition.z);
        
        float distance = Vector3.Distance(playerPos, targetPos);
        
        Vector3 endPos = playerPos + (targetPos - playerPos).normalized * distance;
        
        
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireSphere(playerPos, m_SphereRadius);
        Gizmos.DrawWireSphere(endPos, m_SphereRadius);
        Gizmos.DrawLine(playerPos, endPos);
        
        Gizmos.color = m_TargetDirectlyFound ? Color.green : (m_TargetInRangeFound ? Color.yellow : Color.red);
        Gizmos.DrawWireSphere(targetPos, m_RadiusCursorDetection);
    }
}