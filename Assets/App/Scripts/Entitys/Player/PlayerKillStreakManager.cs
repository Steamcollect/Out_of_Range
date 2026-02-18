using System.Linq;
using MoreMountains.Feedbacks;
using MVsToolkit.Dev;
using UnityEngine;

public class PlayerKillStreakManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, ReadOnly] int m_CurrentStreak = 0;
    [SerializeField] KillStreakStep[] m_Steps;

    float m_Timer;
    int m_CurrentStep;

    [System.Serializable]
    struct KillStreakStep
    {
        public int KillStreakRequire;
        public float SpeedMult;

        [Space(10)]
        public string StepName;
        public Color StepColor;
    }

    [Header("References")]
    [SerializeField] RSO_PlayerController m_Player;

    [Header("Input")]
    [SerializeField] RSE_OnEnemyDie m_OnEnemyKilled;

    //[Header("Output")]

    private void OnEnable()
    {
        m_OnEnemyKilled.Action += OnEnemyKilled;
        m_Player.Value.GetHealth().OnTakeDamage += OnPlayerTakeDamage;
        
    }

    private void OnDisable()
    {
        m_OnEnemyKilled.Action -= OnEnemyKilled;
        m_Player.Value.GetHealth().OnTakeDamage -= OnPlayerTakeDamage;
    }

    private void Start()
    {
        m_Steps = m_Steps.OrderBy(c => c.KillStreakRequire).ToArray();
        if (m_Steps[0].KillStreakRequire > 0)
        {

        }

        m_Player.Value.GetMovement().SetSpeedMult(1);
    }

    void OnEnemyKilled()
    {
        m_CurrentStreak++;

        for (int i = 0; i < m_Steps.Length; i++)
        {
            if (m_Steps[i].KillStreakRequire <= m_CurrentStreak)
            {
                m_CurrentStep = i;
                m_Player.Value.GetMovement().SetSpeedMult(m_Steps[i].SpeedMult);
            }
        }

    }

    void OnPlayerTakeDamage()
    {
        m_CurrentStreak = 0;
        m_CurrentStep = 0;
        m_Player.Value.GetMovement().SetSpeedMult(1);
    }
}