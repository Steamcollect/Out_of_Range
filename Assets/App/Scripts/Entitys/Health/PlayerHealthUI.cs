using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color m_HealthPointEnableColor;
    [SerializeField] private Color m_HealthPointDisableColor;
    [SerializeField] private float m_HealthPointFadeDuration = 0.1f;
    [SerializeField] private bool m_EffectUseTimeScale = true;
    
    [Header("References")]
    [SerializeField] private Transform m_HealthPointParentTransform;
    [SerializeField] private Image m_HealthPointImage;

    [Header("Input")]
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private readonly List<Image> m_HealthPoints = new();
    private RectTransform m_HealthPointParentRectTransform;

    private void Awake() => m_HealthPointParentRectTransform = (RectTransform)m_HealthPointParentTransform;

    private void Start()
    {
        EntityHealth entityHealth = m_PlayerController.Get().GetHealth();
        entityHealth.OnTakeDamage += SetHealthFillValue;
        entityHealth.OnDeath += SetHealthFillValue;
        SetHealthFillValue();
    }
    
    private void OnDestroy()
    {
        EntityHealth entityHealth = m_PlayerController.Get().GetHealth();
        entityHealth.OnTakeDamage -= SetHealthFillValue;
        entityHealth.OnDeath -= SetHealthFillValue;
    }

    private void SetHealthFillValue()
    {
        
        int value = m_PlayerController.Get().GetHealth().GetCurrentHealth();
        int max = m_PlayerController.Get().GetHealth().GetMaxHealth();
        
        if (m_HealthPoints.Count != max)
        {
            foreach (Image point in m_HealthPoints)
            {
                Destroy(point.gameObject);
            }
            m_HealthPoints.Clear();

            for (int i = 0; i < max; i++)
            {
                m_HealthPoints.Add(Instantiate(m_HealthPointImage, m_HealthPointParentTransform));
                m_HealthPoints[i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < m_HealthPoints.Count; i++)
            m_HealthPoints[i].CrossFadeColor(i + 1 <= value ? m_HealthPointEnableColor : m_HealthPointDisableColor,
                m_HealthPointFadeDuration, m_EffectUseTimeScale, true);

        m_HealthPointParentRectTransform.DOPunchScale(Vector3.one * 0.2f,0.2f);
        
    }
}