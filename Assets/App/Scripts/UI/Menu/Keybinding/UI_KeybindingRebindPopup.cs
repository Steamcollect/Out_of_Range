using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class UI_KeybindingRebindPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject m_PopupPanel;
    [SerializeField] private Image m_BlockingOverlay; // Image couvrant tout l'écran pour bloquer les interactions en arrière-plan
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_WaitingText;
    [SerializeField] private TextMeshProUGUI m_CurrentBindingText;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;
    [SerializeField] private UI_PanelTabManager m_PanelTabManager;
    
    [Header("Settings")]
    [SerializeField] private string m_WaitingMessage = "Press any button...";
    [SerializeField] private float m_TimeoutDuration = 5f;
    
    private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;
    private Action<string> m_OnRebindComplete;
    private Action m_OnRebindCancelled;
    private string m_NewBindingPath;
    
    private void Awake()
    {
        Hide();
    }
    
    private void OnDestroy()
    {
        CleanupRebindOperation();
    }
    
    /// <summary>
    /// Shows the rebind popup and starts listening for input
    /// </summary>
    /// <param name="actionName">Display name of the action being rebound</param>
    /// <param name="action">The InputAction to rebind</param>
    /// <param name="bindingIndex">Index of the binding to modify</param>
    /// <param name="onComplete">Callback when rebind is confirmed with new binding path</param>
    /// <param name="onCancelled">Callback when rebind is cancelled</param>
    public void Show(string actionName, InputAction action, int bindingIndex, 
        Action<string> onComplete, Action onCancelled)
    {
        m_OnRebindComplete = onComplete;
        m_OnRebindCancelled = onCancelled;
        m_NewBindingPath = null;
        
        if (m_BlockingOverlay != null)
            m_BlockingOverlay.enabled = true;
        
        if (m_PopupPanel != null)
            m_PopupPanel.SetActive(true);
        
        if (m_TitleText != null)
            m_TitleText.text = $"Rebind: {actionName}";
        
        if (m_WaitingText != null)
            m_WaitingText.text = m_WaitingMessage;
        
        if (m_CurrentBindingText != null)
            m_CurrentBindingText.text = "";
        
        if (m_ConfirmButton != null)
            m_ConfirmButton.interactable = false;
        
        StartRebindOperation(action, bindingIndex);
        m_PanelTabManager.TakeFocus(this);
    }
    
   
    public void Hide()
    {
        m_PanelTabManager.TakeFocus(false);
        CleanupRebindOperation();
        
        if (m_BlockingOverlay != null)
            m_BlockingOverlay.enabled = false;
        
        if (m_PopupPanel != null)
            m_PopupPanel.SetActive(false);
    }
    
    private void StartRebindOperation(InputAction action, int bindingIndex)
    {
        CleanupRebindOperation();
        
        action.Disable();
        
        InputActionRebindingExtensions.RebindingOperation rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTimeout(m_TimeoutDuration)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => OnRebindOperationComplete(operation, action))
            .OnCancel(operation => OnRebindOperationCancelled(operation, action));
        
        rebindOperation.WithControlsHavingToMatchPath("<Keyboard>");
        rebindOperation.WithControlsHavingToMatchPath("<Mouse>");
        
        m_RebindOperation = rebindOperation;
        m_RebindOperation.Start();
    }
    
    private void OnRebindOperationComplete(InputActionRebindingExtensions.RebindingOperation operation, InputAction action)
    {
        m_NewBindingPath = operation.selectedControl.path;
        
        if (m_WaitingText != null)
            m_WaitingText.text = "Input detected!";
        
        if (m_CurrentBindingText != null)
            m_CurrentBindingText.text = InputControlPath.ToHumanReadableString(m_NewBindingPath, 
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        if (m_ConfirmButton != null)
            m_ConfirmButton.interactable = true;
        
        action.Enable();
        CleanupRebindOperation();
    }
    
    private void OnRebindOperationCancelled(InputActionRebindingExtensions.RebindingOperation _, InputAction action)
    {
        action.Enable();
        CleanupRebindOperation();
        
        CancelClicked();
    }
    
    public void ConfirmClicked()
    {
        if (!string.IsNullOrEmpty(m_NewBindingPath))
        {
            m_OnRebindComplete?.Invoke(m_NewBindingPath);
        }
        
        Hide();
    }
    
    public void CancelClicked()
    {
        m_OnRebindCancelled?.Invoke();
        Hide();
    }
    
    private void CleanupRebindOperation()
    {
        m_RebindOperation?.Dispose();
        m_RebindOperation = null;
    }
}
