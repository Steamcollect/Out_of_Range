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
    [Header("UI References")]
    [SerializeField] private Transform m_ScrollContent;
    [SerializeField] private UI_KeybindingKeyboardItem m_ItemPrefab;
    [SerializeField] private GameObject m_CategoryHeaderPrefab;
    [SerializeField] private UI_KeybindingRebindPopup m_RebindPopup;
    private readonly List<UI_KeybindingKeyboardItem> m_CreatedItems = new();
    private readonly List<GameObject> m_CreatedCategoryHeaders = new();
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
        RefreshUI();
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
    private Dictionary<string, List<(InputAction action, ActionBinding config)>> GetActionsGroupedByCategory()
    {
        var categories = new Dictionary<string, List<(InputAction, ActionBinding)>>();
        if (m_InputActionAsset == null) return categories;
        var actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return categories;
        foreach (var action in actionMap.actions)
        {
            int bindingIndex = FindKeyboardBindingIndex(action);
            if (bindingIndex < 0) continue;
            var config = FindActionBindingConfig(action.name);
            string category = config?.Category ?? m_DefaultCategory;
            if (!categories.ContainsKey(category))
            {
                categories[category] = new List<(InputAction, ActionBinding)>();
            }
            categories[category].Add((action, config));
        }
        return categories;
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
            foreach (var (inputAction, config) in category.Value)
            {
                int bindingIndex = FindKeyboardBindingIndex(inputAction);
                if (bindingIndex < 0) continue;
                var actionBinding = config ?? new ActionBinding
                {
                    ActionName = inputAction.name,
                    DisplayName = inputAction.name,
                    Category = m_DefaultCategory
                };
                var item = Instantiate(m_ItemPrefab, m_ScrollContent);
                item.Initialize(this, actionBinding, inputAction, bindingIndex);
                m_CreatedItems.Add(item);
            }
        }
    }
    private int FindKeyboardBindingIndex(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var binding = action.bindings[i];
            if (!binding.isComposite && !binding.isPartOfComposite)
            {
                if (binding.groups.Contains("Keyboard&Mouse") || 
                    binding.path.Contains("<Keyboard>") || 
                    binding.path.Contains("<Mouse>"))
                {
                    return i;
                }
            }
        }
        return -1;
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
                item.ApplyNewBinding(newBindingPath);
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
    public List<UI_KeybindingKeyboardItem> GetItems() => m_CreatedItems;
    public void FocusItem(UI_KeybindingKeyboardItem item)
    {
        if (item == null) return;
        var button = item.GetComponent<Button>();
        if (button != null)
            button.Select();
    }
}
