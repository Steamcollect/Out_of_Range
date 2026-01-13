using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// Activates an FMOD Snapshot for a specified duration. The snapshot is loaded when the feedback starts
    /// and unloaded when it ends. Supports fade in/out times and intensity control.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/FMOD Snapshot")]
    [FeedbackHelp("Activates an FMOD Snapshot. The snapshot will be loaded when the feedback plays and unloaded after the specified duration. Supports fade in/out and intensity control.")]
    public class MMF_FMODSnapshot : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        private static readonly bool s_FeedbackTypeAuthorized = true;

        #if UNITY_EDITOR
        public override Color FeedbackColor => MMFeedbacksInspectorColors.SoundsColor;
        public override bool HasAutomatedTargetAcquisition => false;
        #endif

        [MMFInspectorGroup("Snapshot", true, 12, true)]
        [Tooltip("The FMOD snapshot event to activate")]
        public EventReference SnapshotEvent;

        [Tooltip("The duration (in seconds) the snapshot will remain active before being unloaded. Set to 0 for infinite (manual stop required).")]
        [Min(0f)]
        public float Duration = 1f;

        [Tooltip("The intensity/weight of the snapshot (0 = no effect, 1 = full effect). This is multiplied by the feedback intensity.")]
        [Range(0f, 1f)]
        public float Intensity = 1f;

        [MMFInspectorGroup("Fade", true, 13)]
        [Tooltip("If true, the snapshot will fade in over FadeInDuration seconds")]
        public bool UseFadeIn = false;
        [Tooltip("Duration of the fade in (in seconds)")]
        [ShowIf("UseFadeIn")]
        [Min(0f)]
        public float FadeInDuration = 0.5f;

        [Tooltip("If true, the snapshot will fade out over FadeOutDuration seconds before being unloaded")]
        public bool UseFadeOut = false;
        [Tooltip("Duration of the fade out (in seconds)")]
        [ShowIf("UseFadeOut")]
        [Min(0f)]
        public float FadeOutDuration = 0.5f;

        [MMFInspectorGroup("Stop Settings", true, 14)]
        [Tooltip("How to stop the snapshot: IMMEDIATE stops instantly, ALLOWFADEOUT respects FMOD's built-in fade out")]
        public FMOD.Studio.STOP_MODE StopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT;

        // internal snapshot instance
        private EventInstance m_SnapshotInstance;
        private Coroutine m_DurationCoroutine;
        private Coroutine m_FadeCoroutine;

        /// <summary>
        /// Returns the total duration of this feedback (fade in + duration + fade out)
        /// </summary>
        public override float FeedbackDuration
        {
            get
            {
                float totalDuration = Duration;
                if (UseFadeIn) totalDuration += FadeInDuration;
                if (UseFadeOut) totalDuration += FadeOutDuration;
                return totalDuration;
            }
            set => Duration = value;
        }

        /// <summary>
        /// Plays the FMOD snapshot, applying intensity. Intensity is multiplied by feedbacksIntensity.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !s_FeedbackTypeAuthorized || SnapshotEvent.IsNull) return;

            float computedIntensity = ComputeIntensity(feedbacksIntensity, position);
            float finalIntensity = Intensity * computedIntensity;

            // Stop any existing coroutines and instance
            StopAllFeedbackCoroutines();
            ReleaseSnapshot();

            // Create and start the snapshot instance
            m_SnapshotInstance = RuntimeManager.CreateInstance(SnapshotEvent);

            if (!m_SnapshotInstance.isValid())
            {
                Debug.LogWarning($"[MMF_FMODSnapshot] Failed to create snapshot instance for {SnapshotEvent}");
                return;
            }

            if (UseFadeIn && FadeInDuration > 0f)
            {
                // Start with 0 intensity and fade in
                m_SnapshotInstance.setVolume(0f);
                m_SnapshotInstance.start();
                m_FadeCoroutine = Owner.StartCoroutine(FadeSnapshotCoroutine(0f, finalIntensity, FadeInDuration, OnFadeInComplete));
            }
            else
            {
                // Start at full intensity
                m_SnapshotInstance.setVolume(finalIntensity);
                m_SnapshotInstance.start();
                StartDurationTimer();
            }
        }

        /// <summary>
        /// Called when fade in completes
        /// </summary>
        private void OnFadeInComplete()
        {
            StartDurationTimer();
        }

        /// <summary>
        /// Starts the duration timer if Duration > 0
        /// </summary>
        private void StartDurationTimer()
        {
            if (Duration > 0f)
            {
                m_DurationCoroutine = Owner.StartCoroutine(DurationCoroutine());
            }
        }

        /// <summary>
        /// Coroutine that waits for the duration, then stops/fades out the snapshot
        /// </summary>
        private IEnumerator DurationCoroutine()
        {
            yield return new WaitForSeconds(Duration);
            m_DurationCoroutine = null;

            if (UseFadeOut && FadeOutDuration > 0f && m_SnapshotInstance.isValid())
            {
                // Get current volume for fade out
                m_SnapshotInstance.getVolume(out float currentVolume);
                m_FadeCoroutine = Owner.StartCoroutine(FadeSnapshotCoroutine(currentVolume, 0f, FadeOutDuration, ReleaseSnapshot));
            }
            else
            {
                ReleaseSnapshot();
            }
        }

        /// <summary>
        /// Coroutine that fades the snapshot volume from startVolume to endVolume over duration seconds
        /// </summary>
        private IEnumerator FadeSnapshotCoroutine(float startVolume, float endVolume, float duration, System.Action onComplete)
        {
            if (!m_SnapshotInstance.isValid())
            {
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float volume = Mathf.Lerp(startVolume, endVolume, t);

                if (m_SnapshotInstance.isValid())
                {
                    m_SnapshotInstance.setVolume(volume);
                }
                else
                {
                    yield break;
                }

                yield return null;
            }

            if (m_SnapshotInstance.isValid())
            {
                m_SnapshotInstance.setVolume(endVolume);
            }

            m_FadeCoroutine = null;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Stops any running coroutines related to this feedback
        /// </summary>
        private void StopAllFeedbackCoroutines()
        {
            if (m_DurationCoroutine != null && Owner != null)
            {
                Owner.StopCoroutine(m_DurationCoroutine);
                m_DurationCoroutine = null;
            }

            if (m_FadeCoroutine != null && Owner != null)
            {
                Owner.StopCoroutine(m_FadeCoroutine);
                m_FadeCoroutine = null;
            }
        }

        /// <summary>
        /// Stops and releases the snapshot instance
        /// </summary>
        private void ReleaseSnapshot()
        {
            if (!m_SnapshotInstance.isValid()) return;

            m_SnapshotInstance.stop(StopMode);
            m_SnapshotInstance.release();
            m_SnapshotInstance.clearHandle();
        }

        /// <summary>
        /// Stops the snapshot when the feedback is stopped
        /// </summary>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            StopAllFeedbackCoroutines();

            if (!m_SnapshotInstance.isValid()) return;

            if (UseFadeOut && FadeOutDuration > 0f)
            {
                m_SnapshotInstance.getVolume(out float currentVolume);
                m_FadeCoroutine = Owner.StartCoroutine(FadeSnapshotCoroutine(currentVolume, 0f, FadeOutDuration, ReleaseSnapshot));
            }
            else
            {
                ReleaseSnapshot();
            }
        }

        /// <summary>
        /// Resets the feedback to its initial state
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();
            StopAllFeedbackCoroutines();
            ReleaseSnapshot();
        }

        /// <summary>
        /// When destroyed, make sure to release the snapshot instance
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            StopAllFeedbackCoroutines();
            ReleaseSnapshot();
        }

        /// <summary>
        /// When disabled, stop and release the snapshot
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            StopAllFeedbackCoroutines();
            ReleaseSnapshot();
        }
    }
}