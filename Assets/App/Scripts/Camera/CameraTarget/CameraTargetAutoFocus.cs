using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class CameraTargetAutoFocus : MonoBehaviour, ICameraTarget
{
    #region Serialized Fields

    [Header("Layers")]
    [SerializeField] private LayerMask m_TargetLayer;
    [SerializeField] private LayerMask m_HittableLayer;
    [SerializeField] private LayerMask m_ObstructionLayer;
    [SerializeField] private QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("Detection")]
    [SerializeField] private float m_DetectionRadiusGamepad = 15f;
    [SerializeField] private float m_DetectionRadiusSphereCastGamepad = 20f;
    [SerializeField] private float m_DetectionRangeSphereCastGamepad = 30f;
    [SerializeField] private float m_DetectionRadiusMouse = 5f;

    [Header("Scoring Weights")]
    [SerializeField] private float m_WeightDistance = 1f;
    [SerializeField] private float m_WeightInputAlignment = 2f;
    [SerializeField] private float m_WeightPersistence = 3f;

    [Header("Stability")]
    [SerializeField] private float m_MinimumScoreToLock = 0.5f;
    [SerializeField] private float m_HysteresisRatio = 1.15f;
    [SerializeField] private float m_SwitchCooldown = 0.15f;

    [Header("Input")]
    [SerializeField] private InputActionReference m_MousePositionInput;
    [SerializeField] private InputActionReference m_DirectionInput;
    [SerializeField] private RSO_CurrentInputDeviceType m_CurrentInputDevice;

    [Header("References")]
    [SerializeField] private RSO_PlayerController m_PlayerController;
    [SerializeField] private RSO_PlayerCameraController m_CameraController;

    #endregion

    #region Private Fields

    private const int k_BufferSize = 16;
    private readonly Collider[] m_CandidateBuffer = new Collider[k_BufferSize*2];
    private readonly Collider[] m_OverlapBuffer = new Collider[k_BufferSize];
    private readonly RaycastHit[] m_SphereCastBuffer = new RaycastHit[k_BufferSize];

    private ITargetable m_CurrentTarget;
    private float m_CurrentScore;
    private float m_LastSwitchTime;

    // Debug
    private Vector3 m_DebugOrigin;
    private Vector3 m_DebugHitPoint;
    private Vector3 m_DebugInputDirection;
    private bool m_DebugHasHit;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        m_MousePositionInput.action.Enable();
        m_DirectionInput.action.Enable();
    }

    private void OnDisable()
    {
        m_MousePositionInput.action.Disable();
        m_DirectionInput.action.Disable();
    }

    #endregion

    #region ICameraTarget

    public Vector3? GetCameraTargetPosition()
    {
        ResetDebug();

        return m_CurrentInputDevice.Value switch
        {
            InputDeviceType.Gamepad => ResolveGamepadTarget(),
            InputDeviceType.KeyboardMouse => ResolveMouseTarget(),
            _ => null
        };
    }

    public ITargetable GetCameraTarget()
    {
        return m_CurrentTarget;
    }

    #endregion

    #region Mouse Logic

    private Vector3? ResolveMouseTarget()
    {
        Camera cam = m_CameraController.Get().GetCamera();
        Vector2 screenPos = m_MousePositionInput.action.ReadValue<Vector2>();

        if (!Physics.Raycast(
                cam.ScreenPointToRay(screenPos),
                out RaycastHit hit,
                Mathf.Infinity,
                m_HittableLayer,
                m_QueryTriggerInteraction))
        {
            return null;
        }

        m_DebugHasHit = true;
        m_DebugHitPoint = hit.point;
        m_DebugOrigin = hit.point;

        int count = Physics.OverlapSphereNonAlloc(
            hit.point,
            m_DetectionRadiusMouse,
            m_CandidateBuffer,
            m_TargetLayer,
            m_QueryTriggerInteraction);

        ITargetable closest = null;
        float minDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (!m_CandidateBuffer[i].TryGetComponent(out ITargetable target))
                continue;

            Vector3 targetPos = target.GetTargetPosition();
            float dist = Vector3.Distance(hit.point, targetPos);

            if (!IsTargetVisible(m_PlayerController.Get().GetTargetPosition(), targetPos))
                continue;

            if (dist < minDist)
            {
                minDist = dist;
                closest = target;
            }
        }

        return closest?.GetTargetPosition() ?? hit.point;
    }

    #endregion

    #region Gamepad Logic

    private Vector3? ResolveGamepadTarget()
    {
        Vector3 playerPos = m_PlayerController.Get().GetTargetPosition();
        Vector3 inputDir = GetInputDirectionRelativeToCamera();

        m_DebugOrigin = playerPos;
        m_DebugInputDirection = inputDir;

        int overlapSphereCount = Physics.OverlapSphereNonAlloc(
            playerPos,
            m_DetectionRadiusGamepad,
            m_OverlapBuffer,
            m_TargetLayer,
            m_QueryTriggerInteraction);
        
        int sphereCastCount = 0;
        
        if (inputDir != Vector3.zero)
        {
            sphereCastCount  = Physics.SphereCastNonAlloc(
                playerPos,
                m_DetectionRadiusSphereCastGamepad,
                inputDir.normalized,
                m_SphereCastBuffer,
                m_DetectionRangeSphereCastGamepad,
                m_TargetLayer,
                m_QueryTriggerInteraction);
        }

        ITargetable best = null;
        float bestScore = float.MinValue;

        int count = sphereCastCount + overlapSphereCount;
        Array.Clear(m_CandidateBuffer,0,m_CandidateBuffer.Length);
        if (overlapSphereCount > 0)
        {
            Array.Copy(m_OverlapBuffer, 0, m_CandidateBuffer, 0, overlapSphereCount);
        }
        for (int i = 0; i < sphereCastCount; i++)
        {
            m_CandidateBuffer[overlapSphereCount + i] = m_SphereCastBuffer[i].collider;
        }
        
        if (count > 0)
        {
            best = SelectBestTarget(playerPos, inputDir, count,m_CandidateBuffer, out bestScore);
        }

        if (IsValidTarget(best, bestScore))
        {
            UpdateTargetState(best, bestScore);
            if (best != null) return best.GetTargetPosition();
        }

        Debug.Log("No valid target found, falling back to default behavior.");
        ClearTargetState();
        return ComputeFallback(playerPos, inputDir);
    }

    #endregion

    #region Selection & Scoring

    private ITargetable SelectBestTarget(
        Vector3 playerPos,
        Vector3 inputDir,
        int count,Collider[] buffer,
        out float bestScore)
    {
        bestScore = float.MinValue;
        ITargetable best = null;

        for (int i = 0; i < count; i++)
        {
            if (!buffer[i].TryGetComponent(out ITargetable target))
                continue;

            Vector3 targetPos = target.GetTargetPosition();

            if (!IsTargetVisible(playerPos, targetPos))
                continue;

            float score = ComputeScore(playerPos, targetPos, inputDir, target);

            if (score > bestScore)
            {
                bestScore = score;
                best = target;
            }
        }

        return best;
    }

    private float ComputeScore(
        Vector3 playerPos,
        Vector3 targetPos,
        Vector3 inputDir,
        ITargetable target)
    {
        float score = 0f;

        Vector3 toTarget = targetPos - playerPos;
        float distance = toTarget.magnitude;
        Vector3 dir = toTarget.normalized;

        score += (1f / (1f + distance)) * m_WeightDistance;

        if (inputDir != Vector3.zero)
        {
            float alignment = Vector3.Dot(inputDir.normalized, dir);
            score += Mathf.Max(0f, alignment) * m_WeightInputAlignment;
        }

        if (target == m_CurrentTarget)
            score += m_WeightPersistence;

        return score;
    }

    #endregion

    #region Validation

    private bool IsValidTarget(ITargetable target, float score)
    {
        if (target == null) return false;
        if (score < m_MinimumScoreToLock) return false;

        if (m_CurrentTarget != null && target != m_CurrentTarget)
        {
            if (score < m_CurrentScore * m_HysteresisRatio) return false;
            if (Time.time - m_LastSwitchTime < m_SwitchCooldown) return false;
        }

        return true;
    }

    private void UpdateTargetState(ITargetable target, float score)
    {
        if (target != m_CurrentTarget)
            m_LastSwitchTime = Time.time;

        m_CurrentTarget = target;
        m_CurrentScore = score;
    }

    private void ClearTargetState()
    {
        m_CurrentTarget = null;
        m_CurrentScore = 0f;
    }

    #endregion

    #region Helpers

    private bool IsTargetVisible(Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from).normalized;
        float dist = Vector3.Distance(from, to);

        if (Physics.Raycast(from, dir, dist, m_ObstructionLayer, m_QueryTriggerInteraction))
            return false;

        Vector3 vp = m_CameraController.Get().GetCamera().WorldToViewportPoint(to);

        return vp.x is >= 0 and <= 1 &&
               vp.y is >= 0 and <= 1 &&
               vp.z > 0;
    }

    private Vector3 GetInputDirectionRelativeToCamera()
    {
        Vector2 raw = m_DirectionInput.action.ReadValue<Vector2>();
        if (raw == Vector2.zero)
            return Vector3.zero;

        Vector3 input = new(raw.x, 0f, raw.y);
        return input.DirectionRelativeToCamera();
    }

    private Vector3? ComputeFallback(Vector3 playerPos, Vector3 inputDir)
    {
        if (inputDir != Vector3.zero)
        {
            return playerPos + inputDir.normalized;
        }
            

        Vector3 velocity = m_PlayerController.Value.Velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.001f)
            return playerPos + velocity.normalized;

        return null;
    }

    private void ResetDebug()
    {
        m_DebugHasHit = false;
        m_DebugInputDirection = Vector3.zero;
        m_DebugOrigin = Vector3.zero;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        if (m_CurrentInputDevice.Value == InputDeviceType.Gamepad)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(m_DebugOrigin, m_DetectionRadiusGamepad);

            if (m_DebugInputDirection != Vector3.zero)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(m_DebugOrigin, m_DebugOrigin + m_DebugInputDirection * m_DetectionRangeSphereCastGamepad);
                Gizmos.DrawWireSphere(m_DebugOrigin + m_DebugInputDirection * m_DetectionRangeSphereCastGamepad, m_DetectionRadiusSphereCastGamepad);
            }
        }
        else
        {
            if (m_DebugHasHit)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(m_DebugHitPoint, m_DetectionRadiusMouse);
            }
        }
    }

    #endregion
}
