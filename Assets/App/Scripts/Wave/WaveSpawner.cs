using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Title("CONFIGURATION")]
    [ValueDropdown("@SSO_EnemyCatalog.GetDirectPrefabDropdown()")]
    [ListDrawerSettings(ShowIndexLabels = true, ShowItemCount = true, ShowFoldout = false)]
    [SerializeField] private List<GameObject> m_Wave = new List<GameObject>();

    [SerializeField] private GameObject m_SpawnFeedback;
    [SerializeField] private float m_SpawnFeedbackDuration = 1.0f;
    
    public int ConfiguredWaveCount => m_Wave.Count;

    public IEnumerator SpawnWave(int waveIndex, System.Action<EntityController> onSpawnCallback)
    {
        if (waveIndex < 0 || waveIndex >= m_Wave.Count) yield break;

        GameObject content = m_Wave[waveIndex];
        if (content != null)
        {
            // SPAWN ENEMY
            if(content.TryGetComponent<EntityController>(out EntityController entity))
            {
                GameObject spawnFeedback = Instantiate(m_SpawnFeedback, transform.position, transform.rotation);
                spawnFeedback.transform.localScale = Vector3.zero;
                float timer = 0f;
                
                while (timer < m_SpawnFeedbackDuration)
                {
                    timer += Time.deltaTime;
                    float progress = Mathf.Clamp01(timer / m_SpawnFeedbackDuration);
                    spawnFeedback.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                    yield return null;
                }
                
                Destroy(spawnFeedback);
                
                EntityController spawnedEntity = Instantiate(entity, transform.position, transform.rotation);

                if (spawnedEntity.TryGetComponent(out ISpawnable spawnable)) spawnable.OnSpawn();

                onSpawnCallback?.Invoke(spawnedEntity);
            }
            else //PLACEHOLDER POUR FAIRE SPAWN LES POWER UP, A CLEAN ? (Baptiste)
            {
                GameObject spawnFeedback = Instantiate(m_SpawnFeedback, transform.position, transform.rotation);
                spawnFeedback.transform.localScale = Vector3.zero;
                float timer = 0f;

                while (timer < m_SpawnFeedbackDuration)
                {
                    timer += Time.deltaTime;
                    float progress = Mathf.Clamp01(timer / m_SpawnFeedbackDuration);
                    spawnFeedback.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                    yield return null;
                }

                Destroy(spawnFeedback);

                Instantiate(content, transform.position, transform.rotation);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, .4f);
    }
}