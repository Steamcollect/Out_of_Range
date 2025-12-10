using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class UI_PanelTabManager : MonoBehaviour
{
    [Title("PANEL LIST")]
    [SerializeField] private List<GameObject> m_Panels = new List<GameObject>();

    [Title("BUTTON LIST")]
    [SerializeField] private List<GameObject> m_Buttons = new List<GameObject>();

    private GameObject m_CurrentPanel;
    private GameObject m_NextPanel;

    private GameObject m_CurrentButton;
    private GameObject m_NextButton;

    [Title("SETTINGS")]
    [SerializeField] private int m_CurrentPanelIndex = 0;

    private int m_CurrentButtonIndex = 0;

    private Animator m_CurrentPanelAnimator;
    private Animator m_NextPanelAnimator;

    private Animator m_CurrentButtonAnimator;
    private Animator m_NextButtonAnimator;

    private string m_PanelFadeIn = "FadeIn";
    private string m_PanelFadeOut = "FadeOut";
    private string m_ButtonFadeIn = "HoverToPressed";
    private string m_ButtonFadeOut = "PressedToNormal";

    private void Start()
    {
        m_CurrentButton = m_Buttons[m_CurrentButtonIndex];
        m_CurrentButtonAnimator = m_CurrentButton.GetComponent<Animator>();
        m_CurrentButtonAnimator.Play(m_ButtonFadeIn);

        m_CurrentPanel = m_Panels[m_CurrentPanelIndex];
        m_CurrentPanelAnimator = m_CurrentPanel.GetComponent<Animator>();
        m_CurrentPanelAnimator.Play(m_PanelFadeIn);
    }

    public void OpenFirstTab()
    {
        m_CurrentPanel = m_Panels[m_CurrentPanelIndex];
        m_CurrentPanelAnimator = m_CurrentPanel.GetComponent<Animator>();
        m_CurrentPanelAnimator.Play(m_PanelFadeIn);

        m_CurrentButton = m_Buttons[m_CurrentButtonIndex];
        m_CurrentButtonAnimator = m_CurrentButton.GetComponent<Animator>();
        m_CurrentButtonAnimator.Play(m_ButtonFadeIn);
    }

    public void PanelAnim(int newPanel)
    {
        if(newPanel != m_CurrentPanelIndex)
        {
            m_CurrentPanel = m_Panels[m_CurrentPanelIndex];
            m_CurrentPanelIndex = newPanel;
            m_NextPanel = m_Panels[m_CurrentPanelIndex];

            m_CurrentPanelAnimator = m_CurrentPanel.GetComponent<Animator>();
            m_NextPanelAnimator = m_NextPanel.GetComponent<Animator>();
            m_CurrentPanelAnimator.Play(m_PanelFadeOut);
            m_NextPanelAnimator.Play(m_PanelFadeIn);

            m_CurrentButton = m_Buttons[m_CurrentButtonIndex];
            m_CurrentButtonIndex = newPanel;
            m_NextButton = m_Buttons[m_CurrentButtonIndex];

            m_CurrentButtonAnimator = m_CurrentButton.GetComponent<Animator>();
            m_NextButtonAnimator = m_NextButton.GetComponent<Animator>();

            m_CurrentButtonAnimator.Play(m_ButtonFadeOut);
            m_NextButtonAnimator.Play(m_ButtonFadeIn);
        }
    }

    public void NextPage()
    {
        if(m_CurrentPanelIndex <= m_Panels.Count - 2)
        {
            m_CurrentPanel = m_Panels[m_CurrentPanelIndex];
            m_CurrentButton = m_Buttons[m_CurrentButtonIndex];
            m_NextButton = m_Buttons[m_CurrentButtonIndex + 1];

            m_CurrentPanelAnimator = m_CurrentPanel.GetComponent<Animator>();
            m_CurrentButtonAnimator = m_CurrentButton.GetComponent<Animator>();
            m_CurrentButtonAnimator.Play(m_ButtonFadeOut);
            m_CurrentPanelAnimator.Play(m_PanelFadeOut);

            m_CurrentPanelIndex++;
            m_CurrentButtonIndex++;
            m_NextPanel = m_Panels[m_CurrentPanelIndex];

            m_NextPanelAnimator = m_NextPanel.GetComponent<Animator>();
            m_NextButtonAnimator = m_NextButton.GetComponent<Animator>();

            m_NextButtonAnimator.Play(m_ButtonFadeIn);
            m_NextPanelAnimator.Play(m_PanelFadeIn);
        }
    }

    public void PreviousPage()
    {
        if(m_CurrentPanelIndex >= 1)
        {
            m_CurrentPanel = m_Panels[m_CurrentPanelIndex];
            m_CurrentButton = m_Buttons[m_CurrentButtonIndex];
            m_NextButton = m_Buttons[m_CurrentButtonIndex - 1];

            m_CurrentPanelAnimator = m_CurrentPanel.GetComponent<Animator>();
            m_CurrentButtonAnimator = m_CurrentButton.GetComponent<Animator>();

            m_CurrentButtonAnimator.Play(m_ButtonFadeOut);
            m_CurrentPanelAnimator.Play(m_PanelFadeOut);

            m_CurrentPanelIndex--;
            m_CurrentButtonIndex--;
            m_NextPanel = m_Panels[m_CurrentPanelIndex];

            m_NextPanelAnimator = m_NextPanel.GetComponent<Animator>();
            m_NextButtonAnimator = m_NextButton.GetComponent<Animator>();

            m_NextButtonAnimator.Play(m_ButtonFadeIn);
            m_NextPanelAnimator.Play(m_PanelFadeIn);
        }
    }
}