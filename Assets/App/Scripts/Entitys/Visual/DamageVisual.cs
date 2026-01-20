using System;
using MVsToolkit.Dev;
using UnityEngine;
    using UnityEngine.VFX;
    
    public class DamageVisual : MonoBehaviour
    {
        private static readonly int s_DamageAmount = Shader.PropertyToID("_DamageAmount");
        private static readonly int s_VfxIntensity = Shader.PropertyToID("Intensity");
    
        [Header("Settings")]
        [SerializeField] private bool m_UseSmokeVfx = true;
        
        [Header("References")]
        [SerializeField] private EntityHealth m_EntityHealth;
        [SerializeField, ShowIf("m_UseSmokeVfx",true)] private VisualEffect m_SmokeDamage;
        [SerializeField] private MeshRenderer[] m_Renderers;
        private MaterialPropertyBlock m_Block;
    
        private void Awake()
        {
            m_Block = new MaterialPropertyBlock();
        }

        private void Start() => HandleTakeDamage();

        private void OnEnable()
        {
            if (!m_EntityHealth) return;
            m_EntityHealth.OnTakeDamage += HandleTakeDamage;
        }
    
        private void OnDisable()
        {
            if (!m_EntityHealth) return;
            m_EntityHealth.OnTakeDamage -= HandleTakeDamage;
        }
    
        private void Update()
        {
            float value = 1f - (m_EntityHealth ? m_EntityHealth.GetHealthPercentage() : 1f);
            SetDamage(value);
        }
    
        private void HandleTakeDamage()
        {
            if (!m_EntityHealth) return;
            SetDamage(1f - m_EntityHealth.GetHealthPercentage());
        }
    
        private void SetDamage(float value)
        {
            value = Mathf.Clamp01(value);
    
            if (m_Renderers != null)
            {
                foreach (MeshRenderer rend in m_Renderers)
                {
                    if (!rend) continue;
                    rend.GetPropertyBlock(m_Block);
                    m_Block.SetFloat(s_DamageAmount, value);
                    rend.SetPropertyBlock(m_Block);
                }
            }
    
            if (m_UseSmokeVfx && m_SmokeDamage)
            {
                m_SmokeDamage.SetFloat(s_VfxIntensity, value);
            }
        }
    }