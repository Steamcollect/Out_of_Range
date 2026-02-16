
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class CameraTargetAutoFocus : MonoBehaviour, ICameraTarget
{
    [Header("Settings Layers")]
    [Tooltip("Must contain hittable, ex: Wall, Default ... And Target Layer")]
    [SerializeField] private LayerMask m_LayerMaskHittable;
    [SerializeField] private LayerMask m_LayerMaskTargets;
    [SerializeField] private QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Ignore;
    
    [Header("Settings Detection")]
    [SerializeField] private float m_RadiusSphereCastCursor = 5;
    [SerializeField] private float m_RadiusDetectionCursor = 15;
    [Space]
    [SerializeField] private float m_RadiusSphereCastGamepad = 5;
    [SerializeField] private float m_RadiusDetectionGamepad = 15;
    [SerializeField] private float m_RadiusDetectionGamepadNoInput = 10;
    [Space] [SerializeField] private float m_MaxDistanceSphereCast;

    [Header("References")]
    [SerializeField] private InputActionReference m_MousePositionIa;
    [SerializeField] private InputActionReference m_DirectionIa;
    [Space(10)]
    [SerializeField] private RSO_PlayerCameraController m_CamController;
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDeviceType;

    private const int k_TargetResultsBufferSize = 10;
    private readonly Collider[] m_TargetResults = new Collider[k_TargetResultsBufferSize];

    #region Fields Debug

    private Vector3 m_InputGamepadDirection;
    private Vector3 m_OriginPointInRadiusTargetCheck;
    private float m_RadiusDirectTargetCheck;
    
    private Vector3  m_InputGamepadDirectionRelativeToCamera;
    private bool m_SphereCastCheckValid;
    
    private bool m_TargetDirectCheckValid;
    private bool m_TargetInRangeCheckValid;
    private Vector3 m_HitPosition;

    #endregion
    

    #region Unity Callbacks

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

    #endregion

    #region ICameraTarget

    public Vector3? GetCameraTargetPosition()
    {
        InitializeDebug();
        
        return m_CurrentInputDeviceType.Value switch
        {
            InputDeviceType.Gamepad => HandleCameraTargetGamepad(),
            InputDeviceType.KeyboardMouse => HandleCameraTargetMouse(),
            _ => throw new NotImplementedException(m_CurrentInputDeviceType.Value.ToString())
        };
    }

    private void InitializeDebug()
    { 
        m_InputGamepadDirection = Vector3.zero;
        m_OriginPointInRadiusTargetCheck = Vector3.zero;
        m_RadiusDirectTargetCheck = 0;
        
        m_InputGamepadDirectionRelativeToCamera = Vector3.zero;
        m_SphereCastCheckValid= false;
        
        m_TargetDirectCheckValid = false;
        m_TargetInRangeCheckValid = false;
        m_HitPosition = Vector3.zero;
    }
    
    #endregion

    #region Handle Camera Target Device

    private Vector3? HandleCameraTargetMouse()
    {
        Vector2 screenPoint = m_MousePositionIa.action.ReadValue<Vector2>();

        if (!Physics.Raycast(m_CamController.Get().GetCamera().ScreenPointToRay(screenPoint), out RaycastHit hitMouseWorld, Mathf.Infinity, m_LayerMaskHittable, m_QueryTriggerInteraction))
        {
            return null;
        }
        
        m_HitPosition = hitMouseWorld.point;
        
        Vector3? resultPosition = FindTargetInRadius(m_HitPosition, m_RadiusSphereCastCursor) ?? FindDirectTarget(m_HitPosition, m_RadiusSphereCastCursor) ?? m_HitPosition;
        return resultPosition;
    }

    private Vector3? HandleCameraTargetGamepad()
    {
        m_InputGamepadDirection = m_DirectionIa.action.ReadValue<Vector2>();

        if (m_InputGamepadDirection == Vector3.zero)
        {
            Vector3? targetInRadius = FindTargetInRadius(m_PlayerController.Get().GetTargetPosition(),m_RadiusDetectionGamepadNoInput);

            if (targetInRadius.HasValue) return targetInRadius.Value;
            if (m_PlayerController.Value.Velocity == Vector3.zero) return null;
            return m_PlayerController.Get().GetTargetPosition() + m_PlayerController.Value.Velocity.normalized;

        }
        
        (m_InputGamepadDirection.z, m_InputGamepadDirection.y) = (m_InputGamepadDirection.y, m_InputGamepadDirection.z);

         m_InputGamepadDirectionRelativeToCamera = m_InputGamepadDirection.DirectionRelativeToCamera();
        
        
        if (!Physics.SphereCast(m_PlayerController.Get().GetTargetPosition(),m_RadiusSphereCastGamepad, m_InputGamepadDirectionRelativeToCamera, out RaycastHit hitInfo, m_MaxDistanceSphereCast,
                m_LayerMaskHittable, m_QueryTriggerInteraction))
        {
            return FindTargetInRadius(m_PlayerController.Get().GetTargetPosition(),m_RadiusDetectionGamepad) 
                   ?? m_PlayerController.Get().GetTargetPosition() + m_InputGamepadDirectionRelativeToCamera;
        }

        m_SphereCastCheckValid = true;
        
        m_HitPosition = hitInfo.point;
        Vector3? resultPosition = FindTargetInRadius(m_HitPosition, m_RadiusSphereCastCursor) ?? 
                                  FindDirectTarget(m_HitPosition,m_RadiusSphereCastGamepad) ?? m_HitPosition;
        return resultPosition;
    }

    #endregion
    
    #region Target Detection

    private Vector3? FindDirectTarget(Vector3 originPoint, float radiusSphereCast)
    {
        Vector3 playerPos = m_PlayerController.Get().GetTargetPosition();
        
        originPoint.y = playerPos.y;
        
        Vector3 direction = (originPoint - playerPos).normalized;
        float distance = Vector3.Distance(playerPos, originPoint);
        
        
        if (!Physics.SphereCast(playerPos, radiusSphereCast, direction, out RaycastHit potentialTarget, distance,
                m_LayerMaskHittable, m_QueryTriggerInteraction)) return null;
        
        if (!potentialTarget.collider.TryGetComponent(out ITargetable target)) return null;
        
        Debug.DrawRay(playerPos, target.GetTargetPosition() - playerPos, Color.red);
        if (!Physics.Raycast(playerPos, (target.GetTargetPosition() - playerPos).normalized, out RaycastHit hit,
                Vector3.Distance(target.GetTargetPosition(), playerPos), m_LayerMaskHittable,
                m_QueryTriggerInteraction) || hit.transform != potentialTarget.transform) return null;


        if (!TargetInRange(target.GetTargetPosition()))
        {
            m_TargetDirectCheckValid = false;
            return null;
        }


        m_TargetDirectCheckValid = true;
        
        return target.GetTargetPosition();

    }

    private Vector3? FindTargetInRadius(Vector3 originPoint, float radiusDetection)
    {
        m_OriginPointInRadiusTargetCheck = originPoint;
        m_RadiusDirectTargetCheck = radiusDetection;
        
        int size = Physics.OverlapSphereNonAlloc(originPoint, radiusDetection, m_TargetResults, m_LayerMaskTargets, m_QueryTriggerInteraction);

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

                if (!TargetInRange(targetPos)) continue;
                
                float dPlayerTarget = Vector3.Distance(playerPos, targetPos);
                float dMouseTarget = Vector3.Distance(originPoint, targetPos);
                
                
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

        if (closestTargetToMouse == null)
        {
            m_TargetInRangeCheckValid = false;
            return null;
        }
        
        m_TargetInRangeCheckValid = true;
        
        return dClosePlayerTarget < dCloseMouseTarget ? closestTargetToPlayer!.GetTargetPosition() : closestTargetToMouse.GetTargetPosition();
    }
    
    private bool TargetInRange(Vector3 targetPosition)
    {
        Vector3 targetPositionVS = m_CamController.Get().GetCamera().WorldToViewportPoint(targetPosition);

        return targetPositionVS.x is >= 0 and <= 1 &&
               targetPositionVS.y is >= 0 and <= 1 &&
               targetPositionVS.z > 0;
    }
    

    #endregion
    
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        switch (m_CurrentInputDeviceType.Value)
        {
            case InputDeviceType.Gamepad:
                OnDrawGizmosGamepad();
                break;
            case InputDeviceType.KeyboardMouse:
                OnDrawGizmosMouse();
                break;
        }
    }
    
    private void OnDrawGizmosGamepad()
    {
        if (m_InputGamepadDirection == Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_OriginPointInRadiusTargetCheck,m_RadiusDetectionGamepadNoInput);
        }
        else
        {
            Gizmos.color = Color.darkCyan;
            
            Gizmos.DrawWireSphere(m_PlayerController.Get().GetTargetPosition(),m_RadiusSphereCastGamepad);
            Gizmos.DrawLine(m_PlayerController.Get().GetTargetPosition(),m_PlayerController.Get().GetTargetPosition() + m_MaxDistanceSphereCast * m_InputGamepadDirectionRelativeToCamera);
            Gizmos.DrawWireSphere(m_PlayerController.Get().GetTargetPosition() + m_InputGamepadDirectionRelativeToCamera * m_MaxDistanceSphereCast, m_RadiusDirectTargetCheck);

            if (!m_SphereCastCheckValid)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(m_PlayerController.Get().GetTargetPosition(),m_RadiusDetectionGamepad);
            }
            else
            {
                Gizmos.color = m_TargetInRangeCheckValid ? Color.green : Color.red;
                Gizmos.DrawWireSphere(m_OriginPointInRadiusTargetCheck, m_RadiusDetectionGamepad);
                Gizmos.color = m_TargetDirectCheckValid ? Color.green : Color.red;
                float distanceToHit = Vector3.Distance(m_PlayerController.Get().GetTargetPosition(), m_HitPosition);
                Vector3 direction = (m_HitPosition - m_PlayerController.Get().GetTargetPosition()).normalized;
                Gizmos.DrawWireSphere(m_PlayerController.Get().GetTargetPosition(),m_RadiusSphereCastGamepad);
                Gizmos.DrawLine(m_PlayerController.Get().GetTargetPosition(),m_PlayerController.Get().GetTargetPosition() + distanceToHit * direction);
                Gizmos.DrawWireSphere(m_PlayerController.Get().GetTargetPosition() + direction * distanceToHit, m_RadiusDirectTargetCheck);
            }
        }
    }

    private void OnDrawGizmosMouse()
    {
        Gizmos.color = m_TargetInRangeCheckValid ? Color.green : Color.red;
        Gizmos.DrawWireSphere(m_OriginPointInRadiusTargetCheck, m_RadiusDetectionCursor);
        Gizmos.color = m_TargetDirectCheckValid ? Color.green : Color.red;
        float distanceToHit = Vector3.Distance(m_PlayerController.Get().GetTargetPosition(), m_HitPosition);
        Vector3 direction = (m_HitPosition - m_PlayerController.Get().GetTargetPosition()).normalized;
        Gizmos.DrawWireSphere(m_PlayerController.Get().GetTargetPosition(),m_RadiusSphereCastCursor);
        Gizmos.DrawLine(m_PlayerController.Get().GetTargetPosition(),m_PlayerController.Get().GetTargetPosition() + distanceToHit * direction);
        Gizmos.DrawWireSphere(m_PlayerController.Get().GetTargetPosition() + direction * distanceToHit, m_RadiusDirectTargetCheck);

    }
}