using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Classe de base pour tous les panels UI.
/// Gère les animations, le focus/unfocus et le blocage des interactions.
/// </summary>
public class UI_PanelBase : MonoBehaviour
{
    #region Serialized Fields
    
    [Title("Panel Settings")]
    [SerializeField] protected CanvasGroup m_CanvasGroup;
    [SerializeField] protected Animator m_Animator;
    [SerializeField] protected Selectable m_FirstSelected;
    
    [Title("Buttons")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "ButtonName")]
    [SerializeField] protected List<ButtonItem> m_Buttons = new List<ButtonItem>();
    
    [Title("Events")]
    [SerializeField] protected UnityEvent m_OnPanelFocused;
    [SerializeField] protected UnityEvent m_OnPanelUnfocused;
    
    #endregion

    #region Private Fields
    
    protected const string k_AnimFadeIn = "FadeIn";
    protected const string k_AnimFadeOut = "FadeOut";
    
    private bool m_IsFocused;
    private bool m_IsInteractable = true;
    
    #endregion

    #region Properties
    
    /// <summary>
    /// Indique si le panel a actuellement le focus.
    /// </summary>
    public bool IsFocused => m_IsFocused;
    
    /// <summary>
    /// Indique si le panel est interactable.
    /// </summary>
    public bool IsInteractable => m_IsInteractable;
    
    /// <summary>
    /// Event déclenché quand le panel reçoit le focus.
    /// </summary>
    public event Action OnFocused;
    
    /// <summary>
    /// Event déclenché quand le panel perd le focus.
    /// </summary>
    public event Action OnUnfocused;
    
    #endregion

    #region Nested Types
    
    [Serializable]
    public class ButtonItem
    {
        public string ButtonName = "DefaultButton";
        public Button Button;
        public UnityEvent OnClick;
    }
    
    #endregion

    #region Unity Lifecycle
    
    protected virtual void Awake()
    {
        InitializeButtons();
    }
    
    #endregion

    #region Initialization
    
    protected virtual void InitializeButtons()
    {
        if (m_Buttons == null || m_Buttons.Count == 0) return;
        
        foreach (var buttonItem in m_Buttons)
        {
            if (buttonItem?.Button == null) continue;
            
            var item = buttonItem;
            buttonItem.Button.onClick.AddListener(() => item.OnClick?.Invoke());
        }
    }
    
    #endregion

    #region Focus Management
    
    /// <summary>
    /// Donne le focus au panel et déclenche les événements associés.
    /// </summary>
    public virtual void Focus()
    {
        if (m_IsFocused) return;
        
        m_IsFocused = true;
        SetInteractable(true);
        
        if (m_FirstSelected != null)
            m_FirstSelected.Select();
        
        PlayAnimation(k_AnimFadeIn);
        
        m_OnPanelFocused?.Invoke();
        OnFocused?.Invoke();
    }
    
    /// <summary>
    /// Retire le focus du panel et déclenche les événements associés.
    /// </summary>
    public virtual void Unfocus()
    {
        if (!m_IsFocused) return;
        
        m_IsFocused = false;
        
        PlayAnimation(k_AnimFadeOut);
        
        m_OnPanelUnfocused?.Invoke();
        OnUnfocused?.Invoke();
    }
    
    /// <summary>
    /// Active ou désactive le panel (avec animation).
    /// </summary>
    /// <param name="active">True pour activer, false pour désactiver.</param>
    public virtual void SetPanelActive(bool active)
    {
        if (active)
            Focus();
        else
            Unfocus();
    }
    
    #endregion

    #region Interaction Control
    
    /// <summary>
    /// Prend ou relâche le contrôle des interactions.
    /// Quand le focus est pris par un sous-élément (popup), les autres éléments sont bloqués.
    /// </summary>
    /// <param name="takeFocus">True pour prendre le focus exclusif (bloquer les autres), false pour relâcher.</param>
    public virtual void TakeFocus(bool takeFocus)
    {
        // Quand on prend le focus exclusif, on désactive les interactions du panel parent
        SetInteractable(!takeFocus);
    }
    
    /// <summary>
    /// Active ou désactive l'interactivité du panel et de ses boutons.
    /// </summary>
    /// <param name="interactable">True pour rendre interactable, false sinon.</param>
    public virtual void SetInteractable(bool interactable)
    {
        m_IsInteractable = interactable;
        
        // Utiliser CanvasGroup si disponible (méthode propre)
        if (m_CanvasGroup != null)
        {
            m_CanvasGroup.interactable = interactable;
            m_CanvasGroup.blocksRaycasts = interactable;
        }
        
        // Backup: désactiver les boutons individuellement
        SetButtonsInteractable(interactable);
    }
    
    /// <summary>
    /// Active ou désactive tous les boutons du panel.
    /// </summary>
    /// <param name="interactable">True pour rendre interactable, false sinon.</param>
    protected virtual void SetButtonsInteractable(bool interactable)
    {
        if (m_Buttons == null) return;
        
        foreach (var buttonItem in m_Buttons)
        {
            if (buttonItem?.Button != null)
                buttonItem.Button.interactable = interactable;
        }
    }
    
    #endregion

    #region Animation
    
    /// <summary>
    /// Joue une animation sur le panel.
    /// </summary>
    /// <param name="triggerName">Nom du trigger d'animation.</param>
    protected virtual void PlayAnimation(string triggerName)
    {
        if (m_Animator != null)
            m_Animator.SetTrigger(triggerName);
    }
    
    /// <summary>
    /// Joue directement un état d'animation.
    /// </summary>
    /// <param name="stateName">Nom de l'état d'animation.</param>
    protected virtual void PlayAnimationState(string stateName)
    {
        if (m_Animator != null)
            m_Animator.Play(stateName);
    }
    
    #endregion
}
