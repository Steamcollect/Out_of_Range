using System;
using UnityEngine.Rendering;

namespace GlitchEffect
{
    [Serializable]
    [VolumeComponentMenu("Glitch Effect")]
    public class GlitchEffectVolume : VolumeComponent
    {
        public ClampedFloatParameter scanLineJitter = new(0f, 0f, 1f);
        public ClampedFloatParameter horizontalShake = new(0f, 0f, 1f);
        public ClampedFloatParameter colorDrift = new(0f, 0f, 1f);
    }
}

