using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PanelManager : MonoBehaviour
{
    [Title("PANEL LIST")]
    [SerializeField] private List<PanelItem> m_Panels = new List<PanelItem>();

    [Title("References")]
    [SerializeField] private RSE_OpenPanel m_OpenPanel;

    public int m_CurrentPanelIndex = 0;
    private int m_NewPanelIndex;

    private PanelItem m_NewPanel;

    [System.Serializable]
    private class PanelItem
    {
        public string PanelName = "DefaultPanel";
        public UI_Panel Panel = null;
    }

    private void OnEnable() => m_OpenPanel.Action += OpenPanel;
    private void OnDisable() => m_OpenPanel.Action -= OpenPanel;

    private void Awake()
    {
        if(m_Panels.Count == 0) return;

        InitializePanels();
    }

    private void InitializePanels()
    {
        for(int i = 0; i < m_Panels.Count; i++)
        {
            if(i == m_CurrentPanelIndex)
            {
                m_Panels[i].Panel.SetPanelActive(true);
            }
            else
            {
                m_Panels[i].Panel.SetPanelActive(false);
            }
        }
    }

    private void OpenPanel(string newPanel)
    {
        for(int i = 0; i < m_Panels.Count; i++)
        {
            if(m_Panels[i].PanelName == newPanel)
            {
                m_NewPanelIndex = i;
                break;
            }
        }

        if(m_NewPanelIndex != m_CurrentPanelIndex)
        {
            StopCoroutine(nameof(DisablePreviousPanel));

            m_CurrentPanelIndex = m_NewPanelIndex;
            m_NewPanel = m_Panels[m_CurrentPanelIndex];
            m_NewPanel.Panel.SetPanelActive(true);

            StartCoroutine(nameof(DisablePreviousPanel));
        }
    }

    private IEnumerator DisablePreviousPanel()
    {
        yield return new WaitForSecondsRealtime(0);

        for(int i = 0; i < m_Panels.Count; i++)
        {
            if(i != m_CurrentPanelIndex)
            {
                m_Panels[i].Panel.SetPanelActive(false);
            }
        }
    }
}