using System;
using System.Collections.Generic;
using System.Linq;
using MVsToolkit.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class WaveSystem : MonoBehaviour
{
    [FormerlySerializedAs("timeBetweenWaves")]
    [Header("Settings")]
    [SerializeField] private float m_TimeBetweenWaves;

    [HideInInspector] public bool IsInFight;

    [FormerlySerializedAs("spawners")]
    [Header("References")]
    [SerializeField] private WaveSpawner[] m_Spawners;

    //[Header("Input")]
    [FormerlySerializedAs("OnWavesStart")]
    [Header("Output")]
    [SerializeField] private UnityEvent m_OnWavesStart;

    [FormerlySerializedAs("OnWavesClear")] [SerializeField] private UnityEvent m_OnWavesClear;

    private readonly List<EntityController> m_CurrentEntitiesAlive = new();

    private int m_CurrentWave;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (WaveSpawner spawner in m_Spawners)
            if (spawner.SpawnPoint != null)
                Gizmos.DrawSphere(spawner.SpawnPoint.position, .3f);
    }

    public void SpawnWave()
    {
        int biggestWave = m_Spawners.OrderByDescending(x => x.EntitiesPerWave.Length).ToArray()[0].EntitiesPerWave.Length;
        if (m_CurrentWave >= biggestWave)
        {
            FightDetectorManager.S_Instance?.OnWaveEnd(this);
            IsInFight = false;
            m_OnWavesClear?.Invoke();
            return;
        }

        FightDetectorManager.S_Instance?.OnWaveStart(this);
        IsInFight = true;
        m_OnWavesStart?.Invoke();

        foreach (WaveSpawner spawner in m_Spawners)
        {
            if (spawner.EntitiesPerWave.Length <= m_CurrentWave
                || spawner.EntitiesPerWave[m_CurrentWave] == null)
                continue;

            EntityController entity = Instantiate(
                spawner.EntitiesPerWave[m_CurrentWave],
                spawner.SpawnPoint.position,
                Quaternion.identity,
                transform);

            if (entity.TryGetComponent(out ISpawnable spawnable)) spawnable.OnSpawn();

            entity.transform.position = spawner.SpawnPoint.position;
            entity.OnDeath += OnEntityDie;
            m_CurrentEntitiesAlive.Add(entity);
        }
    }

    private void OnEntityDie(EntityController entity)
    {
        entity.OnDeath -= OnEntityDie;
        m_CurrentEntitiesAlive.Remove(entity);
        if (m_CurrentEntitiesAlive.Count <= 0)
        {
            m_CurrentWave++;

            this.Delay(() => { SpawnWave(); }, m_TimeBetweenWaves);
        }
    }

    [Serializable]
    public struct WaveSpawner
    {
        [FormerlySerializedAs("spawnPoint")] public Transform SpawnPoint;

        [FormerlySerializedAs("entitiesPerWave")] public EntityController[] EntitiesPerWave;
    }
}