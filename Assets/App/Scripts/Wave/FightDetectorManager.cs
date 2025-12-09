using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FightDetectorManager : RegularSingleton<FightDetectorManager>
{

    [Header("Output")]
    [SerializeField] private RSE_OnFightStarted m_OnFightStart;
    [SerializeField] private RSE_OnFightEnded m_OnFightEnd;
    private readonly List<EntityController> m_Enemies = new();
    private readonly List<WaveSystem> m_WaveSystems = new();

    public void OnWaveStart(WaveSystem waveSystem)
    {
        if (m_WaveSystems.Contains(waveSystem)) return;

        CheckStartFight();
        m_WaveSystems.Add(waveSystem);
    }

    public void OnWaveEnd(WaveSystem waveSystem)
    {
        m_WaveSystems.Remove(waveSystem);
        CheckEndFight();
    }

    public void OnEnemyStartCombat(EntityController enemy)
    {
        CheckStartFight();

        if (m_Enemies.Contains(enemy)) return;
        m_Enemies.Add(enemy);
        enemy.OnDeath += OnEnemyDie;
    }

    private void OnEnemyDie(EntityController entity)
    {
        m_Enemies.Remove(entity);
        CheckEndFight();
    }

    private void CheckStartFight()
    {
        if (m_WaveSystems.Count <= 0 && m_Enemies.Count <= 0) m_OnFightStart.Call();
    }

    private void CheckEndFight()
    {
        if (m_WaveSystems.Count <= 0 && m_Enemies.Count <= 0) m_OnFightEnd.Call();
    }
}