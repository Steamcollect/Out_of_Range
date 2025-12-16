using System.Collections.Generic;
using MVsToolkit.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class WaveSystem : MonoBehaviour
{
    [Title("SETTINGS")]
    [SerializeField] private float m_TimeBetweenWaves = 3.0f;
    [ShowInInspector, ReadOnly] private bool IsInFight;

    [Title("REFERENCES")]
    [SerializeField] private List<WaveSpawner> m_Spawners = new List<WaveSpawner>();

    [Title("EVENTS")]
    [SerializeField] private UnityEvent m_OnCombatStart;
    [SerializeField] private UnityEvent m_OnCombatCompleted;
    [SerializeField] private UnityEvent m_OnWaveEnd;
    
    
    // State
    public List<EntityController> m_CurrentEntitiesAlive = new List<EntityController>();
    public int m_CurrentWaveIndex;
    public int m_MaxWaveCount;

    private void Start()
    {
        m_MaxWaveCount = 0;
        foreach(WaveSpawner spawner in m_Spawners)
        {
            if(spawner.ConfiguredWaveCount > m_MaxWaveCount) m_MaxWaveCount = spawner.ConfiguredWaveCount;
        }
    }

    [Button("Start Combat")]
    public void StartCombat()
    {
        if (IsInFight) return;
        m_CurrentWaveIndex = 0;
        IsInFight = true;

        FightDetectorManager.Instance?.OnWaveStart(this);
        m_OnCombatStart?.Invoke();

        SpawnCurrentWave();
    }

    private void SpawnCurrentWave()
    {
        Debug.Log($"Lancement de la Wave {m_CurrentWaveIndex + 1}");

        foreach(WaveSpawner spawner in m_Spawners)
        {
            StartCoroutine(spawner.SpawnWave(m_CurrentWaveIndex, RegisterEntity));
        }

        //this.Delay(() =>
        //{
        //    if (m_CurrentEntitiesAlive.Count == 0 && IsInFight) OnWaveComplete();
        //}, .1f);
    }

    private void RegisterEntity(EntityController entity)
    {
        entity.OnDeath += OnEntityDie;
        m_CurrentEntitiesAlive.Add(entity);
    }

    private void OnEntityDie(EntityController entity)
    {
        entity.OnDeath -= OnEntityDie;
        m_CurrentEntitiesAlive.Remove(entity);

        if (m_CurrentEntitiesAlive.Count <= 0)
        {
            OnWaveComplete();
        }
    }

    private void OnWaveComplete()
    {
        m_CurrentWaveIndex++;
        m_OnWaveEnd.Invoke();

        if (m_CurrentWaveIndex >= m_MaxWaveCount)
        {
            EndCombat();
        }
        else
        {
            Debug.Log($"Wave termin�e. Prochaine wave dans {m_TimeBetweenWaves} secondes...");

            this.Delay(() =>
            {
                SpawnCurrentWave();
            }, m_TimeBetweenWaves);
        }
    }

    private void EndCombat()
    {
        IsInFight = false;
        FightDetectorManager.Instance?.OnWaveEnd(this);
        m_OnCombatCompleted?.Invoke();
    }

#if UNITY_EDITOR
    [Button("Auto-Find Spawners")]
    private void FindSpawners() => m_Spawners = new List<WaveSpawner>(GetComponentsInChildren<WaveSpawner>());

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (WaveSpawner spawner in m_Spawners)
            Gizmos.DrawSphere(spawner.transform.position, .5f);
    }
#endif
}