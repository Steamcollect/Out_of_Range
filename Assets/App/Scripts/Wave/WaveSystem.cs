using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MVsToolkit.Utils;
using UnityEngine.Events;

public class WaveSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float timeBetweenWaves;
    int currentWave = 0;
    [HideInInspector] public bool IsInFight = false;

    [Header("References")]
    [SerializeField] WaveSpawner[] spawners;

    List<EntityController> currentEntitiesAlive = new();

    [System.Serializable]
    public struct WaveSpawner
    {
        public Transform spawnPoint;

        public EntityController[] entitiesPerWave;
    }

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] UnityEvent OnWavesStart;
    [SerializeField] UnityEvent OnWavesClear;

    public void SpawnWave()
    {
        int biggestWave = spawners.OrderByDescending(x => x.entitiesPerWave.Length).ToArray()[0].entitiesPerWave.Length;
        if (currentWave >= biggestWave)
        {
            FightDetectorManager.Instance?.OnWaveEnd(this);
            IsInFight = false;
            OnWavesClear?.Invoke();
            return;
        }
        else
        {
            FightDetectorManager.Instance?.OnWaveStart(this);
            IsInFight = true;
            OnWavesStart?.Invoke();
        }

        foreach (WaveSpawner spawner in spawners)
        {
            if (spawner.entitiesPerWave.Length <= currentWave
                || spawner.entitiesPerWave[currentWave] == null)
                continue;

            EntityController entity = Instantiate(
                spawner.entitiesPerWave[currentWave],
                spawner.spawnPoint.position,
                Quaternion.identity,
                transform);

            if (entity.TryGetComponent(out ISpawnable spawnable)) spawnable.OnSpawn();

            entity.transform.position = spawner.spawnPoint.position;
            entity.OnDeath += OnEntityDie;
            currentEntitiesAlive.Add(entity);
        }
    }

    void OnEntityDie(EntityController entity)
    {
        entity.OnDeath -= OnEntityDie;
        currentEntitiesAlive.Remove(entity);
        if(currentEntitiesAlive.Count <= 0)
        {
            currentWave++;

            CoroutineUtils.Delay(this, () =>
            {
                SpawnWave();
            }, timeBetweenWaves);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var spawner in spawners)
        {
            if (spawner.spawnPoint != null) Gizmos.DrawSphere(spawner.spawnPoint.position, .3f);
        }
    }
}