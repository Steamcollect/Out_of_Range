using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialOverload : MonoBehaviour
{
    [SerializeField] WaveSystem m_WaveConnected;
    [SerializeField] TMP_Text m_WaveCountTxt;

    private void Start()
    {
        for (int i = 0; i < m_WaveConnected.m_MaxWaveCount; i++)
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() => { UpdateWave(); });
            m_WaveConnected.m_OnWaveStartEvents.Add(e);
        }

        m_WaveConnected.AddOnCombatCompleteListener(() => { OnWaveComplete(); });
    }

    [Button]
    void StartTuto()
    {
        m_WaveConnected.StartCombat();
    }

    public void UpdateWave()
    {
        int i = m_WaveConnected.m_CurrentWaveIndex;

        m_WaveCountTxt.text = $"{i}/{m_WaveConnected.m_MaxWaveCount}";
    }

    public void OnWaveComplete()
    {
        m_WaveCountTxt.text = $"{m_WaveConnected.m_MaxWaveCount}/{m_WaveConnected.m_MaxWaveCount}";
    }
}