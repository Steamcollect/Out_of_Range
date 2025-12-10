using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// Plays an FMOD Event. Supports one-shot and instance-based playback, volume, pitch and local parameter overrides.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/FMOD Sound")]
    [FeedbackHelp("Plays an FMOD Event. You can choose OneShot (quick play) or Instance (create, set params, start). Volume and Pitch are supported and multiplied by feedback intensity.")]
    public class MMF_FMODSound : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        private static readonly bool s_FeedbackTypeAuthorized = true;

        [MMFInspectorGroup("Sound", true, 12, true)]
        [Tooltip("The FMOD event to play")] 
        public EventReference Event;

        [Tooltip("Play as a one-shot (no instance kept) or create an instance so we can set params/stop it later")] 
        public bool OneShot = true;

        [Tooltip("Whether to attach the instance to the owner GameObject (useful for 3D events)")]
        public bool AttachToOwner = true;

        [Tooltip("Base volume (linear, 1 = original)")]
        [Range(0f, 2f)]
        public float Volume = 1f;

        [Tooltip("Base pitch multiplier (1 = normal pitch)")]
        [Range(0.1f, 3f)]
        public float Pitch = 1f;

        [MMFInspectorGroup("Randomization", true, 13, true)]
        [Tooltip("If true, volume will be multiplied by a random value picked between RandomVolume.x and RandomVolume.y on play")]
        public bool RandomizeVolume = false;
        [Tooltip("Random volume multiplier range (min, max). Final volume = Volume * intensity * Random.Range(x,y) when RandomizeVolume is true")]
        public Vector2 RandomVolume = new(1f, 1f);

        [Tooltip("If true, pitch will be multiplied by a random value picked between RandomPitch.x and RandomPitch.y on play")]
        public bool RandomizePitch = false;
        [Tooltip("Random pitch multiplier range (min, max). Final pitch = Pitch * intensity * Random.Range(x,y) when RandomizePitch is true")]
        public Vector2 RandomPitch = new(1f, 1f);

        public FMOD.Studio.STOP_MODE StopMode = FMOD.Studio.STOP_MODE.IMMEDIATE;

        // internal instance (when OneShot == false or when we need advanced control)
        private EventInstance m_EventInstance;
        private bool m_InstanceCreated;

        // cached FMOD event length in seconds
        private float m_FMODDuration = 0f;

        /// <summary>
        /// Returns the duration of this feedback based on the FMOD event length when available
        /// </summary>
        public override float FeedbackDuration => m_FMODDuration;

        /// <summary>
        /// Custom initialization: compute FMOD event length if possible
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);
            ComputeFMODDuration();
        }

        /// <summary>
        /// In editor, try to compute duration on validate as well so the inspector shows progress correctly
        /// </summary>
        public override void OnValidate()
        {
            base.OnValidate();
            ComputeFMODDuration();
        }

        /// <summary>
        /// Plays the FMOD event, applying volume/pitch and parameters. Volume and pitch are multiplied by feedbacksIntensity.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !s_FeedbackTypeAuthorized || Event.IsNull) return;

            float computedIntensity = ComputeIntensity(feedbacksIntensity, position);

            float finalVolume = Volume * computedIntensity;
            
            if (RandomizeVolume)
            {
                float rv = UnityEngine.Random.Range(RandomVolume.x, RandomVolume.y);
                finalVolume *= rv;
            }

            float finalPitch = Pitch * computedIntensity;
            if (RandomizePitch)
            {
                float rp = UnityEngine.Random.Range(RandomPitch.x, RandomPitch.y);
                finalPitch *= rp;
            }
            
           
            if (OneShot)
            {
                RuntimeManager.PlayOneShot(Event, position);
            }
            else
            {
                // create instance to allow pitch/params/fade/volume overrides
                CreateAndStartInstance(position, finalVolume, finalPitch, computedIntensity);
            }
        }

        /// <summary>
        /// Creates an FMOD EventInstance, applies volume, pitch and parameters, attaches it if needed, starts it and optionally releases when finished.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="finalVolume"></param>
        /// <param name="finalPitch"></param>
        /// <param name="computedIntensity"></param>
        protected virtual void CreateAndStartInstance(Vector3 position, float finalVolume, float finalPitch, float computedIntensity)
        {
            // if an instance already exists, stop & release it first
            if (m_EventInstance.isValid())
            {
                m_EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                m_EventInstance.release();
            }

            m_EventInstance = RuntimeManager.CreateInstance(Event);

            m_EventInstance.setVolume(finalVolume);

            // set pitch - FMOD Studio's API offers setPitch
            m_EventInstance.setPitch(finalPitch);

            // // apply parameters
            // if (Parameters != null)
            // {
            //     foreach (var p in Parameters)
            //     {
            //         float value = p.Value;
            //         if (p.MultiplyWithIntensity)
            //         {
            //             value *= computedIntensity;
            //         }
            //         // set parameter by name
            //         m_EventInstance.setParameterByName(p.Name, value);
            //     }
            // }

            // attach to owner or set 3D attributes
            if (AttachToOwner)
            {
                RuntimeManager.AttachInstanceToGameObject(m_EventInstance, Owner.transform, Owner.transform);
            }
            else
            {
                // set 3D attributes based on world position
                var attrs = position.To3DAttributes();
                m_EventInstance.set3DAttributes(attrs);
            }

            m_EventInstance.start();
        }

        /// <summary>
        /// Stops the instance if one exists. Supports optional fade out over FadeOutDuration.
        /// </summary>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!m_EventInstance.isValid()) return;
            m_EventInstance.stop(StopMode);
        }
        

        /// <summary>
        /// When destroyed, make sure to release instance
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();

            if (!m_EventInstance.isValid()) return;
            m_EventInstance.stop(StopMode);
            m_EventInstance.release();
        }

        /// <summary>
        /// Attempts to compute the FMOD event length (in seconds) and caches it in _fmodDuration.
        /// </summary>
        protected virtual void ComputeFMODDuration()
        {
            m_FMODDuration = 0f;
            EventDescription desc = default;
            if (!Event.IsNull && String.IsNullOrEmpty(Event.Path))
            {
                desc = RuntimeManager.GetEventDescription(Event);
            }

            if (desc.isValid())
            {
                 var result = desc.getLength(out int lengthMs);
                 if (result == FMOD.RESULT.OK && lengthMs > 0)
                 {
                     m_FMODDuration = lengthMs / 1000f;
                 }
            }
        }
    }
}
