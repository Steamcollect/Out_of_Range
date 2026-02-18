using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_KeybindingManager : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset m_InputActionAsset;
    [SerializeField] private string m_ActionMapName = "Player";
    [SerializeField] private InputDeviceType m_DeviceType = InputDeviceType.KeyboardMouse;

    [Header("Category Configuration")]
    [SerializeField] private List<ActionBinding> m_ActionBindings = new();
    [SerializeField] private string m_DefaultCategory = "General";

    [Header("Action Filtering")]
    [SerializeField] private List<string> m_ExcludedActions = new();
    [SerializeField] private bool m_OnlyShowDefinedActions;

    [Header("UI References")]
    [SerializeField] private Transform m_ScrollContent;
    [SerializeField] private UI_KeybindingKeyboardItem m_KeyboardItemPrefab;
    [SerializeField] private UI_KeybindingGamepadItem m_GamepadItemPrefab;
    [SerializeField] private GameObject m_CategoryHeaderPrefab;
    [SerializeField] private UI_KeybindingRebindPopup m_RebindPopup;

    [Header("Gamepad Icons")]
    [SerializeField] private SSO_InputBindingIconResolver m_IconResolver;

    private readonly List<MonoBehaviour> m_CreatedItems = new();
    private readonly List<GameObject> m_CreatedCategoryHeaders = new();
    private readonly Dictionary<string, string> m_SessionOverrides = new();
    private bool m_IsRebinding;
    private string m_CurrentKeyboardLayout;

    public InputDeviceType DeviceType => m_DeviceType;
    public SSO_InputBindingIconResolver IconResolver => m_IconResolver;

    [Serializable]
    public class ActionBinding
    {
        public string ActionName;
        public string DisplayName;
        public string Category = "General";
        public Sprite BindingIcon;
        [Tooltip("If true, shows a secondary binding slot for this action.")]
        public bool HasSecondarySlot = true;
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
                RefreshAllItemDisplays();
            }
        }
    }

    private void RefreshAllItemDisplays()
    {
        foreach (var item in m_CreatedItems)
        {
            if (item is UI_KeybindingKeyboardItem keyboardItem)
                keyboardItem.UpdateDisplay();
            else if (item is UI_KeybindingGamepadItem gamepadItem)
                gamepadItem.UpdateDisplay();
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

    private Dictionary<string, List<(InputAction action, ActionBinding config, List<int> bindingIndices, string compositePart)>>
        GetActionsGroupedByCategory()
    {
        var categories = new Dictionary<string, List<(InputAction, ActionBinding, List<int>, string)>>();

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
                categories[category] = new List<(InputAction, ActionBinding, List<int>, string)>();

            var bindingsGrouped = FindBindingsGroupedByAction(action);
            foreach (var (bindingIndices, compositePart) in bindingsGrouped)
                categories[category].Add((action, config, bindingIndices, compositePart));
        }

        return categories;
    }

    private List<(List<int> bindingIndices, string compositePart)> FindBindingsGroupedByAction(InputAction action)
    {
        var results = new Dictionary<string, List<int>>();

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (binding.isComposite)
                continue;

            bool isMatch = m_DeviceType == InputDeviceType.KeyboardMouse
                ? IsKeyboardMouseBinding(binding)
                : IsGamepadBinding(binding);

            if (!isMatch) continue;

            string key = binding.isPartOfComposite ? binding.name : "__simple__";

            if (!results.ContainsKey(key))
                results[key] = new List<int>();

            results[key].Add(i);
        }

        var output = new List<(List<int>, string)>();
        foreach (var kvp in results)
        {
            string compositePart = kvp.Key == "__simple__" ? null : kvp.Key;
            output.Add((kvp.Value, compositePart));
        }

        return output;
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

    private static bool IsGamepadBinding(InputBinding binding)
    {
        if (!string.IsNullOrEmpty(binding.groups) && binding.groups.Contains("Gamepad"))
            return true;

        if (!string.IsNullOrEmpty(binding.path))
            if (binding.path.Contains("<Gamepad>"))
                return true;

        if (!string.IsNullOrEmpty(binding.effectivePath))
            if (binding.effectivePath.Contains("<Gamepad>"))
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
        foreach (var item in m_CreatedItems)
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
        if (m_InputActionAsset == null || m_ScrollContent == null)
            return;

        bool hasPrefab = m_DeviceType == InputDeviceType.KeyboardMouse
            ? m_KeyboardItemPrefab != null
            : m_GamepadItemPrefab != null;

        if (!hasPrefab)
            return;

        InputActionMap actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null)
            return;

        var categorizedActions = GetActionsGroupedByCategory();

        foreach (var category in categorizedActions)
        {
            CreateCategoryHeader(category.Key);

            foreach (var (action, config, bindingIndices, compositePart) in category.Value)
                CreateBindingItem(action, config, bindingIndices, compositePart);
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

    private void CreateBindingItem(InputAction action, ActionBinding config, List<int> bindingIndices, string compositePart)
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
            BindingIcon = config?.BindingIcon,
            HasSecondarySlot = config?.HasSecondarySlot ?? false // Si pas de config, pas de slot secondaire
        };

        int primaryIndex = bindingIndices.Count > 0 ? bindingIndices[0] : -1;
        int secondaryIndex = bindingIndices.Count > 1 ? bindingIndices[1] : -1;

        if (m_DeviceType == InputDeviceType.KeyboardMouse)
        {
            UI_KeybindingKeyboardItem item = Instantiate(m_KeyboardItemPrefab, m_ScrollContent);
            item.Initialize(this, actionBinding, action, primaryIndex, secondaryIndex);
            m_CreatedItems.Add(item);
        }
        else
        {
            UI_KeybindingGamepadItem item = Instantiate(m_GamepadItemPrefab, m_ScrollContent);
            item.Initialize(this, actionBinding, action, primaryIndex, secondaryIndex);
            m_CreatedItems.Add(item);
        }
    }

    private void SetAllItemsInteractable(bool interactable)
    {
        foreach (var item in m_CreatedItems)
        {
            if (item is UI_KeybindingKeyboardItem keyboardItem)
                keyboardItem.SetInteractable(interactable);
            else if (item is UI_KeybindingGamepadItem gamepadItem)
                gamepadItem.SetInteractable(interactable);
        }
    }

    public void StartRebind(MonoBehaviour item, InputAction action, int bindingIndex, string displayName, bool isPrimary)
    {
        if (m_IsRebinding || m_RebindPopup == null)
            return;

        m_IsRebinding = true;
        SetAllItemsInteractable(false);

        string slotLabel = isPrimary ? "Primary" : "Secondary";
        string fullDisplayName = $"{displayName} ({slotLabel})";

        m_RebindPopup.Show(fullDisplayName, action, bindingIndex,
            newBindingPath =>
            {
                ApplySessionOverride(action, bindingIndex, newBindingPath);
                UpdateItemDisplay(item);
                EndRebind();
            },
            EndRebind,
            () =>
            {
                ClearBinding(action, bindingIndex);
                UpdateItemDisplay(item);
                EndRebind();
            });
    }

    public void StartRebindNewBinding(MonoBehaviour item, InputAction action, string displayName, bool isPrimary)
    {
        if (m_IsRebinding || m_RebindPopup == null)
            return;

        m_IsRebinding = true;
        SetAllItemsInteractable(false);

        string slotLabel = isPrimary ? "Primary" : "Secondary";
        string fullDisplayName = $"{displayName} ({slotLabel})";

        // Créer un nouveau binding pour cette action
        string bindingGroup = m_DeviceType == InputDeviceType.KeyboardMouse ? "Keyboard&Mouse" : "Gamepad";
        
        InputBinding newBinding = new InputBinding
        {
            path = "",
            interactions = "",
            processors = "",
            groups = bindingGroup,
            action = action.name
        };

        action.AddBinding(newBinding);
        int newBindingIndex = action.bindings.Count - 1;

        // Mettre à jour l'item avec le nouvel index
        if (item is UI_KeybindingGamepadItem gamepadItem)
            gamepadItem.SetSecondaryBindingIndex(newBindingIndex);
        else if (item is UI_KeybindingKeyboardItem keyboardItem)
            keyboardItem.SetSecondaryBindingIndex(newBindingIndex);

        m_RebindPopup.Show(fullDisplayName, action, newBindingIndex,
            newBindingPath =>
            {
                ApplySessionOverride(action, newBindingIndex, newBindingPath);
                UpdateItemDisplay(item);
                EndRebind();
            },
            () =>
            {
                // Annulation : supprimer le binding vide créé
                // Note: InputSystem ne permet pas de supprimer facilement un binding,
                // on le laisse vide et on met à jour l'index à -1
                if (item is UI_KeybindingGamepadItem gpi)
                    gpi.SetSecondaryBindingIndex(-1);
                else if (item is UI_KeybindingKeyboardItem kbi)
                    kbi.SetSecondaryBindingIndex(-1);
                EndRebind();
            },
            () =>
            {
                // Clear : même comportement que cancel pour un nouveau binding
                if (item is UI_KeybindingGamepadItem gpi)
                    gpi.SetSecondaryBindingIndex(-1);
                else if (item is UI_KeybindingKeyboardItem kbi)
                    kbi.SetSecondaryBindingIndex(-1);
                EndRebind();
            });
    }

    private void UpdateItemDisplay(MonoBehaviour item)
    {
        if (item is UI_KeybindingKeyboardItem keyboardItem)
            keyboardItem.UpdateDisplay();
        else if (item is UI_KeybindingGamepadItem gamepadItem)
            gamepadItem.UpdateDisplay();
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

    private void ClearBinding(InputAction action, int bindingIndex)
    {
        string key = GetBindingKey(action, bindingIndex);
        m_SessionOverrides[key] = "";
        action.ApplyBindingOverride(bindingIndex, "");
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

            if (int.TryParse(parts[2], out int bindingIndex))
                action.ApplyBindingOverride(bindingIndex, kvp.Value);
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

        foreach (InputAction action in actionMap.actions)
            action.RemoveAllBindingOverrides();

        RefreshUI();
    }
}

