using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class WaveSpawner : MonoBehaviour
{
    [Title("CONFIGURATION")]
    [ValueDropdown("@SSO_EnemyCatalog.GetDirectPrefabDropdown()")]
    [ListDrawerSettings(ShowIndexLabels = true, ShowItemCount = true, ShowFoldout = false)]
    [SerializeField] private List<GameObject> m_Wave = new List<GameObject>();

    [SerializeField] private GameObject m_SpawnFeedback;
    //[SerializeField] private float m_SpawnFeedbackDuration = 1.0f;
    
    public int ConfiguredWaveCount => m_Wave.Count;

    [Button]
    void SpawnFirstWave()
    {
        StartCoroutine(SpawnWave(0, null));
    }

    public IEnumerator SpawnWave(int waveIndex, System.Action<EntityController> onSpawnCallback)
    {
        if (waveIndex < 0 || waveIndex >= m_Wave.Count) yield break;

        GameObject content = m_Wave[waveIndex];
        if (content != null)
        {
            // SPAWN ENEMY
            if(content.TryGetComponent(out EntityController entity))
            {
                EntityController spawnedEntity = PoolManager.Instance.Spawn(entity, transform.position, transform.rotation);

                if (spawnedEntity.TryGetComponent(out ISpawnable spawnable)) spawnable.OnSpawn();
                onSpawnCallback?.Invoke(spawnedEntity);
            }
            else
            {
                PoolManager.Instance.Spawn(content, transform.position, transform.rotation);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, .4f);
    }
}