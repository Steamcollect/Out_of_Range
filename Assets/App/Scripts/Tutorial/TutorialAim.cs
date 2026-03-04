using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TutorialAim : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The auto-aim component on the camera rig.")]
    [SerializeField] private CameraTargetAutoFocus m_AutoFocus;

    [Header("Targets")]
    [Tooltip("All dummies that must be aimed at to complete this tutorial step.")]
    [SerializeField] private List<TutorialAimTarget> m_RequiredTargets = new();

    [Header("Settings")]
    [Tooltip("How long (seconds) the player must hold aim on each target before it counts.")]
    [SerializeField] private float m_AimHoldDuration = 0.5f;

    [Tooltip("Once completed, the sequence does not reset even if re-enabled.")]
    [SerializeField] private bool m_CompleteOnce = true;

    [Header("Events")]
    [Tooltip("Fired once every required target has been aimed at long enough.")]
    [SerializeField] private UnityEvent m_OnAllTargetsAimed;

    [Tooltip("Fired each time a single target is validated (passes aimed target + its index).")]
    [SerializeField] private UnityEvent<int> m_OnTargetAimed;


    // How long the player has been continuously aiming at each target.
    private readonly Dictionary<TutorialAimTarget, float> m_AimTimers = new();

    // Targets that have been fully validated.
    private readonly HashSet<TutorialAimTarget> m_ValidatedTargets = new();

    // The target the auto-aim is currently locked onto.
    private ITargetable m_LastKnownTarget;

    private bool m_IsCompleted;
    private bool m_IsActive;

    public bool IsCompleted => m_IsCompleted;

    /// <summary>Number of required targets already validated.</summary>
    public int ValidatedCount => m_ValidatedTargets.Count;

    /// <summary>Total number of required targets.</summary>
    public int TotalCount => m_RequiredTargets.Count;


    private void Awake()
    {
        foreach (var dummy in m_RequiredTargets)
            m_AimTimers[dummy] = 0f;
    }
    private void Update()
    {
        ITargetable currentTarget = m_AutoFocus.GetCameraTarget();
        TrackAim(currentTarget);
        m_LastKnownTarget = currentTarget;
    }

    private void TrackAim(ITargetable currentTarget)
    {
        // 1. Trouver si l'objet visé est l'une de nos cibles de tutoriel
        TutorialAimTarget aimedDummy = null;
        if (currentTarget != null)
        {
            // On cherche dans la liste si l'un des composants correspond à l'ITargetable
            aimedDummy = m_RequiredTargets.Find(t => t.GetComponent<ITargetable>() == currentTarget);
        }

        // 2. Mettre à jour l'état de chaque cible
        foreach (var dummy in m_RequiredTargets)
        {
            if (m_ValidatedTargets.Contains(dummy)) continue;

            bool isCurrentlyAimed = (dummy == aimedDummy);
            dummy.SetAimedState(isCurrentlyAimed); //

            if (isCurrentlyAimed)
            {
                m_AimTimers[dummy] += Time.deltaTime; //

                if (m_AimTimers[dummy] >= m_AimHoldDuration)
                    ValidateTarget(dummy); //
            }
            else
            {
                // Reset le timer si on lâche la cible
                m_AimTimers[dummy] = 0f; //
            }
        }
    }

    private void ValidateTarget(TutorialAimTarget dummy)
    {
        if (m_ValidatedTargets.Contains(dummy)) return;

        m_ValidatedTargets.Add(dummy);
        dummy.SetAimedState(true);

        int index = m_RequiredTargets.IndexOf(dummy);
        m_OnTargetAimed?.Invoke(index);

        Debug.Log($"[TutorialAimSequence] Target validated: {dummy.name} ({m_ValidatedTargets.Count}/{m_RequiredTargets.Count})");

        if (m_ValidatedTargets.Count >= m_RequiredTargets.Count)
            CompleteSequence();
    }

    private void CompleteSequence()
    {
        m_IsCompleted = true;
        m_IsActive = false;

        Debug.Log("[TutorialAimSequence] All targets aimed at — sequence complete.");
        m_OnAllTargetsAimed?.Invoke();
    }

    public float GetTargetProgress(TutorialAimTarget target)
    {
        if (m_ValidatedTargets.Contains(target)) return 1f;
        if (m_AimTimers.TryGetValue(target, out float time))
        {
            return Mathf.Clamp01(time / m_AimHoldDuration);
        }
        return 0f;
    }
}
