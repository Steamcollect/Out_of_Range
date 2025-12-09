using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CombatFightingDetection : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private List<EntityController> m_Entities = new();

    [Header("Output")]
    [SerializeField] private UnityEvent m_OnEntitiesKilled;

    private void Start()
    {
        foreach (EntityController entity in m_Entities)
            if (entity != null)
                entity.OnDeath += OnEntityKilled;
    }

    private void OnEntityKilled(EntityController entity)
    {
        m_Entities.Remove(entity);

        if (m_Entities.Count <= 0) m_OnEntitiesKilled?.Invoke();
    }
}