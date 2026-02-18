using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UI_KeybindingKeyboardManager : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset m_InputActionAsset;
    [SerializeField] private string m_ActionMapName = "Player";
    
    [Header("Category Configuration (Optional)")]
    [Tooltip("Optional: Define categories and display names for actions. Actions not listed will use default category.")]
    [SerializeField] private List<ActionBinding> m_ActionBindings = new();
    [SerializeField] private string m_DefaultCategory = "General";
    
    [Header("Action Filtering")]
    [Tooltip("List of action names to exclude from the UI")]
    [SerializeField] private List<string> m_ExcludedActions = new();
    [Tooltip("If true, only actions defined in ActionBindings will be shown")]
    [SerializeField] private bool m_OnlyShowDefinedActions;
    
    [Header("UI References")]
    [SerializeField] private Transform m_ScrollContent;
    [SerializeField] private UI_KeybindingKeyboardItem m_ItemPrefab;
    [SerializeField] private GameObject m_CategoryHeaderPrefab;
    [SerializeField] private UI_KeybindingRebindPopup m_RebindPopup;
    
    private readonly List<UI_KeybindingKeyboardItem> m_CreatedItems = new();
    private readonly List<GameObject> m_CreatedCategoryHeaders = new();
    private readonly Dictionary<string, string> m_SessionOverrides = new();
    private bool m_IsRebinding;
    
    public InputActionAsset InputActionAsset => m_InputActionAsset;
    public string ActionMapName => m_ActionMapName;
    public List<ActionBinding> ActionBindings => m_ActionBindings;
    
    [Serializable]
    public class ActionBinding
    {
        [Tooltip("Name of the action in the InputActionMap")]
        public string ActionName;
        [Tooltip("Display name shown in the UI")]
        public string DisplayName;
        [Tooltip("Category for grouping actions in the UI")]
        public string Category = "General";
        [Tooltip("Icon to display for this binding")]
        public Sprite BindingIcon;
    }
    
    private void OnEnable()
    {
        ApplySessionOverrides();
        RefreshUI();
    }
    
    /// <summary>
    /// Checks if an action should be excluded from the UI
    /// </summary>
    private bool IsActionExcluded(string actionName)
    {
        // Check exclusion list
        if (m_ExcludedActions.Contains(actionName))
            return true;
        
        // If only showing defined actions, check if it's defined
        if (m_OnlyShowDefinedActions)
        {
            return FindActionBindingConfig(actionName) == null;
        }
        
        return false;
    }
    
    private ActionBinding FindActionBindingConfig(string actionName)
    {
        foreach (var binding in m_ActionBindings)
        {
            if (binding.ActionName == actionName)
                return binding;
        }
        return null;
    }
    
    /// <summary>
    /// Groups actions by category, including all composite binding parts
    /// </summary>
    private Dictionary<string, List<(InputAction action, ActionBinding config, int bindingIndex, string compositePart)>> GetActionsGroupedByCategory()
    {
        var categories = new Dictionary<string, List<(InputAction, ActionBinding, int, string)>>();
        if (m_InputActionAsset == null) return categories;
        
        var actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return categories;
        
        foreach (var action in actionMap.actions)
        {
            // Check if action is excluded
            if (IsActionExcluded(action.name)) continue;
            
            var config = FindActionBindingConfig(action.name);
            string category = config?.Category ?? m_DefaultCategory;
            
            if (!categories.ContainsKey(category))
                categories[category] = new List<(InputAction, ActionBinding, int, string)>();
            
            // Find all keyboard/mouse bindings for this action
            var bindings = FindAllKeyboardMouseBindings(action);
            foreach (var (bindingIndex, compositePart) in bindings)
            {
                categories[category].Add((action, config, bindingIndex, compositePart));
            }
        }
        
        return categories;
    }
    
    /// <summary>
    /// Finds all keyboard/mouse binding indices for an action.
    /// Returns composite parts separately.
    /// </summary>
    private List<(int bindingIndex, string compositePart)> FindAllKeyboardMouseBindings(InputAction action)
    {
        var results = new List<(int, string)>();
        
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var binding = action.bindings[i];
            
            // Skip composite headers
            if (binding.isComposite) continue;
            
            // Check if this is a keyboard/mouse binding
            if (IsKeyboardMouseBinding(binding))
            {
                // If part of composite, include the part name
                string compositePart = binding.isPartOfComposite ? binding.name : null;
                results.Add((i, compositePart));
            }
        }
        
        return results;
    }
    
    private bool IsKeyboardMouseBinding(InputBinding binding)
    {
        // Check binding groups
        if (!string.IsNullOrEmpty(binding.groups) && binding.groups.Contains("Keyboard&Mouse"))
            return true;
        
        // Check path for keyboard or mouse
        if (!string.IsNullOrEmpty(binding.path))
        {
            if (binding.path.Contains("<Keyboard>") || binding.path.Contains("<Mouse>"))
                return true;
        }
        
        // Check effective path (for composite parts)
        if (!string.IsNullOrEmpty(binding.effectivePath))
        {
            if (binding.effectivePath.Contains("<Keyboard>") || binding.effectivePath.Contains("<Mouse>"))
                return true;
        }
        
        return false;
    }
    public void RefreshUI()
    {
        ClearItems();
        CreateItemsFromBindings();
    }
    
    private void ClearItems()
    {
        foreach (var item in m_CreatedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        m_CreatedItems.Clear();
        
        foreach (var header in m_CreatedCategoryHeaders)
        {
            if (header != null)
                Destroy(header);
        }
        m_CreatedCategoryHeaders.Clear();
    }
    
    private void CreateItemsFromBindings()
    {
        if (m_InputActionAsset == null || m_ScrollContent == null || m_ItemPrefab == null) return;
        
        var actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return;
        
        var categorizedActions = GetActionsGroupedByCategory();
        
        foreach (var category in categorizedActions)
        {
            if (m_CategoryHeaderPrefab != null)
            {
                var headerObj = Instantiate(m_CategoryHeaderPrefab, m_ScrollContent);
                var headerText = headerObj.GetComponentInChildren<TextMeshProUGUI>();
                if (headerText != null)
                    headerText.text = category.Key;
                m_CreatedCategoryHeaders.Add(headerObj);
            }
            
            foreach (var (action, config, bindingIndex, compositePart) in category.Value)
            {
                // Determine display name
                string baseName = config?.DisplayName ?? action.name;
                string displayName = string.IsNullOrEmpty(compositePart) 
                    ? baseName 
                    : $"{baseName} ({compositePart})";
                
                var actionBinding = new ActionBinding
                {
                    ActionName = action.name,
                    DisplayName = displayName,
                    Category = config?.Category ?? m_DefaultCategory,
                    BindingIcon = config?.BindingIcon
                };
                
                var item = Instantiate(m_ItemPrefab, m_ScrollContent);
                item.Initialize(this, actionBinding, action, bindingIndex);
                m_CreatedItems.Add(item);
            }
        }
    }
    public void StartRebind(UI_KeybindingKeyboardItem item, InputAction action, int bindingIndex)
    {
        if (m_IsRebinding || m_RebindPopup == null) return;
        m_IsRebinding = true;
        SetAllItemsInteractable(false);
        string actionName = item.ActionBinding?.DisplayName ?? action.name;
        m_RebindPopup.Show(actionName, action, bindingIndex,
            (newBindingPath) =>
            {
                // Stocker l'override en session au lieu de persister
                ApplySessionOverride(action, bindingIndex, newBindingPath);
                item.UpdateDisplay();
                EndRebind();
            },
            EndRebind);
    }
    
    private void EndRebind()
    {
        m_IsRebinding = false;
        SetAllItemsInteractable(true);
    }
    
    private void SetAllItemsInteractable(bool interactable)
    {
        foreach (var item in m_CreatedItems)
            item.SetInteractable(interactable);
    }
    
    #region Session Overrides
    
    /// <summary>
    /// Crée une clé unique pour identifier un binding
    /// </summary>
    private string GetBindingKey(InputAction action, int bindingIndex)
    {
        return $"{action.actionMap.name}/{action.name}/{bindingIndex}";
    }
    
    /// <summary>
    /// Applique un override pour la session uniquement (non persisté)
    /// </summary>
    private void ApplySessionOverride(InputAction action, int bindingIndex, string newPath)
    {
        string key = GetBindingKey(action, bindingIndex);
        m_SessionOverrides[key] = newPath;
        action.ApplyBindingOverride(bindingIndex, newPath);
    }
    
    /// <summary>
    /// Réapplique tous les overrides de session (appelé au OnEnable)
    /// </summary>
    private void ApplySessionOverrides()
    {
        if (m_InputActionAsset == null) return;
        
        var actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return;
        
        foreach (var kvp in m_SessionOverrides)
        {
            var parts = kvp.Key.Split('/');
            if (parts.Length != 3) continue;
            
            var action = actionMap.FindAction(parts[1]);
            if (action == null) continue;
            
            if (int.TryParse(parts[2], out int bindingIndex))
            {
                action.ApplyBindingOverride(bindingIndex, kvp.Value);
            }
        }
    }
    
    /// <summary>
    /// Réinitialise tous les bindings aux valeurs par défaut
    /// </summary>
    public void ResetAllBindings()
    {
        if (m_InputActionAsset == null) return;
        
        var actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return;
        
        // Clear session overrides
        m_SessionOverrides.Clear();
        
        // Remove all overrides from actions
        foreach (var action in actionMap.actions)
        {
            action.RemoveAllBindingOverrides();
        }
        
        RefreshUI();
    }
    
    /// <summary>
    /// Réinitialise un binding spécifique à sa valeur par défaut
    /// </summary>
    public void ResetBinding(InputAction action, int bindingIndex)
    {
        string key = GetBindingKey(action, bindingIndex);
        m_SessionOverrides.Remove(key);
        action.RemoveBindingOverride(bindingIndex);
    }
    
    #endregion
    
    public List<UI_KeybindingKeyboardItem> GetItems() => m_CreatedItems;
    
    public void FocusItem(UI_KeybindingKeyboardItem item)
    {
        if (item == null) return;
        var button = item.GetComponent<Button>();
        if (button != null)
            button.Select();
    }
}
