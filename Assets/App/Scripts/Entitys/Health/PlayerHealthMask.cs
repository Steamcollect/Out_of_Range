using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthMask : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private float m_DamageBlurDuration = 0.5f;
    [SerializeField] private Ease m_Ease = Ease.InOutSine;
    [SerializeField] private bool m_EffectUseTimeScale = true;
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
        
        m_CurrentTween = DOTween.To(x => m_BlurMaterial.SetFloat(s_IntensityProp, x), 
                0, 1f, m_DamageBlurDuration/2f)
            .SetUpdate(m_EffectUseTimeScale).SetLoops(2, LoopType.Yoyo).SetEase(m_Ease)
            .SetAutoKill(false);
        m_CurrentTween.Pause();
    }

    private void Start()
    {
        m_PlayerController.Get().GetHealth().OnTakeDamage += OnTakeDamage;
    }

    private void OnTakeDamage()
    {
       m_CurrentTween.Restart();
    }

    private void OnDestroy()
    {
        m_PlayerController.Get().GetHealth().OnTakeDamage -= OnTakeDamage;
        m_CurrentTween.Kill();
    }
}
