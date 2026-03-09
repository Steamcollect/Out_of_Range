using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class KillStreakManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_StepTime = 8;
    [SerializeField] KillStreakStep[] m_Steps;

    [Space(5)]
    [SerializeField, ReadOnly] int m_CurrentStreak = 0;
    
    [Space(10)]
    [SerializeField] string m_DebugCombosNames;

    float m_Timer;
    int m_CurrentStep;

    [Header("References")]
    [SerializeField] RSO_PlayerController m_Player;
    [SerializeField] RSO_KillStreakTimer m_TimerOnMax;

    [Header("Input")]
    [SerializeField] RSE_OnEnemyDie m_OnEnemyKilled;

    [Header("Output")]
    [SerializeField] RSE_OnStepIncrease m_OnStepIncrease;
    [SerializeField] RSE_OnStepDecrease m_OnStepDecrease;

    private void OnEnable()
    {
        m_OnEnemyKilled.Action += IncreaseStep;
        m_Player.Value.GetHealth().OnTakeDamage += DecreaseStep;
        
    }

    private void OnDisable()
    {
        m_OnEnemyKilled.Action -= IncreaseStep;
        m_Player.Value.GetHealth().OnTakeDamage -= DecreaseStep;
    }

    private void Start()
    {
        m_Steps = m_Steps.OrderBy(c => c.KillStreakRequire).ToArray();

        if (m_Steps.Length > 0 && m_Steps[0].KillStreakRequire > 0)
        {
            KillStreakStep baseStep = new KillStreakStep();

            KillStreakStep[] newArray = new KillStreakStep[m_Steps.Length + 1];
            newArray[0] = baseStep;
            for (int i = 0; i < m_Steps.Length; i++)
                newArray[i + 1] = m_Steps[i];

            m_Steps = newArray;
        }

        m_Player.Value.GetMovement().SetSpeedMult(1);
    }

    private void Update()
    {
        if(m_CurrentStep > 0)
        {
            m_Timer -= Time.deltaTime;

            if(m_Timer <= 0)
            {
                m_Timer = m_StepTime;
                DecreaseStep();
            }

            if(m_CurrentStep > 0)
                m_TimerOnMax.Set(m_Timer / m_StepTime);
        }
    }

    void IncreaseStep()
    {
        int current = m_CurrentStep;
        m_CurrentStreak++;

        for (int i = 0; i < m_Steps.Length; i++)
        {
            if (m_Steps[i].KillStreakRequire <= m_CurrentStreak)
            {
                m_CurrentStep = i;
                m_Player.Value.GetMovement().SetSpeedMult(m_Steps[i].SpeedMult);
            }
        }

        if (current == m_CurrentStep) return;

        m_Timer = m_StepTime;
        m_OnStepIncrease.Call(m_Steps[m_CurrentStep]);
        m_TimerOnMax.Set(m_Timer / m_StepTime);
    }

    void DecreaseStep()
    {
        m_CurrentStreak = 0;
        m_CurrentStep = 0;
        m_Player.Value.GetMovement().SetSpeedMult(m_Steps[m_CurrentStep].SpeedMult);
        m_OnStepDecrease.Call(m_Steps[m_CurrentStep]);
    }

    [Button]
    void LerpSteps()
    {
        if (m_Steps == null || m_Steps.Length < 2)
            return;

        KillStreakStep first = m_Steps[0];
        KillStreakStep last = m_Steps[m_Steps.Length - 1];

        int count = m_Steps.Length;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            m_Steps[i].SpeedMult = Mathf.Lerp(first.SpeedMult, last.SpeedMult, t);
        }
    }

    [Button]
    void CreateStepsByName()
    {
        if (string.IsNullOrWhiteSpace(m_DebugCombosNames))
            return;

        string[] names = m_DebugCombosNames
            .Split(',')
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToArray();

        m_Steps = new KillStreakStep[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            m_Steps[i] = new KillStreakStep
            {
                SpeedMult = 1f            
            };
        }
    }

    public KillStreakStep GetKillStreakStep() =>m_Steps[m_CurrentStep];   
}

[System.Serializable]
public class KillStreakStep
{
    public float SpeedMult;
    public int KillStreakRequire;

    [Space(10)]
    public float TrailLifetime;
    public Color StepColor;

    public KillStreakStep()
    {
        KillStreakRequire = 0;
        SpeedMult = 1;
        StepColor = new();
    }
}