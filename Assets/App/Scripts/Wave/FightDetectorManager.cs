using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FightDetectorManager : MonoBehaviour
{
    public static FightDetectorManager S_Instance;

    [FormerlySerializedAs("onFightStart")] [SerializeField] private RSE_OnFightStarted m_OnFightStart;
    [FormerlySerializedAs("onFightEnd")] [SerializeField] private RSE_OnFightEnded m_OnFightEnd;
    private readonly List<EntityController> m_Enemys = new();
    private readonly List<WaveSystem> m_WaveSystems = new();

    private void Awake()
    {
        S_Instance = this;
    }

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

        if (m_Enemys.Contains(enemy)) return;
        m_Enemys.Add(enemy);
        enemy.OnDeath += OnEnemyDie;
    }

    private void OnEnemyDie(EntityController entity)
    {
        m_Enemys.Remove(entity);
        CheckEndFight();
    }

    private void CheckStartFight()
    {
        if (m_WaveSystems.Count <= 0 && m_Enemys.Count <= 0) m_OnFightStart.Call();
    }

    private void CheckEndFight()
    {
        if (m_WaveSystems.Count <= 0 && m_Enemys.Count <= 0) m_OnFightEnd.Call();
    }
}