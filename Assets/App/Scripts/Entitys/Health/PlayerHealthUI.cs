using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("HEALTH")]
    [SerializeField] private Color m_HealthPointEnableColor;
    [SerializeField] private Color m_HealthPointDisableColor;
    [Space(5)]
    [SerializeField] private Transform m_HealthPointParentTransform;
    [SerializeField] private Image m_HealthPointImage;

    private List<Image> m_HealthPoints = new();

    [Header("Input")]
    [SerializeField] private RSO_PlayerController m_PlayerController;

    private void Start()
    {
        EntityHealth entityHealth = m_PlayerController.Get().GetHealth();
        entityHealth.OnTakeDamage += ()=>
        {
            SetHealthFillValue(entityHealth.GetCurrentHealth(), entityHealth.GetMaxHealth());
        };
        
        entityHealth.OnDeath+= ()=>
        {
            SetHealthFillValue(0, entityHealth.GetMaxHealth());
        };

        SetHealthFillValue(entityHealth.GetCurrentHealth(), entityHealth.GetMaxHealth());
    }

    public void SetHealthFillValue(int value, int max)
    {
        if (m_HealthPoints.Count != max)
        {
            foreach (Image point in m_HealthPoints)
            {
                Destroy(point.gameObject);
                m_HealthPoints.Clear();
            }

            for (int i = 0; i < max; i++)
            {
                m_HealthPoints.Add(Instantiate(m_HealthPointImage, m_HealthPointParentTransform));
                m_HealthPoints[i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < m_HealthPoints.Count; i++)
        {
            if (i + 1 <= value)
                m_HealthPoints[i].color = m_HealthPointEnableColor;
            else 
                m_HealthPoints[i].color = m_HealthPointDisableColor;
        }
    }
}