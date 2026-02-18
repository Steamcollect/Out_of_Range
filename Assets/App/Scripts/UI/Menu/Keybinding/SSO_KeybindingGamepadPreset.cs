using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SSO_KeybindingGamepadPreset", menuName = "SSO/Input/Keybinding Gamepad Preset")]
public class SSO_KeybindingGamepadPreset : ScriptableObject
{
    [Header("Preset Info")]
    [SerializeField] private string m_PresetName = "Default";

    [SerializeField] private Sprite m_PresetIcon;

    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset m_InputActionAsset;

    [SerializeField] private string m_ActionMapName = "Player";
    [SerializeField] private List<ActionBinding> m_ActionBindings = new();

    public string PresetName => m_PresetName;
    public Sprite PresetIcon => m_PresetIcon;
    public InputActionAsset InputActionAsset => m_InputActionAsset;
    public string ActionMapName => m_ActionMapName;
    public List<ActionBinding> ActionBindings => m_ActionBindings;

    [Serializable]
    public class ActionBinding
    {
        [Tooltip("Name of the action in the InputActionMap")]
        public string ActionName;

        [Tooltip("Category for grouping actions in the UI")]
        public string Category = "General";

        [Tooltip("List of gamepad binding paths for this action (supports multiple inputs)")]
        public List<BindingPath> BindingPaths = new();
    }

    [Serializable]
    public class BindingPath
    {
        [Tooltip("The gamepad binding path (e.g., <Gamepad>/buttonSouth)")]
        public string Path;

        [Tooltip("Optional: Index of the binding to override. -1 means first gamepad binding found.")]
        public int BindingIndex = -1;

        [Tooltip("Optional: For composite bindings, specify the part name (e.g., 'up', 'down')")]
        public string CompositePart;
    }

    /// <summary>
    /// Applies this preset's bindings to the InputActionAsset
    /// </summary>
    public void ApplyPreset()
    {
        if (m_InputActionAsset == null) return;

        InputActionMap actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return;

        foreach (ActionBinding binding in m_ActionBindings)
        {
            InputAction action = actionMap.FindAction(binding.ActionName);
            if (action == null) continue;

            // Apply all binding paths
            foreach (BindingPath bindingPath in binding.BindingPaths)
                ApplySingleBinding(action, bindingPath.Path, bindingPath.BindingIndex, bindingPath.CompositePart);
        }
    }

    private void ApplySingleBinding(InputAction action, string path, int bindingIndex, string compositePart)
    {
        if (string.IsNullOrEmpty(path)) return;

        if (bindingIndex >= 0 && bindingIndex < action.bindings.Count)
            // Use specific binding index
            action.ApplyBindingOverride(bindingIndex, path);
        else if (!string.IsNullOrEmpty(compositePart))
            // Find composite part binding
            for (int i = 0; i < action.bindings.Count; i++)
            {
                InputBinding b = action.bindings[i];
                if (b.isPartOfComposite &&
                    b.name.Equals(compositePart, StringComparison.OrdinalIgnoreCase) &&
                    IsGamepadBinding(b))
                {
                    action.ApplyBindingOverride(i, path);
                    break;
                }
            }
        else
            // Find first gamepad binding
            for (int i = 0; i < action.bindings.Count; i++)
                if (IsGamepadBinding(action.bindings[i]) && !action.bindings[i].isComposite)
                {
                    action.ApplyBindingOverride(i, path);
                    break;
                }
    }

    private bool IsGamepadBinding(InputBinding binding)
    {
        return binding.groups.Contains("Gamepad") || binding.path.Contains("<Gamepad>");
    }

    /// <summary>
    /// Removes all binding overrides applied by this preset
    /// </summary>
    public void RemovePreset()
    {
        if (m_InputActionAsset == null) return;

        InputActionMap actionMap = m_InputActionAsset.FindActionMap(m_ActionMapName);
        if (actionMap == null) return;

        foreach (ActionBinding binding in m_ActionBindings)
        {
            InputAction action = actionMap.FindAction(binding.ActionName);
            if (action == null) continue;

            // Remove all gamepad binding overrides for this action
            for (int i = 0; i < action.bindings.Count; i++)
                if (IsGamepadBinding(action.bindings[i]))
                    action.RemoveBindingOverride(i);
        }
    }

    /// <summary>
    /// Gets actions grouped by category
    /// </summary>
    public Dictionary<string, List<ActionBinding>> GetActionsByCategory()
    {
        var categories = new Dictionary<string, List<ActionBinding>>();

        foreach (ActionBinding binding in m_ActionBindings)
        {
            if (!categories.ContainsKey(binding.Category)) categories[binding.Category] = new List<ActionBinding>();
            categories[binding.Category].Add(binding);
        }

        return categories;
    }

    /// <summary>
    /// Gets all binding paths for a specific action
    /// </summary>
    public List<BindingPath> GetBindingPathsForAction(string actionName)
    {
        ActionBinding binding = m_ActionBindings.Find(b => b.ActionName == actionName);
        return binding?.BindingPaths ?? new List<BindingPath>();
    }
}