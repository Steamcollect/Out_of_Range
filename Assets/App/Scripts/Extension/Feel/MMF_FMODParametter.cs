using System.Collections;
using FMODUnity;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback lets you modify FMOD parameters over time - globally, on specific event instances, or on buses.
    /// Supports interpolation modes, yoyo, and various curve types.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/FMOD Parameter")]
    [FeedbackHelp("Modifies an FMOD parameter value over time. Can target global parameters, specific event instances, or bus volume. Supports various interpolation modes including yoyo.")]
    public class MMF_FMODParameter : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        private static readonly bool s_FeedbackTypeAuthorized = true;

        #if UNITY_EDITOR
        public override Color FeedbackColor => MMFeedbacksInspectorColors.SoundsColor;
        public override bool HasAutomatedTargetAcquisition => false;
        #endif

        /// <summary>
        /// The target type for the parameter modification
        /// </summary>
        public enum ParameterTargetType
        {
            /// Modify a global FMOD parameter
            Global,
            /// Modify a parameter on a specific EventInstance
            EventInstance,
            /// Modify the volume on a Bus
            Bus
        }

        /// <summary>
        /// The mode of parameter modification
        /// </summary>
        public enum ParameterMode
        {
            /// Set the value instantly
            Instant,
            /// Interpolate the value over time
            OverTime,
            /// Interpolate from current value to a destination
            ToDestination
        }

        /// <summary>
        /// The playback mode for time-based interpolation
        /// </summary>
        public enum PlaybackMode
        {
            /// Play once from start to end
            Normal,
            /// Play forward then backward (yoyo effect)
            Yoyo,
            /// Loop the animation
            Loop
        }

        // ================ TARGET SETTINGS ================
        [MMFInspectorGroup("Target", true, 12, true)]
        
        [Tooltip("The type of target to modify: Global parameter, EventInstance parameter, or Bus volume")]
        public ParameterTargetType TargetType = ParameterTargetType.Global;

        [Tooltip("The name of the global parameter to modify")]
        [ShowIf("@TargetType == ParameterTargetType.Global")]
        [ParamRef]
        public string GlobalParameterName;

        [Tooltip("The StudioEventEmitter containing the event instance to modify")]
        [ShowIf("@TargetType == ParameterTargetType.EventInstance")]
        public StudioEventEmitter TargetEmitter;

        [Tooltip("The name of the parameter to modify on the event instance")]
        [ShowIf("@TargetType == ParameterTargetType.EventInstance")]
        public string InstanceParameterName;

        [Tooltip("The FMOD Bus to modify (for volume control)")]
        [ShowIf("@TargetType == ParameterTargetType.Bus")]
        public SSO_FMODBus TargetBus;

        // ================ VALUE SETTINGS ================
        [MMFInspectorGroup("Parameter Value", true, 13)]

        [Tooltip("The mode of parameter modification")]
        public ParameterMode Mode = ParameterMode.OverTime;

        [Tooltip("The value to set instantly")]
        [ShowIf("@Mode == ParameterMode.Instant")]
        public float InstantValue = 1f;

        [Tooltip("The duration of the interpolation in seconds")]
        [ShowIf("@Mode != ParameterMode.Instant")]
        [Min(0.01f)]
        public float Duration = 1f;

        [Tooltip("The value at the start of the interpolation (curve time 0)")]
        [ShowIf("@Mode == ParameterMode.OverTime")]
        public float RemapZero = 0f;

        [Tooltip("The value at the end of the interpolation (curve time 1)")]
        [ShowIf("@Mode == ParameterMode.OverTime")]
        public float RemapOne = 1f;

        [Tooltip("The destination value to interpolate to (from current value)")]
        [ShowIf("@Mode == ParameterMode.ToDestination")]
        public float DestinationValue = 1f;

        [Tooltip("The curve to use for interpolation")]
        [ShowIf("@Mode != ParameterMode.Instant")]
        public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

        // ================ PLAYBACK SETTINGS ================
        [MMFInspectorGroup("Playback", true, 14)]

        [Tooltip("The playback mode: Normal (one-shot), Yoyo (ping-pong), or Loop")]
        [ShowIf("@Mode != ParameterMode.Instant")]
        public PlaybackMode Playback = PlaybackMode.Normal;

        [Tooltip("Number of complete cycles (forward+backward = 1 cycle for Yoyo). 0 = infinite for Loop mode.")]
        [ShowIf("@Mode != ParameterMode.Instant && Playback != PlaybackMode.Normal")]
        [Min(0)]
        public int Cycles = 1;

        [Tooltip("If true, new plays are allowed even if a coroutine is already running")]
        [ShowIf("@Mode != ParameterMode.Instant")]
        public bool AllowAdditivePlays = false;

        [Tooltip("If true, FMOD will ignore seek speed and set parameter instantly (no internal smoothing)")]
        public bool IgnoreSeekSpeed = false;

        // ================ INTERNAL STATE ================
        private Coroutine m_Coroutine;
        private float m_InitialValue;
        private FMOD.Studio.PARAMETER_DESCRIPTION m_GlobalParamDescription;
        private bool m_GlobalParamDescriptionCached;

        /// <summary>
        /// The duration of this feedback
        /// </summary>
        public override float FeedbackDuration
        {
            get
            {
                if (Mode == ParameterMode.Instant) return 0f;
                
                float baseDuration = ApplyTimeMultiplier(Duration);
                if (Playback == PlaybackMode.Yoyo)
                    return baseDuration * 2f * Mathf.Max(1, Cycles);
                if (Playback == PlaybackMode.Loop && Cycles > 0)
                    return baseDuration * Cycles;
                return baseDuration;
            }
            set => Duration = value;
        }

        /// <summary>
        /// Plays the feedback, modifying the FMOD parameter
        /// </summary>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !s_FeedbackTypeAuthorized) return;

            if (!ValidateTarget()) return;

            switch (Mode)
            {
                case ParameterMode.Instant:
                    SetParameterValue(InstantValue);
                    break;

                case ParameterMode.OverTime:
                case ParameterMode.ToDestination:
                    if (!AllowAdditivePlays && m_Coroutine != null)
                        return;

                    if (m_Coroutine != null)
                        Owner.StopCoroutine(m_Coroutine);

                    m_InitialValue = GetCurrentParameterValue();
                    m_Coroutine = Owner.StartCoroutine(ParameterSequence());
                    break;
            }
        }

        /// <summary>
        /// Validates that the target is properly configured
        /// </summary>
        private bool ValidateTarget()
        {
            switch (TargetType)
            {
                case ParameterTargetType.Global:
                    return !string.IsNullOrEmpty(GlobalParameterName);

                case ParameterTargetType.EventInstance:
                    return TargetEmitter != null && 
                           TargetEmitter.EventInstance.isValid() && 
                           !string.IsNullOrEmpty(InstanceParameterName);

                case ParameterTargetType.Bus:
                    return TargetBus != null && TargetBus.Bus.isValid();

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the current value of the targeted parameter
        /// </summary>
        private float GetCurrentParameterValue()
        {
            float value = 0f;

            switch (TargetType)
            {
                case ParameterTargetType.Global:
                    RuntimeManager.StudioSystem.getParameterByName(GlobalParameterName, out value);
                    break;

                case ParameterTargetType.EventInstance:
                    if (TargetEmitter != null && TargetEmitter.EventInstance.isValid())
                        TargetEmitter.EventInstance.getParameterByName(InstanceParameterName, out value);
                    break;

                case ParameterTargetType.Bus:
                    if (TargetBus != null && TargetBus.Bus.isValid())
                        TargetBus.Bus.getVolume(out value);
                    break;
            }

            return value;
        }

        /// <summary>
        /// Sets the parameter value on the appropriate target
        /// </summary>
        private void SetParameterValue(float value)
        {
            switch (TargetType)
            {
                case ParameterTargetType.Global:
                    CacheGlobalParameterDescription();
                    if (m_GlobalParamDescriptionCached)
                    {
                        RuntimeManager.StudioSystem.setParameterByID(m_GlobalParamDescription.id, value, IgnoreSeekSpeed);
                    }
                    else
                    {
                        RuntimeManager.StudioSystem.setParameterByName(GlobalParameterName, value, IgnoreSeekSpeed);
                    }
                    break;

                case ParameterTargetType.EventInstance:
                    if (TargetEmitter != null && TargetEmitter.EventInstance.isValid())
                        TargetEmitter.EventInstance.setParameterByName(InstanceParameterName, value, IgnoreSeekSpeed);
                    break;

                case ParameterTargetType.Bus:
                    if (TargetBus != null && TargetBus.Bus.isValid())
                        TargetBus.Bus.setVolume(value);
                    break;
            }
        }

        /// <summary>
        /// Caches the global parameter description for efficient ID-based access
        /// </summary>
        private void CacheGlobalParameterDescription()
        {
            if (m_GlobalParamDescriptionCached) return;
            if (string.IsNullOrEmpty(GlobalParameterName)) return;

            var result = RuntimeManager.StudioSystem.getParameterDescriptionByName(GlobalParameterName, out m_GlobalParamDescription);
            m_GlobalParamDescriptionCached = (result == FMOD.RESULT.OK);
        }

        /// <summary>
        /// Coroutine that handles the parameter interpolation over time
        /// </summary>
        private IEnumerator ParameterSequence()
        {
            IsPlaying = true;
            float feedbackDuration = ApplyTimeMultiplier(Duration);
            int completedCycles = 0;
            bool goingForward = true;

            while (true)
            {
                float journey = goingForward ? 0f : feedbackDuration;

                // Run one pass
                while ((goingForward && journey <= feedbackDuration) || (!goingForward && journey >= 0f))
                {
                    float normalizedTime = Mathf.Clamp01(journey / feedbackDuration);
                    float curveValue = MMTween.Tween(normalizedTime, 0f, 1f, 0f, 1f, Curve);

                    float newValue;
                    if (Mode == ParameterMode.ToDestination)
                    {
                        newValue = Mathf.LerpUnclamped(m_InitialValue, DestinationValue, curveValue);
                    }
                    else
                    {
                        newValue = Mathf.LerpUnclamped(RemapZero, RemapOne, curveValue);
                    }

                    SetParameterValue(newValue);

                    journey += goingForward ? FeedbackDeltaTime : -FeedbackDeltaTime;
                    yield return null;
                }

                // Set final value for this pass
                float finalValue;
                if (Mode == ParameterMode.ToDestination)
                {
                    finalValue = goingForward ? DestinationValue : m_InitialValue;
                }
                else
                {
                    finalValue = goingForward ? RemapOne : RemapZero;
                }
                SetParameterValue(finalValue);

                // Handle playback modes
                switch (Playback)
                {
                    case PlaybackMode.Normal:
                        // Done after one pass
                        goto EndSequence;

                    case PlaybackMode.Yoyo:
                        if (goingForward)
                        {
                            goingForward = false;
                        }
                        else
                        {
                            goingForward = true;
                            completedCycles++;
                            if (Cycles > 0 && completedCycles >= Cycles)
                                goto EndSequence;
                        }
                        break;

                    case PlaybackMode.Loop:
                        completedCycles++;
                        if (Cycles > 0 && completedCycles >= Cycles)
                            goto EndSequence;
                        // Reset m_InitialValue for ToDestination mode on loop
                        if (Mode == ParameterMode.ToDestination)
                            m_InitialValue = GetCurrentParameterValue();
                        break;
                }
            }

            EndSequence:
            m_Coroutine = null;
            IsPlaying = false;
        }

        /// <summary>
        /// Stops the feedback
        /// </summary>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !s_FeedbackTypeAuthorized) return;

            if (m_Coroutine != null)
            {
                Owner.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            IsPlaying = false;
        }

        /// <summary>
        /// Restores the initial value
        /// </summary>
        protected override void CustomRestoreInitialValues()
        {
            if (!Active || !s_FeedbackTypeAuthorized) return;

            if (Mode != ParameterMode.Instant)
            {
                SetParameterValue(m_InitialValue);
            }
        }

        /// <summary>
        /// Cleanup on destroy
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();

            if (m_Coroutine != null && Owner != null)
            {
                Owner.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
        }
    }
}