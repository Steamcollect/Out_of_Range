using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Gestionnaire de tabs/panels avec système de focus.
/// Permet de naviguer entre plusieurs panels et gère le blocage des interactions.
/// </summary>
public class UI_PanelTabManager : MonoBehaviour
{
    #region Serialized Fields
    
    [Title("Panel Configuration")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "PanelName")]
    [SerializeField] private List<TabPanelItem> m_TabPanels = new List<TabPanelItem>();

    [Title("Settings")]
    [SerializeField] private int m_DefaultPanelIndex;
    [SerializeField] private bool m_InitializeOnStart = true;
    
    [Title("Events")]
    [SerializeField] private UnityEvent<int> m_OnPanelChanged;
    [SerializeField] private UnityEvent m_OnFocusTaken;
    [SerializeField] private UnityEvent m_OnFocusReleased;
    
    #endregion

    #region Private Fields
    
    private int m_CurrentPanelIndex = -1;
    private bool m_HasExclusiveFocus;
    
    private const string k_AnimButtonSelected = "HoverToPressed";
    private const string k_AnimButtonDeselected = "PressedToNormal";
    
    #endregion

    #region Properties
    
    /// <summary>
    /// Index du panel actuellement actif.
    /// </summary>
    public int CurrentPanelIndex => m_CurrentPanelIndex;
    
    /// <summary>
    /// Panel actuellement actif.
    /// </summary>
    public TabPanelItem CurrentPanel => IsValidIndex(m_CurrentPanelIndex) ? m_TabPanels[m_CurrentPanelIndex] : null;
    
    /// <summary>
    /// Nombre total de panels.
    /// </summary>
    public int PanelCount => m_TabPanels.Count;
    
    /// <summary>
    /// Indique si un sous-élément a pris le focus exclusif.
    /// </summary>
    public bool HasExclusiveFocus => m_HasExclusiveFocus;
    
    /// <summary>
    /// Event déclenché quand un panel reçoit le focus.
    /// </summary>
    public event Action<int> OnPanelFocused;
    
    /// <summary>
    /// Event déclenché quand un panel perd le focus.
    /// </summary>
    public event Action<int> OnPanelUnfocused;
    
    #endregion

    #region Nested Types
    
    [Serializable]
    public class TabPanelItem
    {
        [HorizontalGroup("Main")]
        public string PanelName = "Panel";
        
        [FoldoutGroup("Main/References")]
        [Tooltip("Référence au panel UI (doit hériter de UI_PanelBase)")]
        public UI_PanelBase Panel;
        
        [FoldoutGroup("Main/References")]
        public GameObject TabButton;
        
        // Cached components
        [NonSerialized] public Animator ButtonAnimator;
        [NonSerialized] public Button Button;
        
        public bool IsValid => Panel != null;
    }
    
    #endregion

    #region Unity Lifecycle
    
    private void Awake()
    {
        CacheComponents();
    }
    
    private void Start()
    {
        if (m_InitializeOnStart)
            OpenTab(m_DefaultPanelIndex);
    }
    
    #endregion

    #region Initialization
    
    private void CacheComponents()
    {
        foreach (var tabPanel in m_TabPanels)
        {
            if (tabPanel.TabButton != null)
            {
                tabPanel.ButtonAnimator = tabPanel.TabButton.GetComponent<Animator>();
                tabPanel.Button = tabPanel.TabButton.GetComponent<Button>();
            }
        }
    }
    
    #endregion

    #region Public API - Navigation
    
    /// <summary>
    /// Ouvre un tab par son index.
    /// </summary>
    /// <param name="index">Index du tab à ouvrir.</param>
    public void OpenTab(int index)
    {
        if (!IsValidIndex(index) || index == m_CurrentPanelIndex)
            return;
        
        // Unfocus le panel actuel
        if (IsValidIndex(m_CurrentPanelIndex))
            UnfocusPanel(m_CurrentPanelIndex);
        
        // Focus le nouveau panel
        m_CurrentPanelIndex = index;
        FocusPanel(index);
        
        m_OnPanelChanged?.Invoke(index);
    }
    
    /// <summary>
    /// Alias pour OpenTab - compatibilité avec l'ancien code.
    /// </summary>
    public void PanelAnim(int newPanel) => OpenTab(newPanel);
    
    /// <summary>
    /// Ouvre le premier tab.
    /// </summary>
    public void OpenFirstTab() => OpenTab(m_DefaultPanelIndex);
    
    /// <summary>
    /// Passe au tab suivant.
    /// </summary>
    public void NextPage()
    {
        if (m_CurrentPanelIndex < m_TabPanels.Count - 1)
            OpenTab(m_CurrentPanelIndex + 1);
    }
    
    /// <summary>
    /// Passe au tab précédent.
    /// </summary>
    public void PreviousPage()
    {
        if (m_CurrentPanelIndex > 0)
            OpenTab(m_CurrentPanelIndex - 1);
    }
    
    #endregion

    #region Public API - Focus Control
    
    /// <summary>
    /// Prend ou relâche le focus exclusif.
    /// Utilisé quand un sous-élément (popup, dialogue) doit bloquer les interactions.
    /// </summary>
    /// <param name="takeFocus">True pour prendre le focus (bloquer les tabs), false pour relâcher.</param>
    public void TakeFocus(bool takeFocus)
    {
        m_HasExclusiveFocus = takeFocus;
        
        // Désactiver/activer tous les boutons de tabs
        SetAllTabButtonsInteractable(!takeFocus);
        
        // Désactiver/activer le panel actuel
        if (IsValidIndex(m_CurrentPanelIndex))
            SetPanelInteractable(m_CurrentPanelIndex, !takeFocus);
        
        if (takeFocus)
            m_OnFocusTaken?.Invoke();
        else
            m_OnFocusReleased?.Invoke();
    }
    
    /// <summary>
    /// Active ou désactive l'interactivité d'un panel spécifique.
    /// </summary>
    /// <param name="index">Index du panel.</param>
    /// <param name="interactable">True pour rendre interactable.</param>
    public void SetPanelInteractable(int index, bool interactable)
    {
        if (!IsValidIndex(index)) return;
        
        var tabPanel = m_TabPanels[index];
        
        if (tabPanel.Panel != null)
            tabPanel.Panel.SetInteractable(interactable);
    }
    
    /// <summary>
    /// Active ou désactive tous les boutons de tabs.
    /// </summary>
    /// <param name="interactable">True pour rendre interactable.</param>
    public void SetAllTabButtonsInteractable(bool interactable)
    {
        foreach (var tabPanel in m_TabPanels)
        {
            if (tabPanel.Button != null)
                tabPanel.Button.interactable = interactable;
        }
    }
    
    #endregion

    #region Private Methods - Focus
    
    private void FocusPanel(int index)
    {
        if (!IsValidIndex(index)) return;
        
        var tabPanel = m_TabPanels[index];
        
        // Focus du panel via UI_PanelBase (gère animation + events)
        if (tabPanel.Panel != null)
            tabPanel.Panel.Focus();
        
        // Animation du bouton
        if (tabPanel.ButtonAnimator != null)
            tabPanel.ButtonAnimator.Play(k_AnimButtonSelected);
        
        OnPanelFocused?.Invoke(index);
    }
    
    private void UnfocusPanel(int index)
    {
        if (!IsValidIndex(index)) return;
        
        var tabPanel = m_TabPanels[index];
        
        // Unfocus du panel via UI_PanelBase (gère animation + events)
        if (tabPanel.Panel != null)
            tabPanel.Panel.Unfocus();
        
        // Animation du bouton
        if (tabPanel.ButtonAnimator != null)
            tabPanel.ButtonAnimator.Play(k_AnimButtonDeselected);
        
        OnPanelUnfocused?.Invoke(index);
    }
    
    #endregion

    #region Utility
    
    private bool IsValidIndex(int index) => index >= 0 && index < m_TabPanels.Count;
    
    #endregion
}