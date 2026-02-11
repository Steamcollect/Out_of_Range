using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthMask : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private float m_DamageBlurDuration = 0.5f;
    [SerializeField] private Ease m_Ease = Ease.OutQuad;
    [SerializeField] private bool m_EffectUseTimeScale = true;
    [SerializeField] float[] intensityPerHealthPoint;

    [Header("References")]
    [SerializeField] private Image m_BlurDamageImage;

    [Header("Input")] 
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private Material m_BlurMaterial;
    
    private static readonly  int s_IntensityProp = Shader.PropertyToID("_Intensity");
    
    private Tween m_CurrentTween;
    
    private void Awake()
    {
        m_BlurMaterial = new Material(m_BlurDamageImage.material);
        m_BlurDamageImage.material = m_BlurMaterial;
        m_BlurMaterial.SetFloat(s_IntensityProp, 0f);
    }

    private void Start()
    {
        m_PlayerController.Get().GetHealth().OnTakeDamage += OnTakeDamage;
    }

    private void OnTakeDamage()
    {
        var health = m_PlayerController.Get().GetHealth();
        float targetIntensity = intensityPerHealthPoint[(int)health.GetCurrentHealth()];

        m_CurrentTween?.Kill();

        Sequence dmgSequence = DOTween.Sequence();

        dmgSequence.Append(DOTween.To(() => m_BlurMaterial.GetFloat(s_IntensityProp),
            x => m_BlurMaterial.SetFloat(s_IntensityProp, x), 1f, m_DamageBlurDuration * 0.2f)
            .SetEase(Ease.OutExpo));

        dmgSequence.Append(DOTween.To(() => m_BlurMaterial.GetFloat(s_IntensityProp),
            x => m_BlurMaterial.SetFloat(s_IntensityProp, x), targetIntensity, m_DamageBlurDuration * 0.8f)
            .SetEase(m_Ease));

        dmgSequence.SetUpdate(m_EffectUseTimeScale);
        m_CurrentTween = dmgSequence;
    }

    private void OnDestroy()
    {
        m_PlayerController.Get().GetHealth().OnTakeDamage -= OnTakeDamage;
        m_CurrentTween.Kill();
    }
}