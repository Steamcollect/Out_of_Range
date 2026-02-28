using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthMask : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private DamageMaskEffect[] m_DamageMaskEffects;

    [Header("References")]
    [SerializeField] private Material m_MaskMaterial;
    [SerializeField] private Image m_BlurDamageImage;

    [Header("Input")] 
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private Material m_BlurMaterial;
    
    private void Awake()
    {
        // Create a new material instance, because image not support material property blocks
        m_BlurMaterial = new Material(m_MaskMaterial);
        m_BlurDamageImage.material = m_BlurMaterial;
    }

    private void Start()
    {
        m_PlayerController.Get().GetHealth().OnTakeDamage += UpdateEffect;
        m_PlayerController.Get().GetHealth().OnHeal += UpdateEffect;
        UpdateEffect();
    }

    private void UpdateEffect()
    {
        EntityHealth healthComp = m_PlayerController.Get().GetHealth();
        
        
        int healthDiff = Mathf.RoundToInt(healthComp.GetMaxHealth() - healthComp.GetCurrentHealth());
        int index = Mathf.Clamp(healthDiff, 0, m_DamageMaskEffects.Length - 1);
        
        ApplyEffect(index);
    }
    
    [Button]
    private void ApplyEffect(int index)
    {
        m_DamageMaskEffects[index].PlayEffect(in m_BlurMaterial);
    }

    private void OnDestroy()
    {
        m_PlayerController.Get().GetHealth().OnHeal -= UpdateEffect;
        m_PlayerController.Get().GetHealth().OnTakeDamage -= UpdateEffect;
    }
    
    [System.Serializable]
    private struct DamageMaskEffect
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