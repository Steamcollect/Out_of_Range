using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class CombatFightingDetection : MonoBehaviour
{
    [SerializeField] List<EntityController> entities = new();

    [SerializeField] UnityEvent OnEntitiesKilled;

    private void Start()
    {
        foreach (EntityController entity in entities)
        {
            entity.OnDeath += OnEntityKilled;
        }
    }

    void OnEntityKilled(EntityController entity)
    {
        entities.Remove(entity);

        if(entities.Count <= 0)
        {
            OnEntitiesKilled?.Invoke();
        }
    }
}
