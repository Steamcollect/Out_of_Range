using UnityEngine;
using System.Collections.Generic;

public class FightDetectorManager : MonoBehaviour
{
    List<EntityController> enemys = new();
    List<WaveSystem> waveSystems = new();

    [SerializeField] RSE_OnFightStarted onFightStart;
    [SerializeField] RSE_OnFightEnded onFightEnd;

    public static FightDetectorManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OnWaveStart(WaveSystem waveSystem)
    {
        if (waveSystems.Contains(waveSystem)) return;

        CheckStartFight();
        waveSystems.Add(waveSystem);
    }

    public void OnWaveEnd(WaveSystem waveSystem)
    {
        waveSystems.Remove(waveSystem);
        CheckEndFight();
    }

    public void OnEnemyStartCombat(EntityController enemy)
    {
        CheckStartFight();

        enemys.Add(enemy);
        enemy.OnDeath += OnEnemyDie;
    }

    void OnEnemyDie(EntityController entity)
    {
        enemys.Remove(entity);
        CheckEndFight();
    }

    void CheckStartFight()
    {
        if(waveSystems.Count <= 0 && enemys.Count <= 0)
        {
            print("Fight Start");
            onFightStart.Call();
        }
    }

    void CheckEndFight()
    {
        if (waveSystems.Count <= 0 && enemys.Count <= 0)
        {
            print("Fight End");
            onFightEnd.Call();
        }
    }
}