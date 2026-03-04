using MVsToolkit.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerPerfectHitMask : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PerfectHitMaskEffect[] m_PerfectHitMaskEffects;

    [Header("References")]
    [SerializeField] private Material m_MaskMaterial;
    [SerializeField] private Image m_BlurPerfectHitImage;

    [Header("Input")] 
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private Material m_BlurMaterial;
    private OverloadCombatStyle m_OverloadStyle;

    private void Awake()
    {
        m_BlurMaterial = new Material(m_MaskMaterial);
        m_BlurPerfectHitImage.material = m_BlurMaterial;
    }

    private void OnEnable()
    {
        m_PlayerController.Get().GetPlayerCombat().OnPrimaryCombatStyleChange += InitBindings;

        if (m_PlayerController.Get().GetPlayerCombat().GetPrimaryCombatStyle() != null)
            InitBindings();
    }

    private void OnDisable()
    {
        Unbind();
        m_PlayerController.Get().GetPlayerCombat().OnPrimaryCombatStyleChange -= InitBindings;
    }

    private void InitBindings()
    {
        BindToCombatStyle(m_PlayerController.Get().GetPlayerCombat().GetPrimaryCombatStyle());
    }

    private void BindToCombatStyle(CombatStyle style)
    {
        if (!style) return;

        Unbind();

        if (style is OverloadCombatStyle overloadStyle)
        {
            overloadStyle.OnPerfectHit += StartEffect;
            overloadStyle.OnPerfectHitEnd += StopEffect;
        }
        else
        {
            m_OverloadStyle = null;
        }
    }

    private void Unbind()
    {
        if (m_OverloadStyle)
        {
            m_OverloadStyle.OnPerfectHit -= StartEffect;
            m_OverloadStyle.OnPerfectHitEnd -= StopEffect;
        }

        m_OverloadStyle = null;
    }

    private void StartEffect()
    {
        ApplyEffect(1);
    }

    private void StopEffect()
    {
        ApplyEffect(0);
    }
    
    [Button]
    private void ApplyEffect(int index)
    {
        m_PerfectHitMaskEffects[index].PlayEffect(in m_BlurMaterial);
    }

    private void OnDestroy()
    {
        if (m_PlayerController.Value.GetPlayerCombat().GetPrimaryCombatStyle() is OverloadCombatStyle overloadStyle)
        {
            overloadStyle.OnPerfectHit += StartEffect;
            overloadStyle.OnPerfectHitEnd += StopEffect;
        }
    }
    
    [System.Serializable]
    private struct PerfectHitMaskEffect
    {
        public float Intensity;
        public float SmoothBorder;
        public float PowerDithering;
        public float SpeedWarning;
        public float Thickness;
        
        private static readonly  int s_IntensityProp = Shader.PropertyToID("_Intensity");
        private static readonly int s_SmoothnessProp = Shader.PropertyToID("_SmoothGradientBorder");
        private static readonly int s_PowerDitheringProp = Shader.PropertyToID("_PowerDithering");
        private static readonly int s_SpeedWarningProp = Shader.PropertyToID("_SpeedWarning");
        private static readonly int s_ThicknessProp = Shader.PropertyToID("_Thickness");

        public void PlayEffect(in Material material)
        {
            material.SetFloat(s_SmoothnessProp, SmoothBorder);
            material.SetFloat(s_IntensityProp, Intensity);
            material.SetFloat(s_PowerDitheringProp, PowerDithering);
            material.SetFloat(s_SpeedWarningProp, SpeedWarning);
            material.SetFloat(s_ThicknessProp, Thickness);
        }
    }
}