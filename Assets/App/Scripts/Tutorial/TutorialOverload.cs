using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class TutorialOverload : MonoBehaviour
{
    [SerializeField] WaveSystem m_WaveConnected;
    [SerializeField] RSO_PlayerController m_Player;
    [SerializeField] GameObject m_primmaryTxt;
    [SerializeField] GameObject m_SecondTxtGO;

    private async void OnEnable()
    {
        await Task.Yield();
        (m_Player.Value.GetPlayerCombat().GetPrimaryCombatStyle() as OverloadCombatStyle).OnOverloadStart += OnPlayerOverload;
    }
    private void OnDisable()
    {
        (m_Player.Value.GetPlayerCombat().GetPrimaryCombatStyle() as OverloadCombatStyle).OnOverloadStart -= OnPlayerOverload;
    }

    private void Start()
    {
        for (int i = 0; i < m_WaveConnected.m_MaxWaveCount; i++)
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() => { UpdateWave(); });
            m_WaveConnected.m_OnWaveStartEvents.Add(e);
        }
    }

    [Button]
    void StartTuto()
    {
        m_WaveConnected.StartCombat();
    }

    public void UpdateWave()
    {
        int i = m_WaveConnected.m_CurrentWaveIndex;
    }

    public void OnPlayerOverload()
    {
        m_primmaryTxt.SetActive(false);
        m_SecondTxtGO.SetActive(true);
    }
}