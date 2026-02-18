using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_KeybindingKeyboardManager : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset m_InputActionAsset;

    [SerializeField] private string m_ActionMapName = "Player";

    [Header("Category Configuration")]
    [SerializeField] private List<ActionBinding> m_ActionBindings = new();

    [SerializeField] private string m_DefaultCategory = "General";

    [Header("Action Filtering")]
    [SerializeField] private List<string> m_ExcludedActions = new();

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
    private string m_CurrentKeyboardLayout;

    [Serializable]
    public class ActionBinding
    {
        public string ActionName;
        public string DisplayName;
        public string Category = "General";
        public Sprite BindingIcon;
    }

    private void OnEnable()
    {
        m_CurrentKeyboardLayout = Keyboard.current?.keyboardLayout ?? "Unknown";
        ApplySessionOverrides();
        RefreshUI();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Keyboard && change == InputDeviceChange.ConfigurationChanged)
        {
            string newLayout = Keyboard.current?.keyboardLayout ?? "Unknown";
            if (newLayout != m_CurrentKeyboardLayout)
            {
                m_CurrentKeyboardLayout = newLayout;
                foreach (UI_KeybindingKeyboardItem item in m_CreatedItems)
                    item?.UpdateDisplay();
            }
        }
    }

    public static string GetKeyDisplayString(InputBinding binding)
    {
        if (string.IsNullOrEmpty(binding.effectivePath))
            return string.Empty;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            InputControl control = InputControlPath.TryFindControl(keyboard, binding.effectivePath);
            if (control != null && !string.IsNullOrEmpty(control.displayName))
                return control.displayName;
        }

        return InputControlPath.ToHumanReadableString(
            binding.effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private bool IsActionExcluded(string actionName)
    {
        if (m_ExcludedActions.Contains(actionName))
            return true;

        if (m_OnlyShowDefinedActions)
            return FindActionBindingConfig(actionName) == null;

        return false;
    }

    private ActionBinding FindActionBindingConfig(string actionName)
    {
        foreach (ActionBinding binding in m_ActionBindings)
            if (binding.ActionName == actionName)
                return binding;
        return null;
    }

    private Dictionary<string, List<(InputAction action, ActionBinding config, int bindingIndex, string compositePart)>>
        GetActionsGroupedByCategory()
    {
        var categories = new Dictionary<string, List<(InputAction, ActionBinding, int, string)>>();

        if (m_InputActionAsset == null)
            return categories;

        InputActionMap actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null)
            return categories;

        foreach (InputAction action in actionMap.actions)
        {
            if (IsActionExcluded(action.name))
                continue;

            ActionBinding config = FindActionBindingConfig(action.name);
            string category = config?.Category ?? m_DefaultCategory;

            if (!categories.ContainsKey(category))
                categories[category] = new List<(InputAction, ActionBinding, int, string)>();

            List<(int bindingIndex, string compositePart)> bindings = FindAllKeyboardMouseBindings(action);
            foreach ((int bindingIndex, string compositePart) in bindings)
                categories[category].Add((action, config, bindingIndex, compositePart));
        }

        return categories;
    }

    private List<(int bindingIndex, string compositePart)> FindAllKeyboardMouseBindings(InputAction action)
    {
        var results = new List<(int, string)>();

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (binding.isComposite)
                continue;

            if (IsKeyboardMouseBinding(binding))
            {
                string compositePart = binding.isPartOfComposite ? binding.name : null;
                results.Add((i, compositePart));
            }
        }

        return results;
    }

    private static bool IsKeyboardMouseBinding(InputBinding binding)
    {
        if (!string.IsNullOrEmpty(binding.groups) && binding.groups.Contains("Keyboard&Mouse"))
            return true;

        if (!string.IsNullOrEmpty(binding.path))
            if (binding.path.Contains("<Keyboard>") || binding.path.Contains("<Mouse>"))
                return true;

        if (!string.IsNullOrEmpty(binding.effectivePath))
            if (binding.effectivePath.Contains("<Keyboard>") || binding.effectivePath.Contains("<Mouse>"))
                return true;

        return false;
    }

    public void RefreshUI()
    {
        ClearItems();
        CreateItemsFromBindings();
    }

    private void ClearItems()
    {
        foreach (UI_KeybindingKeyboardItem item in m_CreatedItems)
            if (item != null)
                Destroy(item.gameObject);
        m_CreatedItems.Clear();

        foreach (GameObject header in m_CreatedCategoryHeaders)
            if (header != null)
                Destroy(header);
        m_CreatedCategoryHeaders.Clear();
    }

    private void CreateItemsFromBindings()
    {
        if (m_InputActionAsset == null || m_ScrollContent == null || m_ItemPrefab == null)
            return;

        InputActionMap actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null)
            return;

        Dictionary<string, List<(InputAction action, ActionBinding config, int bindingIndex, string compositePart)>>
            categorizedActions = GetActionsGroupedByCategory();

        foreach (KeyValuePair<string,
                     List<(InputAction action, ActionBinding config, int bindingIndex, string compositePart)>> category
                 in categorizedActions)
        {
            CreateCategoryHeader(category.Key);

            foreach ((InputAction action, ActionBinding config, int bindingIndex, string compositePart) in
                     category.Value) CreateBindingItem(action, config, bindingIndex, compositePart);
        }
    }

    private void CreateCategoryHeader(string categoryName)
    {
        if (m_CategoryHeaderPrefab == null)
            return;

        GameObject headerObj = Instantiate(m_CategoryHeaderPrefab, m_ScrollContent);
        TextMeshProUGUI headerText = headerObj.GetComponentInChildren<TextMeshProUGUI>();
        if (headerText != null)
            headerText.text = categoryName;
        m_CreatedCategoryHeaders.Add(headerObj);
    }

    private void CreateBindingItem(InputAction action, ActionBinding config, int bindingIndex, string compositePart)
    {
        string baseName = config?.DisplayName ?? action.name;
        string displayName = string.IsNullOrEmpty(compositePart)
            ? baseName
            : $"{baseName} ({compositePart})";

        ActionBinding actionBinding = new()
        {
            ActionName = action.name,
            DisplayName = displayName,
            Category = config?.Category ?? m_DefaultCategory,
            BindingIcon = config?.BindingIcon
        };

        UI_KeybindingKeyboardItem item = Instantiate(m_ItemPrefab, m_ScrollContent);
        item.Initialize(this, actionBinding, action, bindingIndex);
        m_CreatedItems.Add(item);
    }

    private void SetAllItemsInteractable(bool interactable)
    {
        foreach (UI_KeybindingKeyboardItem item in m_CreatedItems)
            item.SetInteractable(interactable);
    }

    public void StartRebind(UI_KeybindingKeyboardItem item, InputAction action, int bindingIndex)
    {
        if (m_IsRebinding || m_RebindPopup == null)
            return;

        m_IsRebinding = true;
        SetAllItemsInteractable(false);

        string actionName = item.ActionBinding?.DisplayName ?? action.name;
        m_RebindPopup.Show(actionName, action, bindingIndex,
            newBindingPath =>
            {
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

    private string GetBindingKey(InputAction action, int bindingIndex)
    {
        return $"{action.actionMap.name}/{action.name}/{bindingIndex}";
    }

    private void ApplySessionOverride(InputAction action, int bindingIndex, string newPath)
    {
        string key = GetBindingKey(action, bindingIndex);
        m_SessionOverrides[key] = newPath;
        action.ApplyBindingOverride(bindingIndex, newPath);
    }

    private void ApplySessionOverrides()
    {
        if (m_InputActionAsset == null)
            return;

        InputActionMap actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null)
            return;

        foreach (KeyValuePair<string, string> kvp in m_SessionOverrides)
        {
            string[] parts = kvp.Key.Split('/');
            if (parts.Length != 3)
                continue;

            InputAction action = actionMap.FindAction(parts[1]);
            if (action == null)
                continue;

            if (int.TryParse(parts[2], out int bindingIndex)) action.ApplyBindingOverride(bindingIndex, kvp.Value);
        }
    }

    public void ResetAllBindings()
    {
        if (m_InputActionAsset == null)
            return;

        InputActionMap actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null)
            return;

        m_SessionOverrides.Clear();

        foreach (InputAction action in actionMap.actions) action.RemoveAllBindingOverrides();

        RefreshUI();
    }
}